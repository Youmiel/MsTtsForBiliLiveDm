using MsTtsForBiliDanmaku.HttpHandler;
using MsTtsForBiliLiveDm.HttpHandler;
using MsTtsForBiliLiveDm.MsTts;
using MsTtsForBiliLiveDm.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsTtsForBiliLiveDm
{
    public class Test
    {
        public static void Main()
        {
            Util.LogContent = (string content) => Console.Out.WriteLine(content);
            TtsHandler handler = new TtsHandler("", 35468);
            handler.Start();
            Console.In.ReadLine();
            handler.StopAndWait();
        }
    }
}
