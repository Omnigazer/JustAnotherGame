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
    public class CharHUDState : HUDState
    {
        HUDContainer playerHUD;
        CharView view;
        // InventoryView PlayerInventoryView { get; set; }
        // InventoryView TargetInventoryView { get; set; }
        Game1 Game => GameService.Instance;

        public Dictionary<Keys, (Action, Action, bool)> Controls { get; set; } = new Dictionary<Keys, (Action, Action, bool)>();

        // TODO: TEMPORARY
        // Inventory inv;

        public CharHUDState(HUDContainer hud)
        {
            playerHUD = hud;
            view = new CharView();
            // PlayerInventoryView = new InventoryView(inv, false);
            // TODO: remove this reference
            // this.inv = inv;
            SetupControls();
        }

        public override void Draw()
        {
            view.Draw();
            playerHUD.Draw();
        }

        public void SetupControls()
        {
            Action noop = delegate { };
            Controls = new Dictionary<Keys, (Action, Action, bool)>()
            {
                // {  Keys.X, (Game.player.WieldCurrentSlot, noop, false) },
                // {  Keys.C, (Game.CloseChest, noop, false) },
                {  Keys.Escape, (Game.CloseChar, noop, false) }
            };
        }

        bool lmb_is_pressed;

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
            var mouse = Mouse.GetState();
            var slot = view.GetSlotAtPosition(mouse.Position);
            if (slot != null)
            {
                var skill = slot.Skill;
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (!lmb_is_pressed)
                    {
                        Game.player.UpgradeSkill(skill);
                    }
                    lmb_is_pressed = true;
                }
                else
                {
                    if (lmb_is_pressed)
                    {

                    }
                    lmb_is_pressed = false;
                }
            }
        }
    }
}
