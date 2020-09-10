using Omniplatformer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Interactibles
{
    public abstract class InteractibleComponent : Component
    {
        public abstract void Interact();
    }
}
