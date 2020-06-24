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
using Omniplatformer.Objects.Inventory;
using Omniplatformer.Objects.Items;
using Omniplatformer.Services;
using Omniplatformer.Views.HUD;
using Omniplatformer.Views.Inventory;
using Omniplatformer.Components.Character;

namespace Omniplatformer.HUDStates
{
    public interface IInventoryController
    {
        void OnSlotClick(Slot slot);
    }

    public class InventoryHUDState : HUDState, IInventoryController
    {
        HUDContainer playerHUD;
        InventoryView PlayerInventoryView { get; set; }
        InventoryView TargetInventoryView { get; set; }
        EquipView EquipView { get; set; }

        Item MouseStorage { get; set; }
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
                {Visible = false, Node = { Top=370, Right = 20, PositionType = YogaPositionType.Absolute, Wrap = YogaWrap.Wrap, FlexDirection=YogaFlexDirection.Row}};

            EquipView = new EquipView(this, Game.Player.GetComponent<EquipComponent>().EquipSlots);

            Root.RegisterChild(playerHUD);
            Root.RegisterChild(PlayerInventoryView);
            Root.RegisterChild(TargetInventoryView);
            Root.RegisterChild(EquipView);
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

        public void OnSlotClick(Slot slot)
        {
            if (MouseStorage == null)
            {
                MouseStorage = slot.Item;
                slot.Item = null;
                if (MouseStorage != null)
                    slot.OnItemRemove(MouseStorage);
            }
            else
            {
                if (slot.AcceptsItem(MouseStorage))
                {
                    if (slot.Item == null)
                    {
                        slot.Item = MouseStorage;
                        slot.OnItemAdd(slot.Item);
                        MouseStorage = null;
                    }
                    else
                    {
                        var tmp = MouseStorage;
                        MouseStorage = slot.Item;
                        slot.Item = tmp;
                        slot.OnItemRemove(MouseStorage);
                        slot.OnItemAdd(tmp);
                    }
                }
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
            spriteBatch.Begin();
            if (MouseStorage != null)
            {
                GraphicsService.DrawScreen(((RenderComponent)MouseStorage).Texture, new Rectangle(mouse_pos, new Point(60, 60)), Color.White, 0, Vector2.Zero);
            }
            spriteBatch.End();
        }

        public void SetupControls()
        {
            Action noop = delegate { };
            Controls = new Dictionary<Keys, (Action, Action, bool)>()
            {
                {  Keys.Left, (PlayerInventory.MoveLeft, noop, false) },
                {  Keys.Up, (PlayerInventory.MoveUp, noop, false) },
                {  Keys.Right, (PlayerInventory.MoveRight, noop, false) },
                {  Keys.Down, (PlayerInventory.MoveDown, noop, false) },
                {  Keys.C, (Game.CloseChest, noop, false) },
                {  Keys.Escape, (Game.CloseInventory, noop, false) },
                {  Keys.I, (Game.CloseInventory, noop, false) }
            };
        }
    }
}
