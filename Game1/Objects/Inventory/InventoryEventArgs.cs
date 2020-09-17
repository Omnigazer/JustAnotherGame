using System;

namespace Omniplatformer.Objects.InventoryNS
{
    public class InventoryEventArgs : EventArgs
    {
        public InventoryEventArgs(Inventory inv)
        {
            this.Inventory = inv;
        }
        public Inventory Inventory { get; }
    }
}
