using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;

namespace Omniplatformer
{
    public class Liquid : GameObject
    {
        public Liquid(Vector2 center, Vector2 halfsize)
        {
            Solid = false;
            Liquid = true;
            Components.Add(new PositionComponent(this, center, halfsize));
            // TODO: Add Aqua Color to this renderer
            Components.Add(new RenderComponent(this, Color.Aqua * 0.5f, Layers.Liquid));
        }        
    }
}
