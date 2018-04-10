using Microsoft.Xna.Framework.Input;
using Omniplatformer.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUDStates
{
    public class InventoryHUDState : IHUDState
    {
        HUDContainer playerHUD;
        InventoryView InventoryView { get; set; }
        Game1 Game => GameService.Instance;

        public Dictionary<Keys, (Action, Action, bool)> Controls { get; set; } = new Dictionary<Keys, (Action, Action, bool)>();
        Dictionary<Keys, bool> release_map = new Dictionary<Keys, bool>();

        // TODO: TEMPORARY
        Inventory inv;

        public InventoryHUDState(HUDContainer hud, Inventory inv)
        {
            playerHUD = hud;
            InventoryView = new InventoryView(inv);
            // TODO: remove this reference
            this.inv = inv;
            SetupControls();
        }

        public void Draw()
        {
            InventoryView.Draw();
            // playerHUD.Draw();
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
                // {  Keys.Z, (Game.PickUpSword, noop, false) },
                {  Keys.X, (Game.player.WieldCurrentSlot, noop, false) },
                {  Keys.Escape, (Game.CloseInventory, noop, false) }
            };
        }



        public void HandleControls()
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
