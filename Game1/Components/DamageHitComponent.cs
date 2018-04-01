using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
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

        public override void Hit(GameObject obj)
        {
            obj.ApplyDamage(Damage);
            base.Hit(obj);
        }
    }
}
