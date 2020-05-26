using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Terrain
{
    class MovingPlatform : GameObject
    {
        public override void InitializeCustomComponents()
        {
            RegisterComponent(new PlatformMoveComponent() { Solid = true, Friction = 0.08f, InverseMass = 0 });
            RegisterComponent(new RenderComponent());
        }

        public static MovingPlatform Create(Vector2 coords, Vector2 halfsize)
        {
            var quad = new MovingPlatform();
            quad.InitializeComponents();
            var pos = quad.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            pos.SetLocalHalfsize(halfsize);
            return quad;
        }
    }
}
