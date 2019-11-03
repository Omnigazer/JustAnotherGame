using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;

namespace Omniplatformer.Components
{
    public abstract class HitComponent : Component
    {
        public HitComponent(GameObject obj) : base(obj)
        {
        }

        public abstract bool EligibleTarget(GameObject target);

        public virtual void Hit(GameObject target)
        {
            if (EligibleTarget(target))
            {
                ApplyEffect(target);
            }
        }

        public virtual void ApplyEffect(GameObject target)
        {

        }
    }
}
