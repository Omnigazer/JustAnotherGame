using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omniplatformer.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Items;
using Omniplatformer.Components;

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

        // TODO: TEMPORARY
        Inventory inv;

        public InventoryHUDState(Inventory inv)
        {
            playerHUD = new HUDContainer();
            // TODO: remove this reference
            this.inv = inv;
            SetupControls();
            Game.onTargetInventoryOpen += onTargetInventoryOpen;
            Game.onTargetInventoryClosed += onTargetInventoryClosed;
            SetupGUI();
        }

        public override void RegisterChildren()
        {
            PlayerInventoryView = new InventoryView(this, inv);
            var x = PlayerInventoryView.Node;
            x.Top = 20;
            x.Right = 20;
            x.PositionType = Facebook.Yoga.YogaPositionType.Absolute;
            x.FlexDirection = Facebook.Yoga.YogaFlexDirection.Row;
            x.Wrap = Facebook.Yoga.YogaWrap.Wrap;

            TargetInventoryView = new InventoryView(this, inv) { Visible = false };
            x = TargetInventoryView.Node;
            x.PositionType = Facebook.Yoga.YogaPositionType.Absolute;

            EquipView = new EquipView(this, Game.Player);

            Root.RegisterChild(playerHUD);
            Root.RegisterChild(PlayerInventoryView);
            Root.RegisterChild(TargetInventoryView);
            Root.RegisterChild(EquipView);
        }

        private void onTargetInventoryOpen(object sender, InventoryEventArgs e)
        {
            SetTargetInventory(e.Inventory);
            TargetInventoryView.Visible = true;
        }

        private void onTargetInventoryClosed(object sender, EventArgs e)
        {
            ClearTargetInventory();
        }

        public void SetTargetInventory(Inventory inv)
        {
            TargetInventoryView.SetInventory(inv);
        }

        public void ClearTargetInventory()
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
                var mouse_pos = Mouse.GetState().Position;
                GraphicsService.DrawScreen(((RenderComponent)MouseStorage).Texture, new Rectangle(mouse_pos, new Point(60, 60)), Color.White, 0, Vector2.Zero);
            }
            spriteBatch.End();
        }

        public void SetupControls()
        {
            Action noop = delegate { };
            Controls = new Dictionary<Keys, (Action, Action, bool)>()
            {
                {  Keys.Left, (inv.MoveLeft, noop, false) },
                {  Keys.Up, (inv.MoveUp, noop, false) },
                {  Keys.Right, (inv.MoveRight, noop, false) },
                {  Keys.Down, (inv.MoveDown, noop, false) },                
                {  Keys.C, (Game.CloseChest, noop, false) },
                {  Keys.Escape, (Game.CloseInventory, noop, false) },
                {  Keys.I, (Game.CloseInventory, noop, false) }
            };
        }
    }
}
