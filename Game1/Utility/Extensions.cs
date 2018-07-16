using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Utility
{
    public static class Extensions
    {
        // Numeric extensions
        public static int LimitToRange(
        this int value, int inclusiveMinimum, int inclusiveMaximum)
        {
            if (value < inclusiveMinimum) { return inclusiveMinimum; }
            if (value > inclusiveMaximum) { return inclusiveMaximum; }
            return value;
        }

        public static float LimitToRange(
        this float value, float inclusiveMinimum, float inclusiveMaximum)
        {
            if (value<inclusiveMinimum) { return inclusiveMinimum; }
            if (value > inclusiveMaximum) { return inclusiveMaximum; }
            return value;
        }

        // Dictionary extensions
        public static void SetOrAdd<T>(
        this Dictionary<string, T> dict, string key, T value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }
    }
}
