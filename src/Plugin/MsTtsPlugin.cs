using MsTtsForBiliLiveDm.HttpHandler;
using MsTtsForBiliLiveDm.Plugin.Serialization;
using MsTtsForBiliLiveDm.Utils;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MsTtsForBiliLiveDm.Plugin
{
    public class MsTtsPlugin : BilibiliDM_PluginFramework.DMPlugin, IConfigurable
    {
        private static readonly bool DISABLE = false;

        private static readonly string REPOSITORY_LINK = "https://github.com/Youmiel/MsTtsForBiliLiveDm";

        //private PluginConfig config = null;
        private ConfigWindow configWindow = null;
        private TtsHandler ttsHandler = null;

        private static void CheckDisable()
        {
            if (DISABLE)
            {
                MessageBox.Show("插件还在开发中");
                return;
            }
        }

        public MsTtsPlugin()
        {
            Util.LogContent = (string content) => { this.Log(content); };
            if (this.DebugMode)
                Util.DebugContent = (string content) => { this.Log(content); };
            else
                Util.DebugContent = (string content) => { };

            //this.Connected += MsTtsPlugin_Connected;
            //this.Disconnected += MsTtsPlugin_Disconnected;
            //this.ReceivedDanmaku += MsTtsPlugin_ReceivedDanmaku;
            //this.ReceivedRoomCount += MsTtsPlugin_ReceivedRoomCount;
            this.PluginAuth = "Youmiel";
            this.PluginName = "MsTtsPlugin";
            this.PluginCont = "youmiel@qq.com";
            this.PluginVer = "1.2.1";
            this.PluginDesc = "微软TTS引擎";

            if (DISABLE) return;

            //Util.RunInSTAThread((ThreadStart)delegate
            //{
            //    ConfigWindow cw = new ConfigWindow();
            //    this.configWindow = cw;
            //    this.LoadConfig();
            //    Util.LogContent("window inited!");
            //    cw.ShowDialog();
            //});
            this.Log($"你正在使用的是 {this.PluginVer} 版本的 {this.PluginName}, 项目仓库: {REPOSITORY_LINK}. 如果你遇到了任何使用问题, 欢迎前往反馈.");

            //this.LoadConfig();
        }

        //public PluginConfig LoadConfig()
        //{
        //    PluginConfig oldConfig = this.config;
        //    this.config = PluginConfig.LoadConfig(PluginConfig.CONFIG_PATH);

        //    //if (this.configWindow != null)
        //    //    this.configWindow.BindConfig(this.config);
        //    return oldConfig;
        //}

        public void ApplyConfig(PluginConfig config)
        {
            Util.DebugContent("Port: " + config.Port);
            Util.DebugContent("VoiceType: " + config.VoiceType);
            Util.DebugContent("Rate: " + config.Rate);
            Util.DebugContent("Pitch: " + config.Pitch);

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

        public override void Admin()
        {
            if (DISABLE)
            {
                MessageBox.Show("插件还在开发中");
                return;
            }
            // CheckDisable();

            base.Admin();

            if (this.configWindow == null || this.configWindow.Visibility == Visibility.Hidden)
            {
                Util.RunInSTAThread((ThreadStart)delegate
                {
                    ConfigWindow cw = new ConfigWindow();
                    cw.CloseBehaviour = CloseBehaviourEnum.CLOSE;
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

                    cw.ShowDialog();

                    this.configWindow = null;
                });
            }
            else
            {
                this.configWindow.Dispatcher.Invoke(delegate { this.configWindow.Activate(); });
            }

            Util.DebugContent("Open admin menu.");
        }

        public override void Stop()
        {
            if (DISABLE)
            {
                MessageBox.Show("插件还在开发中");
                return;
            }
            // CheckDisable();

            this.FetchConfig(SerializationManager.Config.Value);
            SerializationManager.Config.Save();
            //PluginConfig.SaveConfig(PluginConfig.CONFIG_PATH, this.config);

            if (this.ttsHandler == null || !this.ttsHandler.IsRunning)
                return;

            base.Stop();
            //請勿使用任何阻塞方法
            _ = this.ttsHandler.Stop();
            this.Log("Plugin stopped!");
        }

        public override void Start()
        {
            if (DISABLE)
            {
                MessageBox.Show("插件还在开发中");
                return;
            }
            // CheckDisable();
                
            base.Start();
            //請勿使用任何阻塞方法

            //this.LoadConfig();
            SerializationManager.Config.Reload();
            Task.Run(() =>
            {
                //this.ApplyConfig(this.config);
                this.ApplyConfig(SerializationManager.Config.Value);
                this.ttsHandler.Start();
                this.Log("Plugin started!");
            });

        }

        public override void DeInit()
        {
            if (DISABLE)
            {
                MessageBox.Show("插件还在开发中");
                return;
            }
            // CheckDisable();

            //this.FetchConfig(this.config);
            //PluginConfig.SaveConfig(PluginConfig.CONFIG_PATH, this.config);
            this.FetchConfig(SerializationManager.Config.Value);
            SerializationManager.Config.Save();

            if (this.ttsHandler == null)
                return;

            base.DeInit();
            this.ttsHandler.Destory();
        }
    }
}
