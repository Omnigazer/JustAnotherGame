using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    public class InventorySlotView : ViewControl
    {
        public Texture2D Image { get; set; }

        public InventorySlotView(Point position)
        {
            Position = position;
        }

        public override void Draw(Point position)
        {
            // var slot = new InventorySlot(0, 0);
            var spriteBatch = GraphicsService.Instance;
            // Point slot_position = new Point(Position.X + (Width + Margin) * slot.Column, Position.Y + (Height + Margin) * slot.Row);
            Point size = new Point(Width, Height);
            Rectangle outer_rect = new Rectangle(Position + position, size);

            // Rectangle inner_rect = new Rectangle(bar_position + border_size, size);
            // float alpha = slot.IsHighlighted ? 1 : 0.6f;
            float alpha = 1;
            spriteBatch.Draw(GameContent.Instance.whitePixel, outer_rect, Color.Gray * alpha);
            outer_rect.Inflate(-5, -5);
            /*
            if (slot.item != null)
            {
                var renderable = (RenderComponent)slot.item;
                spriteBatch.Draw(renderable.Texture, outer_rect, Color.White);
            }
            */
            if (Image != null)
            {
                spriteBatch.Draw(Image, outer_rect, Color.White);
            }
        }
    }
}
