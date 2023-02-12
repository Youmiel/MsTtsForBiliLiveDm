using MsTtsForBiliLiveDm.MsTts;
using MsTtsForBiliLiveDm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsTtsForBiliLiveDm.Plugin.Serialization
{
    public class AccessRecord
    {
        public DateTime MsAccessTime { get; set; }
        public List<SerializablePair<string, int>> MsAccessCountDown { get; set; }

        public AccessRecord()
        {
            this.MsAccessTime = DateTime.Now;
            this.MsAccessCountDown = new List<SerializablePair<string, int>>(); //?
            foreach (var entry in MsAPIProvider.DEFAULT_COUNT_DOWN_MAP)
                this.MsAccessCountDown.Add(Util.ToSerializablePair(entry));
        }
    }
}
