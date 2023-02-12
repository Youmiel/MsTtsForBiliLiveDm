using MsTtsForBiliLiveDm.MsTts;
using MsTtsForBiliLiveDm.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MsTtsForBiliLiveDm.Plugin
{
    public class PluginConfig
    {
        public static readonly PluginConfig DEFAULT = new PluginConfig();

        //string contextRoot;
        private int port;
        private MsVoiceType voiceType; 
        private int rate;
        private int pitch;

        public int Port { get => this.port; set => this.port = Util.Clamp(value, 0, 65535); }
        public MsVoiceType VoiceType { get => this.voiceType; set => this.voiceType = value; }
        public int Rate { get => this.rate; set => this.rate = Util.Clamp(value, -100, 200); }
        public int Pitch { get => this.pitch; set => this.pitch = Util.Clamp(value, -50, 50); }

        public PluginConfig()
        {
            this.port = 35468;
            this.voiceType = MsVoiceType.XiaoxiaoNeural;
            this.rate = 0;
            this.pitch = 0;
        }

        public void CopyValueOf(PluginConfig config)
        {
            this.port = config.Port;
            this.voiceType = config.VoiceType;
            this.rate = config.Rate;
            this.pitch = config.Pitch;
        }
    }
}
