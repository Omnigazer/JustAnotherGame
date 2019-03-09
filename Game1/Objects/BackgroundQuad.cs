using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components;
using Omniplatformer.Utility;

namespace Omniplatformer
{
    class BackgroundQuad : GameObject
    {
        public BackgroundQuad(Vector2 center, Vector2 halfsize, Vector2 origin, bool tiled = false)
        {
            Tickable = false;
            Components.Add(new PhysicsComponent(this, center, halfsize, origin) { Solid = false, Tile = tiled });
            Components.Add(new RenderComponent(this, Color.Gray) { Tile = tiled });
        }

        public override object AsJson()
        {
            return new { type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }

        public static GameObject FromJson(Deserializer deserializer)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(deserializer.getData());
            return new BackgroundQuad(coords, halfsize, origin, true);
        }
    }
}
