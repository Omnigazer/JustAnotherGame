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
            HandleKeyBoard();
            HandleMouseEvents();
        }

        // public List<ViewControl> Children { get; } = new List<ViewControl>();
        public Root Root { get; set; }

        public HUDState()
        {
            Root = new Root();
        }

        public void HandleKeyBoard()
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
                    release_map[key] = true;
                    released_action?.Invoke();
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
