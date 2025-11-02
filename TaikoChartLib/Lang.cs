using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoChartLib
{
    public static class Lang
    {
        public const string Default = "";
        public const string En = "en";
        public const string Ja = "ja";
        public const string Es = "es";
        public const string Fr = "fr";
        public const string Zh = "zh";

        public static T GetValue<T>(Dictionary<string, T> dict, string lang, T defaultValue)
        {
            if (dict.TryGetValue(lang, out T? value))
            {
                return value ?? defaultValue;
            }

            return defaultValue;
        }
    }
}
