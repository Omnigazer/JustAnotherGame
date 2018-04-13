using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using System.Collections.Generic;

namespace Omniplatformer.HUD
{
    /// <summary>
    /// Handles the inventory view logic
    /// </summary>
    public class InventoryView
    {
        Inventory Inventory { get; set; }

        public InventoryView(Inventory inventory)
        {
            Inventory = inventory;
        }

        public void Draw()
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
            return new Rectangle(position + slot_offset, new Point(slot_width, slot_height));
        }

        // slots starting position
        Point position = new Point(700, 150);
        const int slot_width = 70, slot_height = 70;
        const int slot_margin = 15;

        public void DrawSlot(InventorySlot slot)
        {
            var spriteBatch = GraphicsService.Instance;
            Point slot_position = new Point(position.X + (slot_width + slot_margin) * slot.Column, position.Y + (slot_height + slot_margin) * slot.Row);
            Point size = new Point(slot_width, slot_height);
            Rectangle outer_rect = new Rectangle(slot_position, size);

            // Rectangle inner_rect = new Rectangle(bar_position + border_size, size);
            float alpha = slot.IsHighlighted ? 1 : 0.3f;
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
            foreach (var i_slot in Inventory.slots)
            {
                if (i_slot == slot)
                {
                    // TODO: extract this into the inventory logic
                    i_slot.IsCurrent = true;
                    Inventory.col = i_slot.Column;
                    Inventory.row = i_slot.Row;
                }
                else
                {
                    i_slot.IsCurrent = false;
                }
            }
        }
    }
}
