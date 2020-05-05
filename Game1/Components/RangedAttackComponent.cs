using Omniplatformer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public abstract class RangedAttackComponent : Component
    {
        public bool IsAttacking { get; set; }
        public RangedAttackComponent() { }
        public RangedAttackComponent(GameObject obj) : base(obj) { }

        public abstract bool CanAttack();
        public abstract void Attack();
    }
}
