using Microsoft.Xna.Framework;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Terrain
{
    class BackgroundQuad : GameObject
    {
        public BackgroundQuad(Vector2 center, Vector2 halfsize, Vector2 origin, bool tiled = false)
        {
            Components.Add(new PhysicsComponent(this, center, halfsize, origin) { Solid = false, Tile = tiled });
            Components.Add(new RenderComponent(this, Color.Gray));
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
