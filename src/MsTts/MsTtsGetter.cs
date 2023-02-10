using MsTtsForBiliLiveDm.Plugin;
using MsTtsForBiliLiveDm.Utils;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MsTtsForBiliLiveDm.MsTts
{
    public class MsTtsGetter: IConfigurable
    {
        //private static readonly string API_ADDRESS = "wss://eastus.api.speech.microsoft.com/cognitiveservices/websocket/v1";
        private static readonly string TRAFFIC_TYPE = "AzureDemo";
        private static readonly string AUTHORIIZATION = "bearer%20undefined";
        private static readonly int RETRY_INTERVAL = 1000;
        private static readonly TimeSpan QUERY_INTERVAL = TimeSpan.FromSeconds(5.0);

        private MsVoiceType voiceType;
        private int rate;
        private int pitch;
        private MsAPIProvider apiProvider;
        private DateTime lastQueryTime;

        public MsVoiceType VoiceType { get => this.voiceType; set => this.voiceType = value; }
        public int Rate { get => this.rate; set => this.rate = Util.Clamp(value, -100, 200); }
        public int Pitch { get => this.pitch; set => this.pitch = Util.Clamp(value, -50, 50); }

        public MsTtsGetter() : this(MsVoiceType.XiaoxiaoNeural) { }

        public MsTtsGetter(MsVoiceType voiceType)
        {
            this.voiceType = voiceType;
            this.rate = 0;
            this.pitch = 0;
            this.apiProvider = new MsAPIProvider();
            this.lastQueryTime = DateTime.Now;
            this.AutoSetup();
        }

        public void AutoSetup() { }

        private async Task<ClientWebSocket> CreateConnection()
        {
            string connectionId = Guid.NewGuid().ToString("N").ToUpper();

            string url = this.apiProvider.ProvideUrl();

            if (url == null)
            {
                Util.LogContent("今日请求次数已达上限");
                return null;
            }

            UriBuilder uriBuilder = new UriBuilder(url);
            uriBuilder.Query = String.Format("TrafficType={0}&Authorization={1}&X-ConnectionId={2}", TRAFFIC_TYPE, AUTHORIIZATION, connectionId);

            ClientWebSocket connection = new ClientWebSocket();
            //connection.Options.UseDefaultCredentials = true;
            //connection.Options.SetRequestHeader("Host", "eastus.api.speech.microsoft.com");
            connection.Options.SetRequestHeader("Origin", "https://azure.microsoft.com");
            //connection.Options.SetRequestHeader("User-Agent", "Edge/108.0.1462.46");  // cannot do this
            await connection.ConnectAsync(uriBuilder.Uri, CancellationToken.None);
            return connection;
        }

        private static StringBuilder CreateRequestHead(string path, string requestId, string timestamp, string contentType)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Path: ").AppendLine(path);
            sb.Append("X-RequestId: ").AppendLine(requestId);
            sb.Append("X-Timestamp: ").AppendLine(timestamp);
            sb.Append("Content-Type: ").AppendLine(contentType).AppendLine();

            return sb;
        }

        private static async Task SendTextContent(ClientWebSocket connection, String text)
        {
            byte[] content = System.Text.Encoding.UTF8.GetBytes(text);
            ArraySegment<byte> data = new System.ArraySegment<byte>(content);
            await connection.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task SendSpeechConfig(ClientWebSocket connection, string requestId, string timestamp)
        {
            StringBuilder sb = CreateRequestHead("speech.config", requestId, timestamp, "application/json");

            sb.AppendLine("{\"context\":{\"system\":{\"name\":\"SpeechSDK\",\"version\":\"1.19.0\",\"build\":\"JavaScript\",\"lang\":\"JavaScript\"}," +
                "\"os\":{\"platform\":\"Browser/Win32\",\"name\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.46\"," +
                "\"version\":\"5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.46\"}}}");

            await SendTextContent(connection, sb.ToString());
        }

        private async Task SendSynthesisContext(ClientWebSocket connection, string requestId, string timestamp)
        {
            StringBuilder sb = CreateRequestHead("synthesis.context", requestId, timestamp, "application/json");

            sb.AppendLine("{\"synthesis\":{\"audio\":{\"metadataOptions\":{\"bookmarkEnabled\":false,\"sentenceBoundaryEnabled\":false," +
                "\"visemeEnabled\":false,\"wordBoundaryEnabled\":false},\"outputFormat\":\"audio-24khz-96kbitrate-mono-mp3\"}," +
                "\"language\":{\"autoDetection\":false}}}");

            await SendTextContent(connection, sb.ToString());
        }

        private async Task SendSSML(ClientWebSocket connection, string requestId, string timestamp, string ttsText)
        {
            StringBuilder sb = CreateRequestHead("ssml", requestId, timestamp, "application/ssml+xml");
            string ssml = "<speak xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:mstts=\"http://www.w3.org/2001/mstts\" xmlns:emo=\"http://www.w3.org/2009/10/emotionml\" version=\"1.0\" xml:lang=\"en-US\">" +
                    "<voice name=\"" + this.voiceType.InternalName + "\">" +
                        "<prosody rate=\"" + this.rate + "%\" pitch=\"" + this.pitch + "%\">" +
                            ttsText +
                        "</prosody>" +
                    "</voice>" +
                "</speak>";
            sb.AppendLine(ssml);

            await SendTextContent(connection, sb.ToString());
        }

        private async Task<byte[]> ReceiveAudio(ClientWebSocket connection)
        {
            List<byte> audioData = new List<byte>();
            bool readingAudio = false;

            int audioSize = 0;
            List<int> audioSizeRecord = new List<int>();

            while (true)
            {
                List<byte> rawData = new List<byte>();
                List<int> sizeRecord = new List<int>();
                int receivedSize = 0;

                WebSocketReceiveResult receiveResult;
                while (true)
                {
                    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                    receiveResult = await connection.ReceiveAsync(buffer, CancellationToken.None);

                    ArraySegment<byte> dataSegment = new ArraySegment<byte>(buffer.Array, buffer.Offset, receiveResult.Count);
                    rawData.AddRange(dataSegment);

                    receivedSize += receiveResult.Count;
                    sizeRecord.Add(receiveResult.Count);

                    if (receiveResult.EndOfMessage)
                        break;
                }
                //Console.WriteLine((receivedSize == rawData.Count).ToString());

                if (receiveResult.MessageType == WebSocketMessageType.Binary)
                {
                    audioData.AddRange(rawData);
                    if (!readingAudio) readingAudio = true;

                    audioSize += receivedSize;
                    audioSizeRecord.Add(receivedSize);
                }
                else if (receiveResult.MessageType == WebSocketMessageType.Text && readingAudio)
                {
                    char[] textData = Encoding.UTF8.GetChars(rawData.ToArray());
                    string text = new string(textData);
                    if (text.Contains("Path:turn.end"))
                        break;
                }
                else if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }
            }
            //Console.WriteLine("TotalSize: " + (audioData.Count == audioSize).ToString());

            return audioData.ToArray();
        }

        public async Task<byte[]> RequestTtsAudio(string ttsText)
        {
            ClientWebSocket connection = await this.CreateConnection();
            string requestId = Guid.NewGuid().ToString("N").ToUpper();
            string timestamp = DateTime.Now.ToUniversalTime().ToString("yyy-MM-dd'T'HH:mm:ss.fff'Z'");
            await this.SendSpeechConfig(connection, requestId, timestamp);
            await this.SendSynthesisContext(connection, requestId, timestamp);
            await this.SendSSML(connection, requestId, timestamp, ttsText);
            byte[] data = await this.ReceiveAudio(connection);
            _ = connection.CloseAsync(WebSocketCloseStatus.NormalClosure, "close", CancellationToken.None);
            return data;
        }

        public byte[] GetTtsAudio(string ttsText, int times = 1)
        {
            DateTime queryTime = DateTime.Now;
            if (queryTime.Subtract(this.lastQueryTime).CompareTo(QUERY_INTERVAL) < 0)
                return new byte[0];
            // 下次一定重构

            this.lastQueryTime = queryTime;

            for (int t = 0; t < times; t++)
            {
                Task<byte[]> task;
                try
                {
                    task = this.RequestTtsAudio(ttsText);
                    task.Wait();

                    return task.Result;
                }
                catch (AggregateException ae)
                {
                    Util.DebugContent(ae.ToString());
                    //Util.DebugContent("");
                    //Util.DebugContent(ae.InnerException.ToString());
                    if (t < times - 1)
                        Util.LogContent($"Error occured when requesting audio, retrying... T={t + 1}");
                    else if (times > 1)
                        Util.LogContent($"All retry attempts have failed.");
                }
                if (t < times - 1)
                    Thread.Sleep(RETRY_INTERVAL);
            }
            return null;
        }

        public void ApplyConfig(PluginConfig config)
        {
            this.voiceType = config.VoiceType;
            this.rate = config.Rate;
            this.pitch = config.Pitch;

            this.apiProvider.ApplyConfig(config);
        }

        public void FetchConfig(PluginConfig config)
        {
            config.VoiceType = this.voiceType;
            config.Rate = this.rate;
            config.Pitch = this.pitch;

            this.apiProvider.FetchConfig(config);
        }
    }
}
