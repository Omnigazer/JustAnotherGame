using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Terrain
{
    class ForegroundQuad : GameObject
    {
        public override void InitializeCustomComponents()
        {
            RegisterComponent(new PhysicsComponent());
            RegisterComponent(new ForegroundRenderComponent(Color.Green));
        }

        public static ForegroundQuad Create(Vector2 coords, Vector2 halfsize)
        {
            var quad = new ForegroundQuad();
            quad.InitializeComponents();
            var pos = quad.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            pos.SetLocalHalfsize(halfsize);
            return quad;
        }
    }
}
