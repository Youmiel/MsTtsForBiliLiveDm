using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MsTtsForBiliLiveDm.MsTts
{
    public class MsTtsGetter
    {
        private static readonly string API_ADDRESS = "wss://eastus.api.speech.microsoft.com/cognitiveservices/websocket/v1";
        private static readonly string TRAFFIC_TYPE = "AzureDemo";
        private static readonly string AUTHORIIZATION = "bearer%20undefined";

        private string voiceType;

        public string VoiceType { get => this.voiceType; set => this.voiceType = value; }

        public MsTtsGetter() : this(MsVoiceType.XiaoxiaoNeural) { }

        public MsTtsGetter(string voiceType)
        {
            this.voiceType = voiceType;
            this.AutoSetup();
        }

        public void AutoSetup() { }

        private async Task<ClientWebSocket> CreateConnection()
        {
            string connectionId = Guid.NewGuid().ToString("N").ToUpper();

            UriBuilder uriBuilder = new UriBuilder(API_ADDRESS);
            uriBuilder.Query = String.Format("TrafficType={0}&Authorization={1}&X-ConnectionId={2}", TRAFFIC_TYPE, AUTHORIIZATION, connectionId);

            ClientWebSocket connection = new ClientWebSocket();
            //connection.Options.UseDefaultCredentials = true;
            //connection.Options.SetRequestHeader("Host", "eastus.api.speech.microsoft.com");
            connection.Options.SetRequestHeader("Origin", "https://azure.microsoft.com");
            //connection.Options.SetRequestHeader("User-Agent", "Edg/108.0.1462.46");
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
                    "<voice name=\"" + this.voiceType + "\">" +
                        "<prosody rate=\"0%\" pitch=\"0%\">" +
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

        public byte[] GetTtsAudio(string ttsText)
        {
            Task<byte[]> task = this.RequestTtsAudio(ttsText);
            task.Wait();

            return task.Result;
        }
    }
}
