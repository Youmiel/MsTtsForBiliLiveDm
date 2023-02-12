using MsTtsForBiliLiveDm.Plugin;
using MsTtsForBiliLiveDm.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MsTtsForBiliLiveDm.Plugin.Serialization
{
    public class SerializationResource<T> { 
        private static string RESOURCE_PATH = Path.Combine(Assembly.GetExecutingAssembly().Location, "MsTtsPlugin");

        private string path;
        private T value;

        public SerializationResource(string path)
        {
            this.path = path;
            LoadResource();
        }

        private void LoadResource()
        {
        }
    }
    public class SerializationManager
    {
        private static PluginConfig config;
        private static AccessRecord record;

        static PluginConfig Config => config;
        static AccessRecord Record => record;

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
