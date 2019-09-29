using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using Omniplatformer.HUDStates;
using System.Collections.Generic;

namespace Omniplatformer.HUD
{
    /// <summary>
    /// Handles the character equipment view logic
    /// </summary>
    public class EquipView : ViewControl
    {
        Player Player { get; set; }
        IInventoryController controller;

        // slots starting position
        const int slot_width = 70, slot_height = 70;
        const int slot_margin = 30;

        public EquipView(IInventoryController controller, Player player)
        {
            Width = 1000;
            Height = 800;
            Player = player;
            this.controller = controller;
            int i = 0;
            InventorySlotView slot_view;

            foreach (var slot in player.EquipSlots.MiscSlots)
            {
                slot_view = new InventorySlotView(slot, new Point(50, slot_margin + i * (slot_width + slot_margin))) { Width = slot_width, Height = slot_height };
                slot_view.MouseClick += Slot_view_MouseUp;
                RegisterChild(slot_view);
                i++;
            }
            slot_view = new InventorySlotView(player.EquipSlots.RightHandSlot, new Point(200, slot_margin)) { Width = slot_width, Height = slot_height };
            slot_view.MouseClick += Slot_view_MouseUp;
            RegisterChild(slot_view);
            slot_view = new InventorySlotView(player.EquipSlots.LeftHandSlot, new Point(350, slot_margin)) { Width = slot_width, Height = slot_height };
            slot_view.MouseClick += Slot_view_MouseUp;
            RegisterChild(slot_view);
            slot_view = new InventorySlotView(player.EquipSlots.ChannelSlot, new Point(200, slot_height + 2 * slot_margin)) { Width = slot_width, Height = slot_height };
            slot_view.MouseClick += Slot_view_MouseUp;
            RegisterChild(slot_view);
        }

        private void Slot_view_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButton.Left)
            {
                var view = (InventorySlotView)sender;
                controller.OnSlotClick(view.Slot);
            }
        }

        /*
        private void Slot_view_Drag(object sender, System.EventArgs e)
        {
            Player.UnwieldItem();
        }

        private void Slot_view_Drop(object sender, DropEventArgs e)
        {
            Player.WieldItem((WieldedItem)e.DraggedItem);
        }
        */

        public override void Draw()
        {
            DrawContainer();
            base.Draw();
        }

        public void DrawContainer()
        {
            var spriteBatch = GraphicsService.Instance;
            var rect = new Rectangle(GlobalPosition, new Point(Width, Height));
            float alpha = 0.9f;
            spriteBatch.Draw(GameContent.Instance.whitePixel, rect, Color.DarkGray * alpha);
        }
    }
}
