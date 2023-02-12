using MsTtsForBiliLiveDm.Plugin;
using MsTtsForBiliLiveDm.Plugin.Serialization;
using MsTtsForBiliLiveDm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsTtsForBiliLiveDm.MsTts
{
    public class MsAPIProvider: IRecord
    {
        private static readonly Random RANDOM = new Random();
        private static readonly string API_PREFIX = "wss://";
        private static readonly string API_POSTFIX = ".api.speech.microsoft.com/cognitiveservices/websocket/v1";
        private static readonly string[] LOCATION_LIST = new string[]
        {
             "westus",
             "westus2",
             "southcentralus",
             "westcentralus",
             "eastus",
             "eastus2",
             "westeurope",
             "northeurope",
             "southbrazil",
             "eastaustralia",
             "southeastasia",
             "eastasia"
        };
        private static readonly int MAX_COUNT = 50;
        private static readonly int FAIL_PENALTY = 10;
        private static readonly TimeSpan REFREASH_HOUR = TimeSpan.FromHours(16.0d);
        public static readonly Dictionary<string, int> DEFAULT_COUNT_DOWN_MAP = ConstructDefaultCountDownMap();

        private DateTime accessTime;
        private Dictionary<string, int> accessCountDown;
        private string lastAccessKey = null;

        private static Dictionary<string, int> ConstructDefaultCountDownMap()
        {
            var map = new Dictionary<string, int>();
            foreach (string name in LOCATION_LIST)
                map.Add(name, MAX_COUNT);

            return map;
        }

        public MsAPIProvider()
        {
            this.accessTime = DateTime.Now;
            this.accessCountDown = new Dictionary<string, int>(DEFAULT_COUNT_DOWN_MAP);
        }

        private string getLocationName()
        {
            int counter = 1;
            string location = null;
            foreach(var entry in this.accessCountDown)
            {
                if (entry.Value <= 0 || RANDOM.Next(counter++) != 0) continue;
                location = entry.Key;
            }

            if (location != null) this.accessCountDown[location] -= 1;

            return location;
        }

        public string ProvideUrl()
        {
            string name = this.getLocationName();
            this.lastAccessKey = name;

            //Util.DebugContent(Util.DictToString<string, int>(this.accessCountDown));
            Util.DebugContent($"Using {name}");

            if (name == null)
                return null;
            return API_PREFIX + this.getLocationName() + API_POSTFIX;
        }

        public void ReduceLastAccess()
        {
            if (lastAccessKey != null)
                this.accessCountDown[this.lastAccessKey] -= FAIL_PENALTY;
        }

        public void SetRecord(AccessRecord record)
        {
            if (this.accessTime.Subtract(record.MsAccessTime).CompareTo(REFREASH_HOUR) < 0)
            {
                this.accessTime = record.MsAccessTime;
                foreach (var entry in record.MsAccessCountDown)
                {
                    int count = MAX_COUNT;
                    this.accessCountDown.TryGetValue(entry.Key, out count);
                    count = Math.Min(entry.Value, count);
                    this.accessCountDown[entry.Key] = count;
                }
            }
        }

        public void GetRecord(AccessRecord record)
        {
            record.MsAccessTime = this.accessTime;
            record.MsAccessCountDown.Clear();
            foreach (var entry in this.accessCountDown)
                record.MsAccessCountDown.Add(Util.ToSerializablePair(entry));
        }
    }
}