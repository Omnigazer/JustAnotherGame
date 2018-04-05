using Microsoft.Xna.Framework.Input;
using Omniplatformer.Components;
using Omniplatformer.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUDStates
{
    public class DefaultHUDState : IHUDState
    {
        HUDContainer playerHUD;
        Game1 Game => GameService.Instance;

        public Dictionary<Keys, (Action, Action, bool)> Controls { get; set; } = new Dictionary<Keys, (Action, Action, bool)>();
        Dictionary<Keys, bool> release_map = new Dictionary<Keys, bool>();

        public DefaultHUDState()
        {
            playerHUD = new HUDContainer();
            SetupControls();
        }

        public void Draw()
        {
            playerHUD.Draw();
        }

        public void SetupControls()
        {
            Action noop = delegate { };
            Controls = new Dictionary<Keys, (Action, Action, bool)>()
            {
                {  Keys.Left, (Game.WalkLeft, noop, true) },
                {  Keys.Right, (Game.WalkRight, noop, true) },
                {  Keys.Down, (Game.GoDown, noop, true) },
                {  Keys.Up, (Game.GoUp, noop, true) },
                {  Keys.Space, (Game.Jump, Game.StopJumping, false) },
                {  Keys.Z, (Game.Fire, noop, false) },
                {  Keys.X, (Game.Swing, noop, false) },
                {  Keys.C, (Game.ToggleItem, noop, false) },
                {  Keys.OemMinus, (Game.ZoomOut, noop, true) },
                {  Keys.OemPlus, (Game.ZoomIn, noop, true) },
            };
        }

        // check if these keys were released prior to this tick
        bool space_released = true;
        bool fire_released = true;
        bool attack_released = true;
        bool wield_released = true;
        public void HandleControls()
        {
            // TODO: possibly refactor this
            // reset the player's "intention to move" (move_direction) by default as a workaround
            Game.StopMoving();
            var keyboard_state = Keyboard.GetState();
            foreach (var (key, (pressed_action, released_action, continuous)) in Controls)
            {
                if (keyboard_state.IsKeyDown(key))
                {
                    if(continuous || !release_map.ContainsKey(key) || release_map[key])
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

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Game.DragObject();
            }
            else
            {
                Game.ReleaseDraggedObject();
            }
            return;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                Game.WalkLeft();
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                Game.WalkRight();
            else if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Game.GoUp();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Game.GoDown();
            }
            else
                Game.StopMoving();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (space_released)
                {
                    space_released = false;
                    Game.Jump();
                }
            }
            else
            {
                space_released = true;
                Game.StopJumping();
            }


            /*
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
            {
                // var pos = (PositionComponent)player;
                var pos = (PositionComponent)sword;
                pos.Rotate(0.1f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6))
            {
                // var pos = (PositionComponent)player;
                var pos = (PositionComponent)sword;
                pos.Rotate(-0.1f);
            }
            */

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                if (fire_released)
                {
                    fire_released = false;
                    Game.Fire();
                }
            }
            else
            {
                fire_released = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                if (attack_released)
                {
                    attack_released = false;
                    Game.Swing();
                }
            }
            else
            {
                attack_released = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                if (wield_released)
                {
                    wield_released = false;
                    Game.ToggleItem();
                }
            }
            else
            {
                wield_released = true;
            }



            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                Game.ZoomOut();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                Game.ZoomIn();
            }
        }
    }
}
