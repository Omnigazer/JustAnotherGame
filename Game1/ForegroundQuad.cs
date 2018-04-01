using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;

namespace Omniplatformer
{
    class ForegroundQuad : GameObject
    {
        public ForegroundQuad(Vector2 center, Vector2 halfsize)
        {
            Solid = false;
            Components.Add(new PositionComponent(this, center, halfsize));
            Components.Add(new ForegroundRenderComponent(this, Color.Green));
        }
    }
}
