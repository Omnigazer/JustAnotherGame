using System;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Projectiles
{
    public class LifeDrainProjectile : Projectile
    {
        public const int speed = 10;

        public LifeDrainProjectile(Vector2 center, Vector2 direction, GameObject source = null): base(source)
        {
            Team = source?.Team ?? Team.Friend;
        }

        public void InitComponents()
        {
            var proj_movable = new ProjectileMoveComponent(this, Vector2.Zero, new Vector2(20, 5)) { InverseMass = 0 };
            Components.Add(proj_movable);
            Components.Add(new GlowingRenderComponent(this) { GlowColor = Color.Purple });
            Components.Add(new LifeDrainHitComponent(this, damage: 3));
        }

        public void SetDirection(Vector2 direction)
        {
            var proj_movable = GetComponent<ProjectileMoveComponent>();

            direction.Normalize();
            proj_movable.Rotate(-(float)Math.Atan2(direction.Y, direction.X));
            proj_movable.ApplyImpulse(speed * direction, true);
        }
    }
}
