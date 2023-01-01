using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MsTtsForBiliLiveDm.Utils
{
    internal class HttpUtil
    {
        public static void LogRequestContent(HttpListenerRequest request)
        {
            Type cls = request.GetType();
            PropertyInfo[] pInfos = cls.GetProperties();

            foreach (PropertyInfo info in pInfos)
            {
                StringBuilder builder = new StringBuilder(info.Name);
                builder.Append(": ");
                try
                {
                    builder.Append(info.GetValue(request) == null ? "null" : info.GetValue(request).ToString());
                }
                catch (Exception e)
                {
                    builder.Append("--").Append(e.Message);
                }
                finally
                {
                    Util.LogContent(builder.ToString());
                }
            }
            Util.LogContent("");
        }
    }
}
