using System.Collections.Generic;
using Omniplatformer.Utility.DataStructs;
using ZeroFormatter;

namespace Omniplatformer.Utility
{
    [ZeroFormattable]
    public struct GridContainer
    {
        [Index(0)]
        public List<Tile> List { get; set; }

        public GridContainer(List<Tile> list)
        {
            List = list;
        }
    }
}
