using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsTtsForBiliLiveDm.MsTts
{
    public class MsVoiceType
    {

        public static readonly string XiaoxiaoNeural = "zh-CN-XiaoxiaoNeural";
        public static readonly string YunyangNeural = "zh-CN-YunyangNeural";
        public static readonly string XiaochenNeural = "zh-CN-XiaochenNeural";
        public static readonly string XiaohanNeural = "zh-CN-XiaohanNeural";
        public static readonly string XiaomoNeural = "zh-CN-XiaomoNeural";
        public static readonly string XiaoqiuNeural = "zh-CN-XiaoqiuNeural";
        public static readonly string XiaoruiNeural = "zh-CN-XiaoruiNeural";
        public static readonly string XiaoshuangNeural = "zh-CN-XiaoshuangNeural";
        public static readonly string XiaoxuanNeural = "zh-CN-XiaoxuanNeural";
        public static readonly string XiaoyanNeural = "zh-CN-XiaoyanNeural";
        public static readonly string XiaoyouNeural = "zh-CN-XiaoyouNeural";
        public static readonly string YunxiNeural = "zh-CN-YunxiNeural";
        public static readonly string YunyeNeural = "zh-CN-YunyeNeural";
        public static readonly string XiaomengNeural = "zh-CN-XiaomengNeural";
        public static readonly string XiaoyiNeural = "zh-CN-XiaoyiNeural";
        public static readonly string XiaozhenNeural = "zh-CN-XiaozhenNeural";
        public static readonly string YunfengNeural = "zh-CN-YunfengNeural";
        public static readonly string YunhaoNeural = "zh-CN-YunhaoNeural";
        public static readonly string YunjianNeural = "zh-CN-YunjianNeural";
        public static readonly string YunxiaNeural = "zh-CN-YunxiaNeural";
        public static readonly string YunzeNeural = "zh-CN-YunzeNeural";

        public static readonly List<string> ALL_VOICE = MsVoiceType.VoiceTypes();

        private string displayName;
        private string value;

        private static List<string> VoiceTypes()
        {
            List<string> voiceList = new List<string>();
            voiceList.Add(MsVoiceType.XiaoxiaoNeural);
            voiceList.Add(MsVoiceType.YunyangNeural);
            voiceList.Add(MsVoiceType.XiaochenNeural);
            voiceList.Add(MsVoiceType.XiaohanNeural);
            voiceList.Add(MsVoiceType.XiaomoNeural);
            voiceList.Add(MsVoiceType.XiaoqiuNeural);
            voiceList.Add(MsVoiceType.XiaoruiNeural);
            voiceList.Add(MsVoiceType.XiaoshuangNeural);
            voiceList.Add(MsVoiceType.XiaoxuanNeural);
            voiceList.Add(MsVoiceType.XiaoyanNeural);
            voiceList.Add(MsVoiceType.XiaoyouNeural);
            voiceList.Add(MsVoiceType.YunxiNeural);
            voiceList.Add(MsVoiceType.YunyeNeural);
            voiceList.Add(MsVoiceType.XiaomengNeural);
            voiceList.Add(MsVoiceType.XiaoyiNeural);
            voiceList.Add(MsVoiceType.XiaozhenNeural);
            voiceList.Add(MsVoiceType.YunfengNeural);
            voiceList.Add(MsVoiceType.YunhaoNeural);
            voiceList.Add(MsVoiceType.YunjianNeural);
            voiceList.Add(MsVoiceType.YunxiaNeural);
            voiceList.Add(MsVoiceType.YunzeNeural);

            return voiceList;
        }
    }
}
