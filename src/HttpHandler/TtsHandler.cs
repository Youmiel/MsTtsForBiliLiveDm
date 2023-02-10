using MsTtsForBiliDanmaku.HttpHandler;
using MsTtsForBiliLiveDm.MsTts;
using MsTtsForBiliLiveDm.Plugin;
using MsTtsForBiliLiveDm.Utils;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;

namespace MsTtsForBiliLiveDm.HttpHandler
{
    public class TtsHandler : BaseHttpHandler, IConfigurable
    {
        private MsTtsGetter ttsGetter;

        public MsTtsGetter TTSGetter => this.ttsGetter;

        public TtsHandler(string contextRoot, int port = 8080) : base(contextRoot, port)
        {
            this.RequestHandle = this.HandleTtsRequest;
            this.ttsGetter = new MsTtsGetter();
        }
        private void HandleTtsRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            NameValueCollection urlParameters = HttpUtility.ParseQueryString(request.Url.Query, Encoding.UTF8);
            //NameValueCollection urlParameters = request.QueryString;
            string danmakuText = urlParameters.Get("text");

            if (danmakuText != null)
            {
                Util.DebugContent("Received text: " + danmakuText);
                byte[] content = this.ttsGetter.GetTtsAudio(danmakuText);
                if (content != null)
                {
                    response.StatusCode = 200;
                    response.ContentType = "audio/mpeg";

                    response.OutputStream.Write(content, 0, content.Length);
                    response.Close();
                    return;
                }
                else
                {
                    response.StatusCode = 503;
                }
            }
            else
            {
                response.StatusCode = 400;
            }
            response.Close();
        }

        public void ApplyConfig(PluginConfig config)
        {
            if (config.Port != this.Port)
            {
                bool shouldRestart = this.IsRunning;

                if (shouldRestart) { this.Stop(); }
                this.port = config.Port;
                this.SetupListener(this.contextRoot, this.port);
                if (shouldRestart) { this.Start(); }

                this.ttsGetter.ApplyConfig(config);
            }
        }

        public void FetchConfig(PluginConfig config)
        {
            config.Port = this.port;

            this.ttsGetter.FetchConfig(config);
        }
    }
}
