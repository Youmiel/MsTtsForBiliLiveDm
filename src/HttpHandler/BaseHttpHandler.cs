using MsTtsForBiliLiveDm.Utils;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MsTtsForBiliDanmaku.HttpHandler
{
    public class BaseHttpHandler
    {
        public delegate void ContextHandleMethod(HttpListenerContext context);
        public delegate void RequestHandleMethod(HttpListenerRequest request, HttpListenerResponse response);

        private volatile Task listenTask = null;
        private volatile bool running = false;

        protected HttpListener httpListener = new HttpListener();
        protected ContextHandleMethod contextHandle = null;
        protected RequestHandleMethod requestHandle = null;

        //public Task GetListenTask => this.listenTask;

        public bool IsRunning => this.running;
        public ContextHandleMethod ContextHandle { get => this.contextHandle; set => this.contextHandle = value; }
        public RequestHandleMethod RequestHandle { get => this.requestHandle; set => this.requestHandle = value; }

        public BaseHttpHandler(string contextRoot, int port = 8080)
        {
            this.httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            if (contextRoot.Length > 0 && !contextRoot.EndsWith("/"))
                contextRoot += "/";
            this.httpListener.Prefixes.Add(string.Format("http://+:{0}/{1}", port, contextRoot));
        }

        public Task Start()
        {
            if (this.listenTask != null) return this.listenTask;
            this.httpListener.Start();
            this.running = true;
            this.listenTask = Task.Run(() =>
            {
                this.Listen();
            });

            return this.listenTask;
        }

        private void Listen()
        {
            IAsyncResult ar = null;
            while (this.running)
            {
                ar = this.httpListener.BeginGetContext(new AsyncCallback(ContextGetEnd), null);
                ar.AsyncWaitHandle.WaitOne();
            }

            void ContextGetEnd(IAsyncResult asyncResult)
            {
                var context = this.httpListener.EndGetContext(asyncResult);
                this.Handle(context);
            }
        }
 
        protected void Handle(HttpListenerContext context)
        {
            if (this.contextHandle != null) this.contextHandle(context);

            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            if (this.requestHandle != null) this.requestHandle(request, response);
        }

        public async Task Stop()
        {
            this.running = false;
            await this.listenTask;
            this.listenTask = null;
        }

        public void StopAndWait()
        {
            Task task = this.Stop();
            task.Wait();
        }
    }
}
