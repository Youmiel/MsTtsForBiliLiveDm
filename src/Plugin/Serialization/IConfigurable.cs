using MsTtsForBiliLiveDm.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsTtsForBiliLiveDm.Plugin
{
    public interface IConfigurable
    {
        void ApplyConfig(PluginConfig config);
        void FetchConfig(PluginConfig config);
    }
}
