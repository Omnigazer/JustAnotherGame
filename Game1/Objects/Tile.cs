using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components;
using Omniplatformer.Utility;

namespace Omniplatformer
{
    [Serializable]
    public struct Tile
    {
        public int Type { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
