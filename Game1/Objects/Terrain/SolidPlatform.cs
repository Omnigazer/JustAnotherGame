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
            if (tile)
            {
                c.Texture = GameContent.Instance.testTile;
            }
            Components.Add(c);
        }

        public override object AsJson()
        {
            return new {
                Id,
                type = GetType().AssemblyQualifiedName,
                Position = PositionJson.ToJson(this)
            };
        }

        public static GameObject FromJson(Deserializer deserializer)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(deserializer.getData());
            coords.X = (float)Math.Round(coords.X);
            coords.Y = (float)Math.Round(coords.Y);
            var platform = new SolidPlatform(coords, halfsize, origin, true);
            // SerializeService.Instance.RegisterObject(platform);
            return platform;
        }
    }
}
