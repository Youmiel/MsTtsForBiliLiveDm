using MsTtsForBiliLiveDm.Utils;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MsTtsForBiliDanmaku.HttpHandler
{
    public class BaseHttpHandler
    {
        public delegate void ContextHandleMethod(HttpListenerContext context);
        public delegate void RequestHandleMethod(HttpListenerRequest request, HttpListenerResponse response);

        private volatile Thread listenThread = null;
        private volatile bool running = false;

        protected int port;
        protected string contextRoot;
        protected HttpListener httpListener = new HttpListener();
        protected ContextHandleMethod contextHandle = null;
        protected RequestHandleMethod requestHandle = null;

        //public Task GetListenTask => this.listenTask;

        public bool IsRunning => this.running;
        public int Port { get => this.port; }
        public string ContextRoot { get => this.contextRoot; }

        public ContextHandleMethod ContextHandle { get => this.contextHandle; set => this.contextHandle = value; }
        public RequestHandleMethod RequestHandle { get => this.requestHandle; set => this.requestHandle = value; }

        public BaseHttpHandler(string contextRoot, int port = 8080)
        {
            if (contextRoot.Length > 0 && !contextRoot.EndsWith("/"))
                contextRoot += "/";
            this.port = port;
            this.contextRoot = contextRoot;

            SetupListener(this.contextRoot, this.port);
        }

        protected void SetupListener(string contextRoot, int port)
        {
            this.httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            this.httpListener.Prefixes.Clear();
            this.httpListener.Prefixes.Add(string.Format("http://localhost:{0}/{1}", port, contextRoot));
        }

        public Thread Start()
        {
            if (this.listenThread != null && this.running)
                return this.listenThread;

            this.httpListener.Start();
            this.running = true;
            this.listenThread = new Thread(() =>
            {
                this.Listen();
            });
            this.listenThread.Start();

            return this.listenThread;
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
                try
                {
                    HttpListenerContext context = this.httpListener.EndGetContext(asyncResult);
                    this.Handle(context);
                }
                catch (Exception e)
                {
                    Util.LogContent(e.ToString());
                }
            }
        }

        protected void Handle(HttpListenerContext context)
        {
            if (this.contextHandle != null) this.contextHandle(context);

            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            if (this.requestHandle != null) this.requestHandle(request, response);
        }

        public Thread Stop()
        {
            Thread oldThread = this.listenThread;
            this.running = false;
            this.httpListener.Abort();
            if (this.listenThread != null)
            {
                this.listenThread.Abort();
                this.listenThread = null;
            }

            return oldThread;
        }

        public void StopAndWait()
        {
            Thread oldThread = this.Stop();
            oldThread.Join();
        }

        public void Destory()
        {
            this.Stop();
            this.httpListener.Close();
        }

        //public async Task Stop()
        //{
        //    this.running = false;
        //    this.httpListener.Stop();  // 我搞不懂为啥这插件不让我关listener, 一关就炸
        //    await this.listenTask;
        //    this.listenTask = null;
        //}

        //public void StopAndWait()
        //{
        //    Task task = this.Stop();
        //    task.Wait();
        //}
    }
}
