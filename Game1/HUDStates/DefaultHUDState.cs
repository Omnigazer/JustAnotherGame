﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public class DefaultHUDState : HUDState
    {
        HUDContainer playerHUD;
        Game1 Game => GameService.Instance;

        public Dictionary<Keys, (Action, Action, bool)> Controls { get; set; } = new Dictionary<Keys, (Action, Action, bool)>();

        public DefaultHUDState(HUDContainer hud)
        {
            playerHUD = hud;
            SetupControls();
        }

        public override void Draw()
        {
            playerHUD.Draw();

            // TODO: TEST
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            int log_width = 500, log_margin = 50;
            Point log_position = new Point(Game.GraphicsDevice.PresentationParameters.BackBufferWidth - log_width - log_margin, 300);
            Point log_size = new Point(log_width, 700);
            var rect = new Rectangle(log_position, log_size);
            // Draw directly via the SpriteBatch instance bypassing y-axis flip
            GraphicsService.Instance.Draw(GameContent.Instance.whitePixel, rect, Color.Gray * 0.8f);
            foreach (var (message, i) in Game.Logs.Select((x, i) => (x, i)))
            {
                // GraphicsService.Instance.Draw(GameContent.Instance.whitePixel, rect, Color.Gray);
                spriteBatch.DrawString(GameContent.Instance.defaultFont, message, (log_position + new Point(20, 20 + 20 * i)).ToVector2(), Color.White);
            }
            spriteBatch.End();
        }

        public void SetupControls()
        {
            Action noop = delegate { };
            Controls = new Dictionary<Keys, (Action, Action, bool)>()
            {
                {  Keys.A, (Game.WalkLeft, noop, true) },
                {  Keys.D, (Game.WalkRight, noop, true) },
                {  Keys.S, (Game.GoDown, noop, true) },
                {  Keys.W, (Game.GoUp, noop, true) },
                {  Keys.Space, (Game.Jump, Game.StopJumping, false) },
                {  Keys.I, (Game.OpenInventory, noop, false) },
                {  Keys.Z, (Game.Fire, noop, false) },
                {  Keys.X, (Game.Swing, noop, false) },
                {  Keys.C, (Game.OpenChar, noop, false) },
                {  Keys.E, (Game.OpenChest, noop, false) },
                {  Keys.R, (Game.ToggleTestUI, noop, false) },
                {  Keys.OemMinus, (Game.ZoomOut, noop, true) },
                {  Keys.OemPlus, (Game.ZoomIn, noop, true) },
                {  Keys.F11, (Game.OpenEditor, noop, true) }
            };
        }

        public override void HandleControls()
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
        }
    }
}
