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
    class ForegroundQuad : GameObject
    {
        public ForegroundQuad(Vector2 center, Vector2 halfsize, Vector2 origin)
        {
            Components.Add(new PhysicsComponent(this, center, halfsize, origin));
            Components.Add(new ForegroundRenderComponent(this, Color.Green));
        }

        public override object AsJson()
        {
            return new { type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }

        public static GameObject FromJson(JObject data)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(data);
            return new ForegroundQuad(coords, halfsize, origin);
        }
    }
}
