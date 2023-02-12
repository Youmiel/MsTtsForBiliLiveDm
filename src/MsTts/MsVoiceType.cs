using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MsTtsForBiliLiveDm.MsTts
{
    [XmlType("MsVoiceType"), XmlRoot("MsVoiceType")]
    public partial class MsVoiceType: IComparable<MsVoiceType>
    {
        //private static int ID = 0;

        //private int id;
        private string displayName;
        private string internalName;

        public string DisplayName { get => displayName; set => displayName = value; }
        public string InternalName { get => internalName; set => internalName = value; }

        public MsVoiceType() : this("", "") { }

        public MsVoiceType(string displayName, string internalName)
        {
            //this.id = ID++;
            this.displayName = displayName;
            this.internalName = internalName;
        }

        public override string ToString()
        {
            return this.displayName;
        }

        public override bool Equals(object obj)
        {
            return obj is MsVoiceType msObj && this.internalName.Equals(msObj.InternalName);
        }

        public override int GetHashCode()
        {
            return this.internalName.GetHashCode();
        }

        public int CompareTo(MsVoiceType other)
        {
            return this.internalName.CompareTo(other.InternalName);
            //if (this.Equals(other))
            //    return 0;
            //return this.id.CompareTo(other.id);
        }
    }
}
