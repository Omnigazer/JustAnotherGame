using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Terrain
{
    public class Liquid : GameObject
    {

        public Liquid(Vector2 coords, Vector2 halfsize) : this(coords, halfsize, Position.DefaultOrigin)
        {

        }

        public Liquid(Vector2 coords, Vector2 halfsize, Vector2 origin, bool tile = false)
        {
            Tickable = false;
            Components.Add(new PhysicsComponent(this, coords, halfsize, origin) { Liquid = true, Solid = false, Friction = 0.2f, Tile = tile });
            Components.Add(new RenderComponent(this, Color.Aqua * 0.5f, Layers.Liquid) { Tile = tile });
        }

        public override object AsJson()
        {
            return new { type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }

        public static GameObject FromJson(JObject data)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(data);
            return new Liquid(coords, halfsize, origin);
        }
    }
}
