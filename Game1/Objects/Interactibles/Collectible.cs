using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Interactibles
{
    public class Collectible : GameObject
    {
        public Collectible(Vector2 center, Vector2 halfsize)
        {
            Components.Add(new PhysicsComponent(this, center, halfsize) { Pickupable = true });
            // TODO: Add Aqua Color to this renderer
            Components.Add(new RenderComponent(this, Color.Green));
        }
        public Bonus Bonus { get; set; }

        public override object AsJson()
        {
            return new { type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }

        public static GameObject FromJson(JObject data)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(data);
            return new Collectible(coords, halfsize);
        }
    }
}
