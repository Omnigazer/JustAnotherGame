using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components;
using Omniplatformer.Utility;
using ZeroFormatter;

namespace Omniplatformer
{
    [Serializable, ZeroFormattable]
    public struct Tile
    {
        [IgnoreFormat]
        public (short, short) Type { get; set; }
        // public (short, short) Type { get; set; }
        [Index(0)]
        public short middle_type => Type.Item1;
        [Index(1)]
        public short back_type => Type.Item2;
        [Index(2)]
        public int Row { get; set; }
        [Index(3)]
        public int Col { get; set; }

        public Tile(short middle_type, short back_type, int row, int col)
        {
            Type = (middle_type, back_type);
            Row = row;
            Col = col;
        }
    }
}
