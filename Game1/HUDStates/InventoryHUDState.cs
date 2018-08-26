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
        Game1 Game => GameService.Instance;

        Item MouseStorage { get; set; }

        public Dictionary<Keys, (Action, Action, bool)> Controls { get; set; } = new Dictionary<Keys, (Action, Action, bool)>();

        // TODO: TEMPORARY
        Inventory inv;

        public InventoryHUDState(HUDContainer hud, Inventory inv)
        {
            playerHUD = hud;
            PlayerInventoryView = new InventoryView(this, inv);
            TargetInventoryView = new InventoryView(this, inv) { Visible = false };
            EquipView = new EquipView(this, Game.player);
            Root.RegisterChild(PlayerInventoryView);
            Root.RegisterChild(TargetInventoryView);
            Root.RegisterChild(EquipView);
            // TODO: remove this reference
            this.inv = inv;
            SetupControls();
            Game.onTargetInventoryOpen += onTargetInventoryOpen;
            Game.onTargetInventoryClosed += onTargetInventoryClosed;
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

        public void ProcessLayout()
        {
            int margin = 50;
            var (screen_width, screen_height) = Game.RenderSystem.GetResolution();
            PlayerInventoryView.Position = new Point(
                screen_width - PlayerInventoryView.Width - margin,
                margin
                );
            Point target_inv_position = PlayerInventoryView.Position + new Point(0, PlayerInventoryView.Height) + new Point(0, margin);
            if (TargetInventoryView != null)
                TargetInventoryView.Position = target_inv_position;

            Point equip_position = new Point(600, 2 * margin);
            EquipView.Position = equip_position;
        }

        public override void Draw()
        {
            // EquipView.Draw(new Point());
            ProcessLayout();
            base.Draw();
            DrawStorage();
            playerHUD.Draw();
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
                {  Keys.X, (Game.player.WieldCurrentSlot, noop, false) },
                {  Keys.C, (Game.CloseChest, noop, false) },
                {  Keys.Escape, (Game.CloseInventory, noop, false) }
            };
        }

        InventorySlot dragged_slot;

        void DragItem(InventorySlot slot)
        {
            dragged_slot = slot;
        }

        void DropDraggedSlot(InventorySlot target)
        {
            if (target != null)
            {
                var item = target.Item;
                target.Item = dragged_slot.Item;
                dragged_slot.Item = item;
            }
        }

        public override void HandleControls()
        {
            Game.StopMoving();
            var keyboard_state = Keyboard.GetState();
            foreach (var (key, (pressed_action, released_action, continuous)) in Controls)
            {
                if (keyboard_state.IsKeyDown(key))
                {
                    if (continuous || !release_map.ContainsKey(key) || release_map[key])
                    {
                        release_map[key] = false;
                        pressed_action();
                    }
                }
                else
                {
                    release_map[key] = true;
                    released_action?.Invoke();
                }
            }
        }
    }
}
