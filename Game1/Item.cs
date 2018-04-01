using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Omniplatformer
{
    public class Collectible : GameObject
    {
        public Collectible(Vector2 center, Vector2 halfsize)
        {
            Pickupable = true;
            Solid = false;
            Components.Add(new PositionComponent(this, center, halfsize));
            // TODO: Add Aqua Color to this renderer
            Components.Add(new RenderComponent(this, Color.Green));
        }        
        public Bonus Bonus { get; set; }
    }
}
