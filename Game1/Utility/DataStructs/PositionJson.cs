using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Physics;
using Omniplatformer.Objects;

namespace Omniplatformer.Utility.DataStructs
{
    class PositionJson
    {
        public static object ToJson(GameObject obj)
        {
            var pos = obj.GetComponent<PositionComponent>();
            return new
            {
                coords = new
                {
                    x = pos.WorldPosition.Coords.X,
                    y = pos.WorldPosition.Coords.Y,
                },
                halfsize = new
                {
                    x = pos.WorldPosition.Halfsize.X,
                    y = pos.WorldPosition.Halfsize.Y
                },
                origin = new
                {
                    x = pos.WorldPosition.Origin.X,
                    y = pos.WorldPosition.Origin.Y
                }
            };
        }

        public static (Vector2 coords, Vector2 halfsize, Vector2 origin) FromJson(JObject input)
        {
            JObject pos_data = (JObject)input["Position"];
            var coords = new Vector2(pos_data["coords"]["x"].Value<float>(), pos_data["coords"]["y"].Value<float>());
            var halfsize = new Vector2(pos_data["halfsize"]["x"].Value<float>(), pos_data["halfsize"]["y"].Value<float>());

            Vector2 origin = Position.DefaultOrigin;
            if (pos_data["origin"]?.Type == JTokenType.Object)
            {
                origin = new Vector2(pos_data["origin"]["x"].Value<float>(), pos_data["origin"]["y"].Value<float>());
            }
            return (coords, halfsize, origin);
        }

        /*
        public static (Vector2 coords, Vector2 halfsize, Vector2 origin) FromJson(Deserializer deserializer)
        {
            // JObject pos_data = (JObject)input["Position"];
            // deserializer.decodeObject()
            var coords = new Vector2(float.Parse((string)pos_data["coords"]["x"]), float.Parse((string)pos_data["coords"]["y"]));
            var halfsize = new Vector2(float.Parse((string)pos_data["halfsize"]["x"]), float.Parse((string)pos_data["halfsize"]["y"]));

            Vector2 origin = Position.DefaultOrigin;
            if (pos_data["origin"]?.Type == JTokenType.Object)
            {
                origin = new Vector2(float.Parse((string)pos_data["origin"]["x"]), float.Parse((string)pos_data["origin"]["y"]));
                // return new SolidPlatform(coords, halfsize, origin);
            }
            return (coords, halfsize, origin);
        }
        */
    }
}
