using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Components.Physics;
using Omniplatformer.Objects;

namespace Omniplatformer.Components
{
    /// <summary>
    /// Used for collision-based attacks, such as melee monster hit or a projectile hit
    /// </summary>
    class DamageHitComponent : HitComponent
    {
        /// <summary>
        /// Represents the damage dealt on hit
        /// </summary>
        public int Damage { get; set; }
        public Vector2 Knockback { get; set; }
        public override bool EligibleTarget(GameObject target) => target.Team != GameObject.Team;

        public DamageHitComponent(GameObject obj, int damage) : base(obj)
        {
            Damage = damage;
        }

        public DamageHitComponent(GameObject obj, int damage, Vector2 knockback) : base(obj)
        {
            Damage = damage;
            Knockback = knockback;
        }

        public override void ApplyEffect(GameObject target)
        {
            target.ApplyDamage(DetermineDamage());
            ApplyKnockback(target);
        }

        public void ApplyKnockback(GameObject target)
        {
            var movable = target.GetComponent<DynamicPhysicsComponent>();
            if (movable != null)
            {
                var pos = GetComponent<PositionComponent>();
                var their_pos = (PositionComponent)target;
                var dir_sign = Math.Sign(their_pos.WorldPosition.Center.X - pos.WorldPosition.Center.X);
                movable.ApplyImpulse(new Vector2(Knockback.X * dir_sign, Knockback.Y));
            }
        }

        protected virtual int DetermineDamage()
        {
            return Damage;
        }
    }
}
