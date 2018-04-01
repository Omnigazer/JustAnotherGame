using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;

namespace Omniplatformer
{
    class SolidPlatform : GameObject
    {
        public SolidPlatform(Vector2 center, Vector2 halfsize)
        {
            Components.Add(new PositionComponent(this, center, halfsize));
            Components.Add(new RenderComponent(this));            
        }
    }
}
