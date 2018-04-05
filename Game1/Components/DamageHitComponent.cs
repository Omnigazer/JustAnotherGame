using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public DamageHitComponent(GameObject obj, int damage) : base(obj)
        {
            Damage = damage;
        }

        public DamageHitComponent(GameObject obj, int damage, Vector2 knockback) : base(obj)
        {
            Damage = damage;
            Knockback = knockback;
        }

        /// <summary>
        /// Represents the damaging hit action
        /// </summary>
        /// <param name="target"></param>
        public override void Hit(GameObject target)
        {
            // TODO: implement more accurate determining of eligible teams
            // also direct referencing of GameObject
            if (target.Team != GameObject.Team)
            {
                target.ApplyDamage(Damage);
                var movable = (MoveComponent)target;
                var pos = GetComponent<PositionComponent>();
                var their_pos = (PositionComponent)target;
                var dir_sign = Math.Sign(their_pos.WorldPosition.Center.X - pos.WorldPosition.Center.X);
                movable.CurrentMovement += new Vector2(Knockback.X * dir_sign, Knockback.Y);
            }
            base.Hit(target);
        }
    }
}
