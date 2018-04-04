using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    /// <summary>
    /// Used for the usual monster-player collision attacks
    /// </summary>
    class DamageHitComponent : HitComponent
    {
        /// <summary>
        /// Represents the damage dealt on hit
        /// </summary>
        public int Damage { get; set; }
        public DamageHitComponent(GameObject obj, int damage) : base(obj)
        {
            Damage = damage;
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
            target.ApplyDamage(Damage);
            base.Hit(target);
        }
    }
}
