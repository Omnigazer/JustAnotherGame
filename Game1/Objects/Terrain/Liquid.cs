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
        public override void InitializeCustomComponents()
        {
            RegisterComponent(new PhysicsComponent() { Liquid = true, Solid = false, Friction = 0.2f });
            RegisterComponent(new RenderComponent(Color.Aqua * 0.5f, z_index: Layers.Liquid));
        }

        public static Liquid Create(Vector2 coords, Vector2 halfsize)
        {
            var quad = new Liquid();
            quad.InitializeComponents();
            var pos = quad.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            pos.SetLocalHalfsize(halfsize);
            return quad;
        }
    }
}
