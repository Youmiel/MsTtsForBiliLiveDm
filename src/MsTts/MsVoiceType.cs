using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MsTtsForBiliLiveDm.MsTts
{
    [XmlType("MsVoiceType"), XmlRoot("MsVoiceType")]
    public class MsVoiceType: IComparable<MsVoiceType>
    {

        public static readonly MsVoiceType XiaoxiaoNeural = new MsVoiceType("Xiaoxiao","zh-CN-XiaoxiaoNeural");
        public static readonly MsVoiceType YunyangNeural = new MsVoiceType("Yunyang","zh-CN-YunyangNeural");
        public static readonly MsVoiceType XiaochenNeural = new MsVoiceType("Xiaochen","zh-CN-XiaochenNeural");

        public static readonly SortedSet<MsVoiceType> ALL_VOICE = MsVoiceType.VoiceTypes();

        private string displayName;
        private string internalName;

        public string DisplayName { get => displayName; set => displayName = value; }
        public string InternalName { get => internalName; set => internalName = value; }

        public MsVoiceType() : this("Xiaoxiao", "zh-CN-XiaoxiaoNeural") { }

        public MsVoiceType(string displayName, string internalName)
        {
            this.displayName = displayName;
            this.internalName = internalName;
        }

        private static SortedSet<MsVoiceType> VoiceTypes()
        {
            SortedSet<MsVoiceType> voiceSet = new SortedSet<MsVoiceType>
            {
                MsVoiceType.XiaoxiaoNeural,
                MsVoiceType.YunyangNeural,
                MsVoiceType.XiaochenNeural
            };

            return voiceSet;
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
        }
    }
}
