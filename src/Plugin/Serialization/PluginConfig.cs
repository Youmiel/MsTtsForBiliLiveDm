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
        public static readonly string CONFIG_PATH = "config.xml";
        public static readonly PluginConfig DEFAULT = new PluginConfig();

        //string contextRoot;
        private int port;
        private MsVoiceType voiceType; 
        private int rate;
        private int pitch;
        private DateTime accessTime;
        private List<SerializablePair<string, int>> accessCountDown;

        public int Port { get => this.port; set => this.port = Util.Clamp(value, 0, 65535); }
        public MsVoiceType VoiceType { get => this.voiceType; set => this.voiceType = value; }
        public int Rate { get => this.rate; set => this.rate = Util.Clamp(value, -100, 200); }
        public int Pitch { get => this.pitch; set => this.pitch = Util.Clamp(value, -50, 50); }
        public DateTime AccessTime { get => accessTime; set => accessTime = value; }
        public List<SerializablePair<string, int>> AccessCountDown { get => accessCountDown; }

        public PluginConfig()
        {
            this.port = 35468;
            this.voiceType = MsVoiceType.XiaoxiaoNeural;
            this.rate = 0;
            this.pitch = 0;
            this.accessTime = DateTime.Now;
            //this.accessCountDown = new Dictionary<string, int>();//?
            this.accessCountDown = new List<SerializablePair<string, int>>(); //?
            foreach (var entry in MsAPIProvider.DEFAULT_COUNT_DOWN_MAP)
                this.accessCountDown.Add(Util.ToSerializablePair(entry));
        }

        public void CopyValueOf(PluginConfig config)
        {
            this.port = config.Port;
            this.voiceType = config.VoiceType;
            this.rate = config.Rate;
            this.pitch = config.Pitch;
            this.accessTime = config.AccessTime;
            this.accessCountDown.Clear();
            foreach (var entry in config.AccessCountDown)
            {
                this.accessCountDown.Add(entry.Clone());
            }
        }

        public void Save()
        {
            PluginConfig.SaveConfig(CONFIG_PATH, this);
        }
        public async void SaveAsync()
        {
            await Task.Run(delegate { this.Save(); });
        }

        public static PluginConfig CreateDefaultConfig()
        {
            PluginConfig newConfig = new PluginConfig();
            newConfig.CopyValueOf(DEFAULT);
            newConfig.Save();
            return newConfig;
        }

        public static PluginConfig LoadConfig(string path)
        {
            StreamReader file = null;
            try
            {
                Util.DebugContent($"Loading config from {Path.GetFullPath(path)}");
                if (File.Exists(path))
                {
                    XmlSerializer reader = new XmlSerializer(typeof(PluginConfig));
                    file = new StreamReader(path);
                    PluginConfig config = (PluginConfig)reader.Deserialize(file);
                    file.Close();
                    return config;
                }
                else
                {
                    Util.LogContent("Config not found. Creating new one...");
                    return CreateDefaultConfig();
                }
            }
            catch (InvalidOperationException)
            {
                if (file != null)
                    file.Close();
                Util.LogContent("Config load error. Creating new one...");
                return CreateDefaultConfig();
            }
        }

        public static void SaveConfig(string path, PluginConfig config)
        {
            XmlSerializer writer = new XmlSerializer(typeof(PluginConfig));
            FileStream file = File.Create(path);
            writer.Serialize(file, config);
            file.Close();
        }
    }
}
