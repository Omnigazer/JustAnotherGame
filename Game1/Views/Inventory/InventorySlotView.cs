using System;
using Microsoft.Xna.Framework;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Objects.InventoryNS;
using Omniplatformer.Services;

namespace Omniplatformer.Views.InventoryNS
{
    public class InventorySlotView : ViewControl
    {
        public ItemSlot Slot { get; set; }

        public InventorySlotView(ItemSlot slot)
        {
            Slot = slot;
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
            float alpha = Hover ? 1 : 0.6f;
            // float alpha = 1;
            spriteBatch.Draw(GameContent.Instance.whitePixel, outer_rect, Color.Gray * alpha);
            outer_rect.Inflate(-5, -5);

            if (Slot.Item != null)
            {
                var texture = ((RenderComponent)Slot.Item).Texture;
                spriteBatch.Draw(texture, outer_rect, Color.White);
                outer_rect.Inflate(-10, -15);
                spriteBatch.DrawString(GameContent.Instance.defaultFont, Slot.Item.Count.ToString(), new Vector2(outer_rect.Right, outer_rect.Bottom), Color.White);
            }
        }
    }
}
