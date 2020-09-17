using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Objects.InventoryNS;

namespace Omniplatformer.Components.Character
{
    public class InventoryComponent : Component
    {
        public Inventory Inventory { get; set; }
    }
}
