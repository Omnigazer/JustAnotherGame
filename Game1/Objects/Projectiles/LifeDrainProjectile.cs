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
        public LifeDrainProjectile(Vector2 center, Vector2 direction, GameObject source = null): base(source)
        {
            Team = source?.Team ?? Team.Friend;

            var proj_movable = new ProjectileMoveComponent(this, center, new Vector2(20, 5)) { InverseMass = 0 };
            // direction.Normalize();
            // float speed = 20;
            proj_movable.Rotate(-(float)Math.Atan2(direction.Y, direction.X));
            // proj_movable.CurrentMovement = speed * direction;
            proj_movable.ApplyImpulse(direction, true);

            Components.Add(proj_movable);
            var c = new GlowingRenderComponent(this) { GlowColor = Color.Purple };
            Components.Add(c);
            Components.Add(new LifeDrainHitComponent(this, damage: 3));
        }

        public override object AsJson()
        {
            return new { type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }
    }
}
