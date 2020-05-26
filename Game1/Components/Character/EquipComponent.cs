using Omniplatformer.Objects;
using Omniplatformer.Objects.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components.Character
{
    class EquipComponent : Component
    {
        public EquipSlotCollection EquipSlots { get; set; }
    }
}
