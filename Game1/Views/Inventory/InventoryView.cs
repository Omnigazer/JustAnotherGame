using Omniplatformer.HUDStates;
using Omniplatformer.Objects.Items;
using Omniplatformer.Services;

namespace Omniplatformer.Views.Inventory
{
    /// <summary>
    /// Handles the inventory view logic
    /// </summary>
    public class InventoryView : ViewControl
    {
        public Objects.Inventory.Inventory Inventory { get; set; }
        public IInventoryController controller;

        public InventoryView(IInventoryController controller, Objects.Inventory.Inventory inventory = null)
        {
            this.controller = controller;
            Inventory = inventory;
            if (Inventory != null)
                InitSlots();
        }

        public override void SetupNode()
        {
            // Node.Padding = 15;
        }

        public void SetInventory(Objects.Inventory.Inventory inv)
        {
            Inventory = inv;
            if (Inventory != null)
            {
                InitSlots();
            }
        }

        public void InitSlots()
        {
            const int slot_width = 70, slot_height = 70, slot_margin = 15;
            Clear();
            foreach (var slot in Inventory.slots)
            {
                var view = new InventorySlotView(slot)
                { Width = slot_width, Height = slot_height };
                RegisterChild(view);
                view.MouseClick += View_MouseUp;
            }
            Width = slot_width * Inventory.Cols + slot_margin * (Inventory.Cols - 1);
            // Height = slot_height * Inventory.Rows + slot_margin * (Inventory.Rows - 1);
        }

        private void View_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButton.Left)
            {
                var view = (InventorySlotView)sender;
                controller.OnSlotClick(view.Slot);
            }
        }
    }
}
