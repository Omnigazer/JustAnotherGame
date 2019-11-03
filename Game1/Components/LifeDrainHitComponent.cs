using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;

namespace Omniplatformer.Components
{
    /// <summary>
    /// Used for collision-based attacks, such as melee monster hit or a projectile hit
    /// </summary>
    class LifeDrainHitComponent : HitComponent
    {
        /// <summary>
        /// Represents the damage dealt on hit
        /// </summary>
        public int Damage { get; set; }
        public Vector2 Knockback { get; set; }
        public override bool EligibleTarget(GameObject target) => target.Team != GameObject.Team;

        public LifeDrainHitComponent(GameObject obj, int damage) : base(obj)
        {
            Damage = damage;
        }

        public override void ApplyEffect(GameObject target)
        {
            var damage = DetermineDamage();
            target.ApplyDamage(damage);
            GameObject.Source.ApplyDamage(-damage);
        }

        protected virtual int DetermineDamage()
        {
            return Damage;
        }
    }
}
