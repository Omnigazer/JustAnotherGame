﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    static class CustomDeconstructs
    {
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }

        public static void Deconstruct(this Vector2 tuple, out float key, out float value)
        {
            key = tuple.X;
            value = tuple.Y;
        }
    }
}
