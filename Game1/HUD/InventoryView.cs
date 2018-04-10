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

        // slots starting position
        Point position = new Point(700, 150);

        public void DrawSlot(InventorySlot slot)
        {
            const int slot_width = 100, slot_height = 100;
            const int slot_margin = 25;

            var spriteBatch = GraphicsService.Instance;
            Point slot_position = new Point(position.X + (slot_width + slot_margin) * slot.Column, position.Y + (slot_height + slot_margin) * slot.Row);
            Point size = new Point(slot_width, slot_height);
            Rectangle outer_rect = new Rectangle(slot_position, size);

            // Rectangle inner_rect = new Rectangle(bar_position + border_size, size);
            float alpha = slot.IsCurrent ? 1 : 0.3f;
            spriteBatch.Draw(GameContent.Instance.whitePixel, outer_rect, Color.Gray * alpha);
            outer_rect.Inflate(-5, -5);
            if (slot.item != null)
            {
                var renderable = (RenderComponent)slot.item;
                spriteBatch.Draw(renderable.Texture, outer_rect, Color.White);
            }
        }
    }
}
