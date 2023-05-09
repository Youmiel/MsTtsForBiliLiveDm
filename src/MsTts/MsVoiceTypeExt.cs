using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MsTtsForBiliLiveDm.MsTts
{
    public partial class MsVoiceType : IComparable<MsVoiceType>
    {
        public static readonly MsVoiceType XiaoxiaoNeural = new MsVoiceType("晓晓", "zh-CN-XiaoxiaoNeural");
        public static readonly MsVoiceType YunyangNeural = new MsVoiceType("云扬","zh-CN-YunyangNeural");
        public static readonly MsVoiceType XiaochenNeural = new MsVoiceType("晓辰","zh-CN-XiaochenNeural");
        public static readonly MsVoiceType XiaohanNeural = new MsVoiceType("晓涵","zh-CN-XiaohanNeural");
        public static readonly MsVoiceType XiaomoNeural = new MsVoiceType("晓墨","zh-CN-XiaomoNeural");
        public static readonly MsVoiceType XiaoqiuNeural = new MsVoiceType("晓秋","zh-CN-XiaoqiuNeural");
        public static readonly MsVoiceType XiaoruiNeural = new MsVoiceType("晓睿","zh-CN-XiaoruiNeural");
        public static readonly MsVoiceType XiaoshuangNeural = new MsVoiceType("晓双","zh-CN-XiaoshuangNeural");
        public static readonly MsVoiceType XiaoxuanNeural = new MsVoiceType("晓萱","zh-CN-XiaoxuanNeural");
        public static readonly MsVoiceType XiaoyanNeural = new MsVoiceType("晓颜","zh-CN-XiaoyanNeural");
        public static readonly MsVoiceType XiaoyouNeural = new MsVoiceType("晓悠","zh-CN-XiaoyouNeural");
        public static readonly MsVoiceType YunxiNeural = new MsVoiceType("云希","zh-CN-YunxiNeural");
        public static readonly MsVoiceType YunyeNeural = new MsVoiceType("云野","zh-CN-YunyeNeural");
        public static readonly MsVoiceType XiaomengNeural = new MsVoiceType("晓梦","zh-CN-XiaomengNeural");
        public static readonly MsVoiceType XiaoyiNeural = new MsVoiceType("晓伊","zh-CN-XiaoyiNeural");
        public static readonly MsVoiceType XiaozhenNeural = new MsVoiceType("晓甄","zh-CN-XiaozhenNeural");
        public static readonly MsVoiceType YunfengNeural = new MsVoiceType("云枫","zh-CN-YunfengNeural");
        public static readonly MsVoiceType YunhaoNeural = new MsVoiceType("云皓","zh-CN-YunhaoNeural");
        public static readonly MsVoiceType YunjianNeural = new MsVoiceType("云健","zh-CN-YunjianNeural");
        public static readonly MsVoiceType YunxiaNeural = new MsVoiceType("云夏","zh-CN-YunxiaNeural");
        public static readonly MsVoiceType YunzeNeural = new MsVoiceType("云泽","zh-CN-YunzeNeural");

        public static readonly SortedSet<MsVoiceType> ALL_VOICE = MsVoiceType.VoiceTypes();
        private static SortedSet<MsVoiceType> VoiceTypes()
        {
            SortedSet<MsVoiceType> voiceSet = new SortedSet<MsVoiceType>
            {
                MsVoiceType.XiaoxiaoNeural,
                MsVoiceType.YunyangNeural,
                MsVoiceType.XiaochenNeural,
                MsVoiceType.XiaohanNeural,
                MsVoiceType.XiaomoNeural,
                MsVoiceType.XiaoqiuNeural,
                MsVoiceType.XiaoruiNeural,
                MsVoiceType.XiaoshuangNeural,
                MsVoiceType.XiaoxuanNeural,
                MsVoiceType.XiaoyanNeural,
                MsVoiceType.XiaoyouNeural,
                MsVoiceType.YunxiNeural,
                MsVoiceType.YunyeNeural,
                MsVoiceType.XiaomengNeural,
                MsVoiceType.XiaoyiNeural,
                MsVoiceType.XiaozhenNeural,
                MsVoiceType.YunfengNeural,
                MsVoiceType.YunhaoNeural,
                MsVoiceType.YunjianNeural,
                MsVoiceType.YunxiaNeural,
                MsVoiceType.YunzeNeural
            };

            return voiceSet;
        }
    }
}
