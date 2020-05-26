using Microsoft.Xna.Framework;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Terrain
{
    class BackgroundQuad : GameObject
    {
        public override void InitializeCustomComponents()
        {
            RegisterComponent(new PhysicsComponent() { Solid = false });
            RegisterComponent(new RenderComponent(Color.Gray));
        }

        public static BackgroundQuad Create(Vector2 coords, Vector2 halfsize)
        {
            var quad = new BackgroundQuad();
            quad.InitializeComponents();
            var pos = quad.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            pos.SetLocalHalfsize(halfsize);
            return quad;
        }
    }
}
