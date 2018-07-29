using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using System.Collections.Generic;

namespace Omniplatformer.HUD
{
    /// <summary>
    /// Handles the character equipment view logic
    /// </summary>
    public class EquipView : ViewControl
    {
        Player Player { get; set; }

        // slots starting position
        const int slot_width = 70, slot_height = 70;
        const int slot_margin = 15;

        public override int Width => 0;
        public override int Height => 0;

        public EquipView(Player player)
        {
            Player = player;
        }

        public override void Draw()
        {
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            DrawContainer();
            DrawMisc();
            /*
            foreach (var slot in Inventory.slots)
            {
                DrawSlot(slot);
            }
            */
            spriteBatch.End();
        }

        /*
        public Rectangle GetRect(InventorySlot slot)
        {
            var slot_offset = new Point((slot_width + slot_margin) * slot.Column, (slot_height + slot_margin) * slot.Row);
            return new Rectangle(position + slot_offset, new Point(slot_width, slot_height));
        }
        */

        public void DrawContainer()
        {
            var spriteBatch = GraphicsService.Instance;
            var rect = new Rectangle(Position, new Point(1000, 800));
            float alpha = 0.9f;
            spriteBatch.Draw(GameContent.Instance.whitePixel, rect, Color.DarkGray * alpha);
        }

        public void DrawMisc()
        {
            var spriteBatch = GraphicsService.Instance;
            int margin = 15;
            Point slot_position = Position + new Point(margin, margin);
            Point size = new Point(slot_width, slot_height);
            float alpha = true ? 1 : 0.6f;
            foreach (var slot in Player.equip_slots)
            {
                Rectangle rect = new Rectangle(slot_position, size);
                spriteBatch.Draw(GameContent.Instance.whitePixel, rect, Color.Gray * alpha);
                slot_position += new Point(0, slot_height + margin);
            }




            // Rectangle inner_rect = new Rectangle(bar_position + border_size, size);



            /*
            if (slot.item != null)
            {
                var renderable = (RenderComponent)slot.item;
                spriteBatch.Draw(renderable.Texture, outer_rect, Color.White);
            }
            */
        }

        public void DrawSlot(InventorySlot slot)
        {
            var spriteBatch = GraphicsService.Instance;
            /*
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
            */
        }

        /*
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
        */

        /*
        public void HoverSlot(InventorySlot slot)
        {
            foreach (var i_slot in Inventory.slots)
            {
                i_slot.IsHovered = (i_slot == slot);
            }
        }
        */

        /*
        public void SelectSlot(InventorySlot slot)
        {
            Inventory.SetCurrentSlot(slot);
        }
        */
    }
}
