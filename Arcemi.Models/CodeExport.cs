using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Models
{
    public static class CodeExport
    {

        public static JObject FromCode(string code)
        {
            var bytes = Convert.FromBase64String(code.Trim().Trim('\"'));
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return JObject.Parse(json);
        }

        public static string ToCode(JToken token)
        {
            var json = token.ToString(Formatting.None);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }
    }
}
