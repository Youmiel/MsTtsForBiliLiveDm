using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MsTtsForBiliLiveDm.Utils
{
    public class Util
    {
        public delegate void LogMethod(string content);

        public static LogMethod LogContent { get; set; }
    }
}
