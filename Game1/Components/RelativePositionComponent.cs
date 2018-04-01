using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public class RelativePositionComponent : PositionComponent
    {        
        public RelativePositionComponent(GameObject obj, Vector2 center, Vector2 halfsize) : base(obj, center, halfsize)
        {
        }

        
    }
}
