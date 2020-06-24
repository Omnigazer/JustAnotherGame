using System;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using System.Reactive.Linq;
using Omniplatformer.Components.Character;

namespace Omniplatformer.Objects.Projectiles
{
    class Boulder : Projectile
    {
        public static float InverseMass => 0.8f;

        public override void InitializeCustomComponents()
        {
            TTL = 50000;
            var movable = new ProjectileMoveComponent() { Solid = false, Hittable = false, InverseMass = InverseMass };
            RegisterComponent(movable);
            var c = new AnimatedRenderComponent(Color.White, "Textures/boulder");
            c.AddAnimation(new Animations.DeathAnimation(c));
            RegisterComponent(c);
            RegisterComponent(new DamageHitComponent(damage: 3));
            RegisterComponent(new AnimatedDestructibleComponent() { AnimationLength = 50 });
        }

        public static Boulder Create()
        {
            var boulder = new Boulder();
            boulder.InitializeComponents();
            var pos = (PositionComponent)boulder;
            pos.SetLocalHalfsize(new Vector2(6, 6));
            return boulder;
        }
    }
}
