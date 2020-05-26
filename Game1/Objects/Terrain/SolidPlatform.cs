using System;
using Microsoft.Xna.Framework;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Terrain
{
    class SolidPlatform : GameObject
    {
        public override void InitializeCustomComponents()
        {
            RegisterComponent(new PhysicsComponent() { Solid = true, Friction = 0.1f });
            RegisterComponent(new RenderComponent());
        }

        public static SolidPlatform Create(Vector2 coords, Vector2 halfsize)
        {
            var quad = new SolidPlatform();
            quad.InitializeComponents();
            var pos = quad.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            pos.SetLocalHalfsize(halfsize);
            return quad;
        }
    }
}
