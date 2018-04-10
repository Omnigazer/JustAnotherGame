using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public abstract class HitComponent : Component
    {
        public HitComponent(GameObject obj) : base(obj)
        {
        }

        public virtual void Hit(GameObject target)
        {

        }
    }
}
