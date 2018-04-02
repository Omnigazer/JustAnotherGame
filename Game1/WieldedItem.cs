using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Omniplatformer
{
    class WieldedItem : GameObject
    {
        public WieldedItem(Character parent, Vector2 origin)
        {
            Solid = false;
            var size = new Vector2(5, 10);
            Components.Add(new PositionComponent(this, origin, size) { parent_pos = (PositionComponent)parent });
            Components.Add(new RenderComponent(this, Color.RoyalBlue));
        }
    }
}
