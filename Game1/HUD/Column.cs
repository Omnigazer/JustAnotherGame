using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    public class Column: ViewControl
    {
        public Column()
        {
            Node.FlexDirection = Facebook.Yoga.YogaFlexDirection.Column;
        }
    }
}
