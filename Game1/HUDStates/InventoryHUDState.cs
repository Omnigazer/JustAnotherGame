using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Omniplatformer.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUDStates
{
    public class InventoryHUDState : HUDState
    {
        HUDContainer playerHUD;
        InventoryView PlayerInventoryView { get; set; }
        InventoryView TargetInventoryView { get; set; }
        EquipView EquipView { get; set; }
        Game1 Game => GameService.Instance;

        public Dictionary<Keys, (Action, Action, bool)> Controls { get; set; } = new Dictionary<Keys, (Action, Action, bool)>();

        // TODO: TEMPORARY
        Inventory inv;

        public InventoryHUDState(HUDContainer hud, Inventory inv)
        {
            playerHUD = hud;
            PlayerInventoryView = new InventoryView(inv, false);
            EquipView = new EquipView(Game.player);
            // TODO: remove this reference
            this.inv = inv;
            SetupControls();
            Game.onTargetInventoryOpen += onTargetInventoryOpen;
            Game.onTargetInventoryClosed += onTargetInventoryClosed; ;
        }

        private void onTargetInventoryOpen(object sender, InventoryEventArgs e)
        {
            SetTargetInventory(e.Inventory);
        }

        private void onTargetInventoryClosed(object sender, EventArgs e)
        {
            ClearTargetInventory();
        }

        public void SetTargetInventory(Inventory inv)
        {
            TargetInventoryView = new InventoryView(inv, true);
        }

        public void ClearTargetInventory()
        {
            TargetInventoryView = null;
        }

        public override void Draw()
        {
            int margin = 50;
            var (screen_width, screen_height) = Game.RenderSystem.GetResolution();
            PlayerInventoryView.Position = new Point(
                screen_width - PlayerInventoryView.Width - margin,
                margin
                );
            PlayerInventoryView.Draw(new Point());
            Point target_inv_position = PlayerInventoryView.Position + new Point(0, PlayerInventoryView.Height) + new Point(0, margin);
            if (TargetInventoryView != null)
                TargetInventoryView.Position = target_inv_position;
            TargetInventoryView?.Draw(new Point());

            Point equip_position = new Point(600, 2 * margin);
            EquipView.Position = equip_position;
            EquipView.Draw(new Point());
            playerHUD.Draw();
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
        bool lmb_is_pressed;

        /*
        InventorySlot GetSlotAtPosition(Point position)
        {
            InventorySlot slot = null;
            // we're assuming only one of these will match
            slot = PlayerInventoryView.GetSlotAtPosition(position) ?? TargetInventoryView?.GetSlotAtPosition(position);
            return slot;
        }
        */

        void DragItem(InventorySlot slot)
        {
            dragged_slot = slot;
        }

        void DropDraggedSlot(InventorySlot target)
        {
            if (target != null)
            {
                var item = target.item;
                target.item = dragged_slot.item;
                dragged_slot.item = item;
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

            HandleMouse();
        }

        public void HandleMouse()
        {
            /*
            var mouse = Mouse.GetState();
            // var slot = PlayerInventoryView.GetSlotAtPosition(mouse.Position);
            var slot = GetSlotAtPosition(mouse.Position);
            // well, now making assumptions gets silly
            PlayerInventoryView.HoverSlot(slot);
            TargetInventoryView?.HoverSlot(slot);
            if (slot != null)
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    // TODO: figure out how to get rid of these twin calls
                    PlayerInventoryView.SelectSlot(slot);
                    TargetInventoryView?.SelectSlot(slot);
                    if (!lmb_is_pressed)
                    {
                        DragItem(slot);
                    }
                    lmb_is_pressed = true;
                }
                else
                {
                    if (lmb_is_pressed)
                    {
                        DropDraggedSlot(slot);
                    }
                    lmb_is_pressed = false;
                }
            }
            */
        }
    }
}
