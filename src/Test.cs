using MsTtsForBiliDanmaku.HttpHandler;
using MsTtsForBiliLiveDm.HttpHandler;
using MsTtsForBiliLiveDm.MsTts;
using MsTtsForBiliLiveDm.Plugin;
using MsTtsForBiliLiveDm.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MsTtsForBiliLiveDm
{
    public class Test
    {
        //public static void Main()
        //{
        //    Util.DebugContent = (string content) => Console.Out.WriteLine(content);
        //    Util.LogContent = (string content) => Console.Out.WriteLine(content);

        //    //TtsHandler ttsHandler = new TtsHandler("", 35468);
        //    //ttsHandler.Start();

        //    TtsHandler ttsHandler = null;

        //    Thread thread = new Thread((ThreadStart)delegate
        //    {
        //        ConfigWindow configWindow = new ConfigWindow();
        //        PluginConfig config = PluginConfig.LoadConfig(PluginConfig.CONFIG_PATH);
        //        configWindow.BindConfig(config);

        //        ttsHandler = new TtsHandler("", config.Port);
        //        ttsHandler.TTSGetter.VoiceType = config.VoiceType;

        //        ttsHandler.Start();
        //        Util.LogContent("Plugin started!");

        //        while (true)
        //        {
        //            configWindow.ShowDialog();
        //            Util.LogContent("Port: " + config.Port);
        //            Util.LogContent("Type: " + config.VoiceType);
        //            ttsHandler.TTSGetter.VoiceType = config.VoiceType;
        //        }
        //    });
        //    thread.SetApartmentState(ApartmentState.STA);
        //    thread.Start();

        //    //Application.Current.Dispatcher.Invoke(() =>
        //    //{

        //    //});

        //    Console.ReadLine();
        //    if (ttsHandler != null) ttsHandler.StopAndWait();
        //}
    }
}
