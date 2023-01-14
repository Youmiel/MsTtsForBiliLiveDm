using MsTtsForBiliDanmaku.HttpHandler;
using MsTtsForBiliLiveDm.MsTts;
using MsTtsForBiliLiveDm.Utils;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;

namespace MsTtsForBiliLiveDm.HttpHandler
{
    public class TtsHandler : BaseHttpHandler
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
                byte[] content = this.ttsGetter.GetTtsAudio(danmakuText, 3);
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
    }
}
