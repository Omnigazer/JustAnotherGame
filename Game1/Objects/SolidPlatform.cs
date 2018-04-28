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
            return new {
                Id,
                type = GetType().AssemblyQualifiedName,
                Position = PositionJson.ToJson(this)
            };
        }

        /*
        public static GameObject FromJson(JObject data)
        {
            Guid id = Guid.Parse(data["Id"].ToString());
            SolidPlatform platform = (SolidPlatform)SerializeService.Instance.LocateObject(id);
            if (platform == null)
            {
                var (coords, halfsize, origin) = PositionJson.FromJson(data);
                platform = new SolidPlatform(coords, halfsize, origin);
                SerializeService.Instance.RegisterObject(platform);
            }
            return platform;
        }
        */

        public static GameObject FromJson(Deserializer deserializer)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(deserializer.getData());
            var platform = new SolidPlatform(coords, halfsize, origin);
            // SerializeService.Instance.RegisterObject(platform);
            return platform;
        }
    }
}
