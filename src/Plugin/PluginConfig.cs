﻿using MsTtsForBiliLiveDm.MsTts;
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
        int port;
        string voiceType;

        public int Port { get => this.port; set => this.port = Math.Min(Math.Max(value, 0), 65535); }
        public string VoiceType {
            get => this.voiceType;
            set {
                if (MsVoiceType.ALL_VOICE.Contains(value))
                    this.voiceType = value;
            }
        }

        public PluginConfig()
        {
            this.port = 35468;
            this.voiceType = MsVoiceType.XiaoxiaoNeural;
        }

        public void CopyValueOf(PluginConfig config)
        {
            this.port = config.Port;
            this.voiceType = config.VoiceType;
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