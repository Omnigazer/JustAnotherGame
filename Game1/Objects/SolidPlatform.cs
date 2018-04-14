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
        public SolidPlatform(Vector2 center, Vector2 halfsize) : this(center, halfsize, Position.DefaultOrigin)
        {

        }

        public SolidPlatform(Vector2 center, Vector2 halfsize, Vector2 origin)
        {
            Components.Add(new PositionComponent(this, center, halfsize, 0, origin));
            Components.Add(new RenderComponent(this));
        }
    }
}
