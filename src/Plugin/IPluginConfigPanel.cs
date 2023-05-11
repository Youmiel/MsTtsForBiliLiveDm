using MsTtsForBiliLiveDm.MsTts;
using MsTtsForBiliLiveDm.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MsTtsForBiliLiveDm.Plugin
{
    internal interface IPluginConfigPanel
    {
        double RequiredPanelHeight { get; }
        void UpdateWithConfig(PluginConfig config);
        void ApplyToConfig(PluginConfig config);
    }
}
