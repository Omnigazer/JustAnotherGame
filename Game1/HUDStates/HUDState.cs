using Microsoft.Xna.Framework;
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
    public enum MouseButton
    {
        None,
        Left,
        Right
    }

    public class MouseEventArgs : EventArgs
    {
        public MouseEventArgs()
        {
            Button = MouseButton.None;
        }

        public MouseEventArgs(MouseButton button, Point pos)
        {
            Button = button;
            Position = pos;
        }
        public MouseButton Button { get; set; }
        public Point Position { get; set; }
    }

    public abstract class HUDState
    {
        // tracks key press/release
        public static Dictionary<Keys, bool> release_map = new Dictionary<Keys, bool>();
        bool lmb_pressed;
        bool rmb_pressed;
        public static Point click_pos;

        public List<string> status_messages = new List<string>();
        protected Game1 Game => GameService.Instance;

        // Events
        protected event EventHandler<MouseEventArgs> MouseMove = delegate { };
        protected event EventHandler<MouseEventArgs> MouseUp = delegate { };
        protected event EventHandler<MouseEventArgs> MouseDown = delegate { };
        // Keyboard controls
        public Dictionary<Keys, (Action, Action, bool)> Controls { get; set; } = new Dictionary<Keys, (Action, Action, bool)>();

        // public static GameObject MouseStorage { get; set; }
        Point mouse_pos;
        public virtual void Draw()
        {
            var spriteBatch = GraphicsService.Instance;
            DrawStatus();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Root.Draw();
            DrawCursor();
            // DrawStorage();
            spriteBatch.End();
        }

        protected virtual void DrawCursor()
        {
            Point cursor_position = Mouse.GetState().Position;
            Point cursor_size = new Point(24, 48);
            var rect = new Rectangle(cursor_position, cursor_size);
            // Draw directly via the SpriteBatch instance bypassing y-axis flip
            GraphicsService.Instance.Draw(GameContent.Instance.cursor, rect, Color.White);
        }

        public virtual IEnumerable<string> GetStatusMessages()
        {
            yield break;
        }

        public virtual void Tick()
        {
            // Update camera offset based on player position
            var pos = (PositionComponent)Game.Player;
            Game.RenderSystem.SetCameraPosition(pos.WorldPosition.Center);
        }

        public void DrawStatus()
        {
            // TODO: TEST
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            int log_width = 500, log_margin = 50;
            Point log_position = new Point(log_margin, 300);
            Point log_size = new Point(log_width, 700);
            var rect = new Rectangle(log_position, log_size);
            int i = 0;
            void displayMessage(string message)
            {
                spriteBatch.DrawString(GameContent.Instance.defaultFont, message, (log_position + new Point(20, 20 + 20 * i++)).ToVector2(), Color.White);
            }
            // Draw directly via the SpriteBatch instance bypassing y-axis flip
            // GraphicsService.Instance.Draw(GameContent.Instance.whitePixel, rect, Color.Gray * 0.8f);
            foreach (var msg in GetStatusMessages())
            {
                displayMessage(msg);
            }
            // displayMessage(String.Format("Current constructor: {0}", CurrentConstructor));
            // displayMessage(String.Format("Current group: {0}", CurrentGroupName));
            // displayMessage(String.Format("Current object: {0}", Game.GetObjectAtCursor()));
            /*
            foreach (var msg in collision_messages)
            {
                displayMessage(msg);
            }
            */
            spriteBatch.End();
        }

        /*
        public virtual void DrawStorage()
        {
            if (MouseStorage != null)
            {
                var mouse_pos = Mouse.GetState().Position;
                GraphicsService.DrawScreen(((RenderComponent)MouseStorage).Texture, new Rectangle(mouse_pos, new Point(60, 60)), Color.White, 0, Vector2.Zero);
            }
        }
        */

        public virtual void HandleControls()
        {
            HandleKeyboard();
            HandleMouseEvents();
        }

        // public List<ViewControl> Children { get; } = new List<ViewControl>();
        public Root Root { get; set; }

        public HUDState()
        {
            Root = new Root();
        }

        public void HandleKeyboard()
        {
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
                    if (release_map.ContainsKey(key) && !release_map[key])
                        released_action?.Invoke();
                    release_map[key] = true;
                }
            }
        }

        public void HandleMouseEvents()
        {
            var mouse = Mouse.GetState();
            if (mouse.Position != mouse_pos)
            {
                Root.MouseMove(mouse.Position);
                mouse_pos = mouse.Position;
                MouseMove(this, new MouseEventArgs());
            }

            // Left mouse button
            if (mouse.LeftButton == ButtonState.Pressed && !lmb_pressed)
            {
                lmb_pressed = true;
                Root.onMouseDown(mouse.Position);
                MouseDown(this, new MouseEventArgs(MouseButton.Left, mouse.Position));
            }
            if (mouse.LeftButton == ButtonState.Released && lmb_pressed)
            {
                lmb_pressed = false;
                Root.onMouseUp(mouse.Position);
                MouseUp(this, new MouseEventArgs(MouseButton.Left, mouse.Position));
            }

            // Right mouse button
            if (mouse.RightButton == ButtonState.Pressed && !rmb_pressed)
            {
                rmb_pressed = true;
                Root.onMouseDown(mouse.Position);
                MouseDown(this, new MouseEventArgs(MouseButton.Right, mouse.Position));
            }
            if (mouse.RightButton == ButtonState.Released && rmb_pressed)
            {
                rmb_pressed = false;
                Root.onMouseUp(mouse.Position);
                MouseUp(this, new MouseEventArgs(MouseButton.Right, mouse.Position));
            }
        }

    }
}
