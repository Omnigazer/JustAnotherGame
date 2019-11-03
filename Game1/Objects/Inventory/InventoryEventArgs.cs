using System;

namespace Omniplatformer.Objects.Inventory
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
