using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Utility.DataStructs;
using System.Reactive.Linq;

namespace Omniplatformer.Objects.Terrain
{
    class DestructibleObject : GameObject
    {
        public override void InitializeCustomComponents()
        {
            RegisterComponent(new PhysicsComponent() { Solid = true, Hittable = true });
            RegisterComponent(new WallRenderComponent(Color.Yellow));
            RegisterComponent(new AnimatedDestructibleComponent() { AnimationLength = 50 });
            var damageable = new HitPointComponent(10);
            RegisterComponent(damageable);
        }

        public static DestructibleObject Create(Vector2 coords, Vector2 halfsize)
        {
            var quad = new DestructibleObject();
            quad.InitializeComponents();
            var pos = quad.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            pos.SetLocalHalfsize(halfsize);
            return quad;
        }
    }
}
