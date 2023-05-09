using MsTtsForBiliLiveDm.HttpHandler;
using MsTtsForBiliLiveDm.MsTts;
using MsTtsForBiliLiveDm.Plugin;
using MsTtsForBiliLiveDm.Plugin.Serialization;
using MsTtsForBiliLiveDm.Utils;
using System;
using System.Windows;

namespace MsTtsForBiliLiveDm
{
    public class TtsApplication : Application, IConfigurable
    {
        //private PluginConfig config = null;
        private ConfigWindow configWindow = null;
        private TtsHandler ttsHandler = null;

        public TtsApplication()
        {
            //this.LoadConfig();

            //Util.RunInSTAThread((ThreadStart)delegate
            //{
            ConfigWindow cw = new ConfigWindow();
            cw.CloseBehaviour = CloseBehaviourEnum.CROSS_CLOSE;
            cw.ConfigApplyAsync = (cfg) =>
            {
                this.ApplyConfig(cfg);
                //SerializationManager.Config.Value.CopyValueOf(cfg);
                SerializationManager.Config.Save();
                //PluginConfig.SaveConfig(PluginConfig.CONFIG_PATH, this.config);
            };
            this.configWindow = cw;
            this.configWindow.BindConfig(SerializationManager.Config.Value);
            //this.configWindow.BindConfig(this.config);

            this.MainWindow = cw;
            this.MainWindow.ShowDialog();
            //});
        }

        //public PluginConfig LoadConfig()
        //{
        //    PluginConfig oldConfig = this.config;
        //    this.config = PluginConfig.LoadConfig(PluginConfig.CONFIG_PATH);
        //    return oldConfig;
        //}

        public void ApplyConfig(PluginConfig config)
        {
            if (this.ttsHandler == null)
                this.ttsHandler = new TtsHandler("", config.Port);

            this.ttsHandler.ApplyConfig(config);
        }

        public void FetchConfig(PluginConfig config)
        {
            if (this.ttsHandler == null)
                return;

            this.ttsHandler.FetchConfig(config);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.ApplyConfig(SerializationManager.Config.Value);
            //this.ApplyConfig(this.config);

            this.ttsHandler.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            //this.FetchConfig(this.config);
            //PluginConfig.SaveConfig(PluginConfig.CONFIG_PATH, this.config);
            this.FetchConfig(SerializationManager.Config.Value);
            SerializationManager.Config.Save();

            if (this.ttsHandler == null)
                return;

            _ = this.ttsHandler.Stop();
        }

        //[DllExport("AppMain")]
        public static void Main()
        {
            //Util.DebugContent = (string content) => { };
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
