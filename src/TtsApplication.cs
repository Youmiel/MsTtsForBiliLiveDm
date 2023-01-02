using MsTtsForBiliLiveDm.HttpHandler;
using MsTtsForBiliLiveDm.Plugin;
using MsTtsForBiliLiveDm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MsTtsForBiliLiveDm
{
    public class TtsApplication : Application
    {
        private PluginConfig config = null;
        private ConfigWindow configWindow = null;
        private TtsHandler ttsHandler = null;

        public TtsApplication()
        {
            this.LoadConfig();

            //Util.RunInSTAThread((ThreadStart)delegate
            //{
            ConfigWindow cw = new ConfigWindow();
            cw.CloseBehaviour = CloseBehaviourEnum.CROSS_CLOSE;
            cw.ConfigApplyAsync = (cfg) => this.ApplyConfig(cfg);

            this.configWindow = cw;
            this.configWindow.BindConfig(this.config);

            this.MainWindow = cw;
            this.MainWindow.ShowDialog();
            //});
        }

        public PluginConfig LoadConfig()
        {
            PluginConfig oldConfig = this.config;
            this.config = PluginConfig.LoadConfig(PluginConfig.CONFIG_PATH);
            return oldConfig;
        }

        private void ApplyConfig(PluginConfig config)
        {
            if (this.ttsHandler == null)
                this.ttsHandler = new TtsHandler("", config.Port);

            if (config.Port != this.ttsHandler.Port)
            {
                TtsHandler oldHandler = this.ttsHandler;
                this.ttsHandler = new TtsHandler("", config.Port);
                if (oldHandler.IsRunning)
                {
                    _ = oldHandler.Stop();
                    this.ttsHandler.Start();
                }
            }

            this.ttsHandler.TTSGetter.VoiceType = config.VoiceType;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.ttsHandler = new TtsHandler("", this.config.Port);
            this.ttsHandler.TTSGetter.VoiceType = this.config.VoiceType;

            ttsHandler.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (this.ttsHandler == null)
                return;

            _ = this.ttsHandler.Stop();
        }

        public static void Main()
        {
            Util.DebugContent = (string content) => Console.Out.WriteLine(content);
            Util.LogContent = (string content) => Console.Out.WriteLine(content);

            TtsApplication app;
            Util.RunInSTAThread(() =>
            {
                app = new TtsApplication();
                app.Run();
            });

            Console.ReadLine();
        }
    }
}
