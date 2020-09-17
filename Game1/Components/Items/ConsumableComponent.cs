using Omniplatformer.Components.Character;
using Omniplatformer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components.Items
{
    public abstract class ConsumableComponent : Component
    {
        public abstract void ApplyEffect();
    }
}
