using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace Omniplatformer.Objects
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
