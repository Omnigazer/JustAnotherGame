using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Utility
{
    public static class RandomGen
    {
        static Random rnd = new Random();
        public static int Next(int min, int max)
        {
            return rnd.Next(min, max);
        }

        public static float NextFloat(float min, float max)
        {
            return (float)(rnd.NextDouble() * (max - min) + min);
        }
    }
}
