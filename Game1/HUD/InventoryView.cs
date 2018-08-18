using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using System.Collections.Generic;

namespace Omniplatformer.HUD
{
    /// <summary>
    /// Handles the inventory view logic
    /// </summary>
    public class InventoryView : ViewControl
    {
        Inventory Inventory { get; set; }
        const int slot_width = 70, slot_height = 70;
        const int slot_margin = 15;

        public InventoryView(Inventory inventory, bool target)
        {
            Inventory = inventory;
            int slot_width = 70, slot_height = 70;
            foreach (var slot in Inventory.slots)
            {
                Content.Add(new InventorySlotView(
                        new Point((slot_width + slot_margin) * slot.Column,
                        (slot_height + slot_margin) * slot.Row)
                    )
                { Width = slot_width, Height = slot_height });
            }
            Width = slot_width * Inventory.Cols + slot_margin * (Inventory.Cols - 1);
            Height = slot_height * Inventory.Rows + slot_margin * (Inventory.Rows - 1);
        }

        /*
        public override void Draw(Point position)
        {
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            foreach (var slot in Inventory.slots)
            {
                DrawSlot(slot);
            }
            spriteBatch.End();
        }


        public Rectangle GetRect(InventorySlot slot)
        {
            var slot_offset = new Point((slot_width + slot_margin) * slot.Column, (slot_height + slot_margin) * slot.Row);
            return new Rectangle(Position + slot_offset, new Point(slot_width, slot_height));
        }


        public void DrawSlot(InventorySlot slot)
        {
            var spriteBatch = GraphicsService.Instance;
            Point slot_position = new Point(Position.X + (slot_width + slot_margin) * slot.Column, Position.Y + (slot_height + slot_margin) * slot.Row);
            Point size = new Point(slot_width, slot_height);
            Rectangle outer_rect = new Rectangle(slot_position, size);

            // Rectangle inner_rect = new Rectangle(bar_position + border_size, size);
            float alpha = slot.IsHighlighted ? 1 : 0.6f;
            spriteBatch.Draw(GameContent.Instance.whitePixel, outer_rect, Color.Gray * alpha);
            outer_rect.Inflate(-5, -5);
            if (slot.item != null)
            {
                var renderable = (RenderComponent)slot.item;
                spriteBatch.Draw(renderable.Texture, outer_rect, Color.White);
            }
        }

        public InventorySlot GetSlotAtPosition(Point pos)
        {
            foreach (var slot in Inventory.slots)
            {
                var rect = GetRect(slot);
                if (rect.Contains(pos))
                {
                    return slot;
                }
            }
            return null;
        }

        public void HoverSlot(InventorySlot slot)
        {
            foreach (var i_slot in Inventory.slots)
            {
                i_slot.IsHovered = (i_slot == slot);
            }
        }

        public void SelectSlot(InventorySlot slot)
        {
            Inventory.SetCurrentSlot(slot);
        }
        */
    }
}
