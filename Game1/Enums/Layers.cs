﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    static class Layers
    {
        public static int Background => -1;
        public static int Default => 0;
        public static int Character => 1;
        public static int Liquid => 2;
        // will introduce this if we actually need it, for now there's a separate drawing scene for this
        // public static int Foreground => 3;
    }
}
