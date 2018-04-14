using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    class Ladder : GameObject
    {
        public Ladder(Vector2 center, Vector2 halfsize)
        {
            Solid = false; Climbable = true;
            Components.Add(new PositionComponent(this, center, halfsize));
            // TODO: Add Purple Color to this renderer
            Components.Add(new RenderComponent(this, Color.Purple, GameContent.Instance.ladder));
        }

        public Ladder(Vector2 center, Vector2 halfsize, GameObject parent)
        {
            Solid = false; Climbable = true;
            Components.Add(new PositionComponent(this, center, halfsize) { parent_pos = (PositionComponent)parent });
            // TODO: Add Purple Color to this renderer
            Components.Add(new RenderComponent(this, Color.Purple, GameContent.Instance.ladder));
        }
    }
}
