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

        public Liquid(Vector2 coords, Vector2 halfsize) : this(coords, halfsize, Position.DefaultOrigin)
        {

        }

        public Liquid(Vector2 coords, Vector2 halfsize, Vector2 origin)
        {
            Solid = false;
            Liquid = true;
            Components.Add(new RenderComponent(this, Color.Aqua * 0.5f, Layers.Liquid));
            Components.Add(new PositionComponent(this, coords, halfsize, 0, origin));
        }
    }
}
