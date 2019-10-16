using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    public class Row : ViewControl
    {
        public Row()
        {
            Node.FlexDirection = Facebook.Yoga.YogaFlexDirection.Row;
        }
    }
}
