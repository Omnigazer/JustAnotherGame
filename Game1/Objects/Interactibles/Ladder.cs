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
        public Ladder()
        {

        }

        public override void InitializeCustomComponents()
        {
            RegisterComponent(new PhysicsComponent(Vector2.Zero, Vector2.Zero) { Climbable = true });
            // TODO: Add Purple Color to this renderer
            RegisterComponent(new RenderComponent(Color.Purple, "Textures/ladder", 0, true));
        }

        public static Ladder Create(Vector2 coords, Vector2 halfsize)
        {
            var ladder = new Ladder();
            ladder.InitializeCustomComponents();
            var pos = ladder.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            pos.SetLocalHalfsize(halfsize);
            return ladder;
        }
    }
}
