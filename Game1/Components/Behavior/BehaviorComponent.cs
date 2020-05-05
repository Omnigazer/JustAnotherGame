using Omniplatformer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components.Behavior
{
    public class BehaviorComponent : Component
    {
        public bool Aggressive { get; set; }

        public BehaviorComponent() { }
        public BehaviorComponent(GameObject obj) : base(obj) { }
    }
}
