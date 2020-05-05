using System;
using Microsoft.Xna.Framework;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Terrain
{
    class SolidPlatform : GameObject
    {
        public SolidPlatform(Vector2 coords, Vector2 halfsize) : this(coords, halfsize, Position.DefaultOrigin)
        {

        }

        public SolidPlatform(Vector2 coords, Vector2 halfsize, Vector2 origin, bool tile = false)
        {
            Components.Add(new PhysicsComponent(this, coords, halfsize, origin) { Solid = true, Friction = 0.1f, Tile = tile });
            var c = new RenderComponent(this);
            /*
            if (tile)
            {
                c.Texture = GameContent.Instance.testTile;
            }
            */
            Components.Add(c);
        }
    }
}
