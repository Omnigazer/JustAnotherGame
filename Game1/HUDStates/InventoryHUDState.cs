using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook.Yoga;
using Omniplatformer.Components;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Objects.InventoryNS;
using Omniplatformer.Objects.Items;
using Omniplatformer.Services;
using Omniplatformer.Views.HUD;
using Omniplatformer.Views.InventoryNS;
using Omniplatformer.Components.Character;
using Omniplatformer.Content;
using Omniplatformer.Components.Items;

namespace Omniplatformer.HUDStates
{
    public interface IInventoryController
    {
        void OnSlotLeftClick(ItemSlot slot);
        void OnSlotRightClick(ItemSlot slot);
    }

    public class InventoryHUDState : HUDState, IInventoryController
    {
        HUDContainer playerHUD;
        InventoryView PlayerInventoryView { get; set; }
        InventoryView TargetInventoryView { get; set; }
        EquipView EquipView { get; set; }

        Item MouseStorage;
        private Inventory PlayerInventory { get; set; }

        public InventoryHUDState(Inventory playerInventory)
        {
            playerHUD = new HUDContainer();
            // TODO: remove this reference
            this.PlayerInventory = playerInventory;
            SetupControls();
            Game.onTargetInventoryOpen.Subscribe((inv) => onTargetInventoryOpen(inv));
            Game.onTargetInventoryClosed.Subscribe((_) => onTargetInventoryClosed());
            SetupGUI();
        }

        public override void RegisterChildren()
        {
            PlayerInventoryView = new InventoryView(this, PlayerInventory)
            {
                Node =
                {
                    Top = 20,
                    Right = 20,
                    PositionType = Facebook.Yoga.YogaPositionType.Absolute,
                    FlexDirection = Facebook.Yoga.YogaFlexDirection.Row,
                    Wrap = Facebook.Yoga.YogaWrap.Wrap
                }
            };

            // TODO: extract this positioning to a parent
            TargetInventoryView = new InventoryView(this)
            { Visible = false, Node = { Top = 370, Right = 20, PositionType = YogaPositionType.Absolute, Wrap = YogaWrap.Wrap, FlexDirection = YogaFlexDirection.Row } };

            EquipView = new EquipView(this, Game.Player.GetComponent<EquipComponent>().EquipSlots);

            Root.RegisterChild(playerHUD);
            Root.RegisterChild(PlayerInventoryView);
            Root.RegisterChild(TargetInventoryView);
            Root.RegisterChild(EquipView);
            Root.MouseClick += (sender, e) =>
            {
                ConsumeCursorItem();
            };
        }

        private void ConsumeCursorItem()
        {
            var consumable = MouseStorage?.GetComponent<ConsumableComponent>();
            if (consumable != null)
            {
                consumable.ApplyEffect();
                if (--MouseStorage.Count == 0)
                    MouseStorage = null;
            }
        }

        private void onTargetInventoryOpen(Inventory inv)
        {
            TargetInventoryView.SetInventory(inv);
            TargetInventoryView.Visible = true;
        }

        private void onTargetInventoryClosed()
        {
            TargetInventoryView.Visible = false;
            TargetInventoryView.SetInventory(null);
        }

        public void OnSlotLeftClick(ItemSlot slot)
        {
            if (MouseStorage == null)
                slot.TakeItem(ref MouseStorage);
            else if (slot.AcceptsItem(MouseStorage))
                if (slot.Item == null)
                    slot.PutItem(ref MouseStorage);
                else
                {
                    if (slot.Item.ItemId == MouseStorage.ItemId)
                        slot.MergeStack(ref MouseStorage);
                    else
                        slot.SwapItem(ref MouseStorage);
                }
        }

        public void OnSlotRightClick(ItemSlot slot)
        {
            if (MouseStorage == null)
            {
                if (slot.Item != null)
                    slot.SplitStack(ref MouseStorage);
            }
            else if (slot.AcceptsItem(MouseStorage))
                if (slot.Item == null)
                    slot.PutFirstItem(ref MouseStorage);
                else if (slot.Item.ItemId == MouseStorage.ItemId)
                {
                    if (slot.Item.Count < slot.Item.MaxCount)
                        slot.MergeFirstItem(ref MouseStorage);
                }
        }

        public override void Draw()
        {
            base.Draw();
            DrawStorage();
        }

        public virtual void DrawStorage()
        {
            var spriteBatch = GraphicsService.Instance;
            if (MouseStorage != null)
            {
                spriteBatch.Begin();
                GraphicsService.DrawScreen(((RenderComponent)MouseStorage).Texture, new Rectangle(mouse_pos, new Point(60, 60)), Color.White, 0, Vector2.Zero);
                spriteBatch.DrawString(GameContent.Instance.defaultFont, MouseStorage.Count.ToString(), (mouse_pos + new Point(50, 45)).ToVector2(), Color.White);
                spriteBatch.End();
            }
        }

        public void SetupControls()
        {
            Action noop = delegate { };
            Controls = new Dictionary<Keys, (Action, Action, bool)>()
            {
                {  Keys.C, (Game.CloseChest, noop, false) },
                {  Keys.Escape, (Game.CloseInventory, noop, false) },
                {  Keys.I, (Game.CloseInventory, noop, false) },
            };
        }
    }
}
