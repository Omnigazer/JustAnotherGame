using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
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
