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
        public Slot Slot { get; set; }
        protected override GameObject DragObject { get => Slot.Item; set => Slot.Item = (WieldedItem)value; }

        public InventorySlotView(Slot slot, Point position)
        {
            Slot = slot;
            Position = position;
            IsDragSource = true;
            IsDropTarget = true;
        }

        private void InventorySlotView_Drag(object sender, EventArgs e)
        {

        }

        public override void Draw()
        {
            var spriteBatch = GraphicsService.Instance;
            Rectangle outer_rect = GlobalRect;
            float alpha = Hover || Slot.IsCurrent ? 1 : 0.6f;
            // float alpha = 1;
            spriteBatch.Draw(GameContent.Instance.whitePixel, outer_rect, Color.Gray * alpha);
            outer_rect.Inflate(-5, -5);
            /*
            if (slot.item != null)
            {
                var renderable = (RenderComponent)slot.item;
                spriteBatch.Draw(renderable.Texture, outer_rect, Color.White);
            }
            */
            if (Slot.Item != null)
            {
                var texture = ((RenderComponent)Slot.Item).Texture;
                spriteBatch.Draw(texture, outer_rect, Color.White);
            }
        }
    }
}
