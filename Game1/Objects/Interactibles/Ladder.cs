using Microsoft.Xna.Framework;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Interactibles
{
    class Ladder : GameObject
    {
        public Ladder(Vector2 center, Vector2 halfsize)
        {
            Components.Add(new PhysicsComponent(this, center, halfsize) { Climbable = true });
            // TODO: Add Purple Color to this renderer
            Components.Add(new RenderComponent(this, Color.Purple, GameContent.Instance.ladder, 0, true));
        }

        public override object AsJson()
        {
            return new
            {
                Id,
                type = GetType().AssemblyQualifiedName,
                Position = PositionJson.ToJson(this)
            };
        }

        public static GameObject FromJson(Deserializer deserializer)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(deserializer.getData());
            return new Ladder(coords, halfsize);
        }
    }
}
