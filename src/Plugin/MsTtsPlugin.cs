using MsTtsForBiliLiveDm.HttpHandler;
using MsTtsForBiliLiveDm.MsTts;
using MsTtsForBiliLiveDm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MsTtsForBiliLiveDm.Plugin
{
    public class MsTtsPlugin : BilibiliDM_PluginFramework.DMPlugin
    {
        private static readonly bool DISABLE = false;

        private PluginConfig config = null;
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
            this.PluginVer = "1.0.0";
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
            this.LoadConfig();
        }

        public PluginConfig LoadConfig()
        {
            PluginConfig oldConfig = this.config;
            this.config = PluginConfig.LoadConfig(PluginConfig.CONFIG_PATH);

            //if (this.configWindow != null)
            //    this.configWindow.BindConfig(this.config);
            return oldConfig;
        }

        private void ApplyConfig(PluginConfig config)
        {
            Util.LogContent("Port: " + config.Port);
            Util.LogContent("Type: " + config.VoiceType);

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

        public override void Admin()
        {
            CheckDisable();

            base.Admin();

            if (this.configWindow == null || this.configWindow.Visibility == Visibility.Hidden)
            {
                Util.RunInSTAThread((ThreadStart)delegate
                {
                    ConfigWindow cw = new ConfigWindow();
                    cw.CloseBehaviour = CloseBehaviourEnum.CLOSE;
                    cw.ConfigApplyAsync = (cfg) => this.ApplyConfig(cfg);
                    this.configWindow = cw;
                    this.configWindow.BindConfig(this.config);

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
            CheckDisable();

            if (this.ttsHandler == null || !this.ttsHandler.IsRunning)
                return;

            base.Stop();
            //請勿使用任何阻塞方法
            _ = this.ttsHandler.Stop();
            this.Log("Plugin stopped!");
        }

        public override void Start()
        {
            CheckDisable();
                
            base.Start();
            //請勿使用任何阻塞方法

            this.LoadConfig();
            Task.Run(() =>
            {
                this.ApplyConfig(this.config);
                this.ttsHandler.Start();
                this.Log("Plugin started!");
            });

        }
    }
}
