using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsTtsForBiliLiveDm.Plugin.Serialization
{
    public interface IRecord
    {
        void SetRecord(AccessRecord record);
        void GetRecord(AccessRecord record);
    }
}
