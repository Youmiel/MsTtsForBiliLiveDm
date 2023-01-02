using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
                    Util.DebugContent(builder.ToString());
                }
            }
            Util.DebugContent("");
        }
        public static Dictionary<string, string> ParseQueryString(string query, Encoding encoding)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            if (query.Length > 0 && query[0] == '?')
            {
                query = query.Substring(1);
            }

            Dictionary<string, string> map = query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                        // divide parameters by "&"
                    .Select(param => param.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries))
                        // divide key and value by first "="
                    .GroupBy(t => t[0], t => t.Length > 1 ? HttpUtility.UrlDecode(t[1], encoding) : string.Empty)
                        // group by key
                    .ToDictionary(group => group.Key, group => string.Join(",", group));
                        // concat group members with ","

            return map;
        }
    }
}
