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
    public class SerializationResource<T> where T : new()
    {
        private static string RESOURCE_DIR = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MsTtsPlugin");

        static SerializationResource()
        {
            if (!File.Exists(RESOURCE_DIR))
                Directory.CreateDirectory(RESOURCE_DIR);
        }

        private static void SaveResource(string fullpath, T value)
        {

            XmlSerializer writer = new XmlSerializer(typeof(T));
            FileStream file = File.Create(fullpath);
            writer.Serialize(file, value);
            file.Close();
        }
        private static T CreateDefaultConfig(string fullpath)
        {
            T newObj = new T();
            SaveResource(fullpath, newObj);
            return newObj;
        }

        private static T LoadResource(string fullpath)
        {
            StreamReader file = null;
            try
            {
                Util.DebugContent($"Loading resource from {Path.GetFullPath(fullpath)}");
                if (File.Exists(fullpath))
                {
                    XmlSerializer reader = new XmlSerializer(typeof(T));
                    file = new StreamReader(fullpath);
                    T value = (T)reader.Deserialize(file);
                    file.Close();
                    return value;
                }
                else
                {
                    Util.LogContent("Config not found. Creating new one...");
                    return CreateDefaultConfig(fullpath);
                }
            }
            catch (InvalidOperationException)
            {
                if (file != null)
                    file.Close();
                Util.LogContent("Config load error. Creating new one...");
                return CreateDefaultConfig(fullpath);
            }
        }

        private string path;
        private T value;

        public T Value => value;

        public SerializationResource(string path)
        {
            this.path = Path.Combine(RESOURCE_DIR, path);
            this.value = LoadResource(this.path);
        }

        public void Reload()
        {
            this.value = LoadResource(this.path);
        }

        public void Save()
        {
            SaveResource(this.path, this.value);
        }

        public async void SaveAsync()
        {
            await Task.Run(delegate { this.Save(); });
        }
    }
    public class SerializationManager
    {
        private static SerializationResource<PluginConfig> config = new SerializationResource<PluginConfig>("config.xml");
        private static SerializationResource<AccessRecord> record = new SerializationResource<AccessRecord>("record.xml");

        public static SerializationResource<PluginConfig> Config => config;
        public static SerializationResource<AccessRecord> Record => record;
    }
}
