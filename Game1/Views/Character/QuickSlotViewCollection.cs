using System;
using Microsoft.Xna.Framework;
using Omniplatformer.Components.Actions;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Services;
using Omniplatformer.Views.BasicControls;

namespace Omniplatformer.Views.Character
{
    /// <summary>
    /// Handles the inventory view logic
    /// </summary>
    public class QuickSlotViewCollection : ViewControl
    {
        public static int SlotsCount => 9;

        public QuickSlotViewCollection()
        {
            Row row = new Row();
            RegisterChild(row);
            for (int i = 0; i < SlotsCount; i++)
            {
                row.RegisterChild(new QuickSlotView(i));
            }
        }

        public override void SetupNode()
        {
            Width = 650;
        }
    }

    public class QuickSlotView : ViewControl
    {
        Player Player => GameService.Player;
        EquipComponent Equippable => Player.GetComponent<EquipComponent>();
        public int SlotIndex { get; set; }

        public QuickSlotView(int index)
        {
            SlotIndex = index;
        }

        public override void SetupNode()
        {
            Width = 60;
            Height = 60;
            Margin = 5;
            BorderThickness = 2;
        }

        public override void DrawSelf()
        {
            DrawSlot();
        }

        public void DrawSlot()
        {
            var spriteBatch = GraphicsService.Instance;
            ActionComponent action;
            action = Player.Actionable.QuickSlots[SlotIndex];
            if (action != null)
            {
                var texture = action.GameObject.GetComponent<RenderComponent>().Texture;
                var rect = GlobalRect;
                rect.Inflate(-5, -5);
                spriteBatch.Draw(texture, rect, Color.White);
                //var count = Slot.Item.Count;
                //if (count != 1)
                //    spriteBatch.DrawString(GameContent.Instance.defaultFont, count.ToString(), new Vector2(outer_rect.Right, outer_rect.Bottom), Color.White);
            }

            var border_color = Player.Actionable.CurrentQuickSlot == SlotIndex ? Color.LightYellow : Color.Black;
            DrawBorder(border_color);
        }
    }
}
