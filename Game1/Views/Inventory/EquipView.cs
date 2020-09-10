using Microsoft.Xna.Framework;
using Omniplatformer.Content;
using Omniplatformer.HUDStates;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Objects.Inventory;
using Omniplatformer.Services;
using Omniplatformer.Views.BasicControls;

namespace Omniplatformer.Views.Inventory
{
    /// <summary>
    /// Handles the character equipment view logic
    /// </summary>
    public class EquipView : ViewControl
    {
        EquipSlotCollection EquipSlots { get; set; }
        IInventoryController controller;

        public EquipView(IInventoryController controller, EquipSlotCollection equip_slots)
        {
            EquipSlots = equip_slots;
            this.controller = controller;
            SetupGUI();
        }

        public override void SetupNode()
        {
            Width = 1000;
            Height = 800;
        }

        public void SetupGUI()
        {
            Clear();
            var main_equip_slots = new Row()
            {
                BuildEquipSlot(EquipSlots.RightHandSlot),
                BuildEquipSlot(EquipSlots.LeftHandSlot),
                BuildEquipSlot(EquipSlots.ChannelSlot)
            };
            var misc_slots = new Row();
            foreach (var slot in EquipSlots.MiscSlots)
            {
                misc_slots.RegisterChild(BuildEquipSlot(slot));
            }
            RegisterChild(main_equip_slots);
            RegisterChild(misc_slots);
        }

        public InventorySlotView BuildEquipSlot(Slot slot)
        {
            var view = new InventorySlotView(slot);
            view.MouseClick += Slot_view_MouseUp;
            return view;
        }

        private void Slot_view_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                var view = (InventorySlotView)sender;
                controller.OnSlotLeftClick(view.Slot);
            }
        }

        public override void DrawSelf()
        {
            DrawContainer();
        }

        public void DrawContainer()
        {
            var spriteBatch = GraphicsService.Instance;
            float alpha = 0.9f;
            spriteBatch.Draw(GameContent.Instance.whitePixel, GlobalRect, Color.DarkGray * alpha);
        }
    }
}
