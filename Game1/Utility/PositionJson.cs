using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Utility
{
    class PositionJson
    {
        public static object ToJson(GameObject obj)
        {
            var pos = obj.GetComponent<PositionComponent>();
            return new
            {
                type = obj.GetType().AssemblyQualifiedName,
                coords = new
                {
                    x = pos.WorldPosition.Coords.X,
                    y = pos.WorldPosition.Coords.Y,
                },
                halfsize = new
                {
                    x = pos.WorldPosition.halfsize.X,
                    y = pos.WorldPosition.halfsize.Y
                },
                origin = new
                {
                    x = pos.WorldPosition.Origin.X,
                    y = pos.WorldPosition.Origin.Y
                }
            };
        }

        public static (Vector2 coords, Vector2 halfsize, Vector2 origin) FromJson(JObject data)
        {
            var coords = new Vector2(float.Parse((string)data["coords"]["x"]), float.Parse((string)data["coords"]["y"]));
            var halfsize = new Vector2(float.Parse((string)data["halfsize"]["x"]), float.Parse((string)data["halfsize"]["y"]));

            Vector2 origin = Position.DefaultOrigin;
            if (data["origin"]?.Type == JTokenType.Object)
            {
                origin = new Vector2(float.Parse((string)data["origin"]["x"]), float.Parse((string)data["origin"]["y"]));
                // return new SolidPlatform(coords, halfsize, origin);
            }
            return (coords, halfsize, origin);
        }
    }
}
