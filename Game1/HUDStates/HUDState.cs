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
        public Point? LastPosition { get; set; }
    }

    public abstract class HUDState
    {
        // Internal flags
        // tracks key press/release
        public static Dictionary<Keys, bool> release_map = new Dictionary<Keys, bool>();
        protected bool lmb_pressed;
        protected bool rmb_pressed;
        static int last_scroll_value;
        protected Point mouse_pos;
        public static Point? last_click_pos;
        ViewControl captured_element;

        public List<string> status_messages = new List<string>();
        protected Game1 Game => GameService.Instance;
        // Root window
        public Root Root { get; set; }

        // Events
        protected event EventHandler<MouseEventArgs> MouseMove = delegate { };
        protected event EventHandler<MouseEventArgs> MouseUp = delegate { };
        protected event EventHandler<MouseEventArgs> MouseDown = delegate { };
        protected event EventHandler<MouseEventArgs> MouseWheelUp = delegate { };
        protected event EventHandler<MouseEventArgs> MouseWheelDown = delegate { };
        // Keyboard controls
        public Dictionary<Keys, (Action, Action, bool)> Controls { get; set; } = new Dictionary<Keys, (Action, Action, bool)>();

        // public static GameObject MouseStorage { get; set; }

        public virtual void Draw()
        {
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
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

        // Update camera offset based on player position
        public void AdjustCamera()
        {
            var pos = (PositionComponent)Game.Player;
            var direction = pos.WorldPosition.Center - Game.RenderSystem.Camera.Position;
            float min_camera_speed = 1f;
            float max_camera_speed = direction.Length() / 8;
            float camera_speed = Math.Max(Math.Min(min_camera_speed, direction.Length()), max_camera_speed);
            if (direction.Length() > 1)
                direction.Normalize();
            Game.RenderSystem.Camera.Position = Game.RenderSystem.Camera.Position + direction * camera_speed;
        }

        public virtual void Tick()
        {
            AdjustCamera();
        }

        public virtual void HandleControls()
        {
            HandleKeyboard();
            HandleMouseEvents();
        }

        public HUDState()
        {
            Root = new Root();
        }

        public virtual void RegisterChildren() { }

        protected void SetupGUI()
        {
            RegisterChildren();
            Root.RegisterChild(new StatusView(this));
            Root.Node.CalculateLayout();
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
                if (!lmb_pressed && !rmb_pressed)
                    Root.onMouseMove(mouse.Position);
                else
                    captured_element.onMouseMove(mouse.Position - captured_element?.Parent?.Position ?? Point.Zero);

                mouse_pos = mouse.Position;
            }

            if(mouse.ScrollWheelValue != last_scroll_value)
            {
                if (mouse.ScrollWheelValue > last_scroll_value)
                    MouseWheelUp(this, new MouseEventArgs());
                else
                    MouseWheelDown(this, new MouseEventArgs());
                last_scroll_value = mouse.ScrollWheelValue;
            }

            // Left mouse button
            if (mouse.LeftButton == ButtonState.Pressed && !lmb_pressed)
            {
                if(!lmb_pressed && !rmb_pressed)
                    captured_element = Root.onMouseDown(MouseButton.Left, mouse.Position);
                else
                {
                    captured_element.onMouseDown(MouseButton.Left, mouse.Position);
                }
                lmb_pressed = true;
            }
            if (mouse.LeftButton == ButtonState.Released && lmb_pressed)
            {
                lmb_pressed = false;
                captured_element.onMouseUp(MouseButton.Left, mouse.Position);
            }

            // Right mouse button
            if (mouse.RightButton == ButtonState.Pressed && !rmb_pressed)
            {
                if (!lmb_pressed && !rmb_pressed)
                    captured_element = Root.onMouseDown(MouseButton.Right, mouse.Position);
                else
                {
                    captured_element.onMouseDown(MouseButton.Right, mouse.Position);
                }
                rmb_pressed = true;
            }
            if (mouse.RightButton == ButtonState.Released && rmb_pressed)
            {
                rmb_pressed = false;
                captured_element.onMouseUp(MouseButton.Right, mouse.Position);
            }
        }
    }
}
