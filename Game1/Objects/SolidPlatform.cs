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
    class SolidPlatform : GameObject
    {
        public SolidPlatform(Vector2 coords, Vector2 halfsize) : this(coords, halfsize, Position.DefaultOrigin)
        {

        }

        public SolidPlatform(Vector2 coords, Vector2 halfsize, Vector2 origin)
        {
            Components.Add(new PositionComponent(this, coords, halfsize, 0, origin));
            Components.Add(new RenderComponent(this));
        }

        public override object AsJson()
        {
            return PositionJson.ToJson(this);
        }

        public static GameObject FromJson(JObject data)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(data);
            return new SolidPlatform(coords, halfsize, origin);
        }
    }
}
