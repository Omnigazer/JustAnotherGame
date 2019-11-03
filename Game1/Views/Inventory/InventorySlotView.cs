using System;
using Microsoft.Xna.Framework;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Objects.Inventory;
using Omniplatformer.Services;

namespace Omniplatformer.Views.Inventory
{
    public class InventorySlotView : ViewControl
    {
        public Slot Slot { get; set; }
        // protected override GameObject DragObject { get => Slot.Item; set => Slot.Item = (WieldedItem)value; }

        public InventorySlotView(Slot slot)
        {
            Slot = slot;
            IsDragSource = true;
            IsDropTarget = true;
        }

        private void InventorySlotView_Drag(object sender, EventArgs e)
        {

        }

        public override void SetupNode()
        {
            Width = 70;
            Height = 70;
            Node.Margin = 5;
        }

        public override void DrawSelf()
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
