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
    public abstract class HUDState
    {
        // tracks key press/release
        public static Dictionary<Keys, bool> release_map = new Dictionary<Keys, bool>();
        public static bool lmb_pressed;
        public static bool rmb_pressed;
        public static Point click_pos;
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

        public virtual void HandleControls() { }

        // public List<ViewControl> Children { get; } = new List<ViewControl>();
        public Root Root { get; set; }

        public HUDState()
        {
            Root = new Root();
        }

        public void HandleMouseEvents()
        {
            var mouse = Mouse.GetState();
            if (mouse.Position != mouse_pos)
            {
                Root.MouseMove(mouse.Position);
                mouse_pos = mouse.Position;
            }
            if (mouse.LeftButton == ButtonState.Pressed && !lmb_pressed)
            {
                lmb_pressed = true;
                Root.onMouseDown(mouse.Position);
            }
            if (mouse.LeftButton == ButtonState.Released && lmb_pressed)
            {
                lmb_pressed = false;
                Root.onMouseUp(mouse.Position);
                /*
                if (MouseStorage == null)
                {
                    var res = Root.onDrag(mouse.Position);
                    if (res != null)
                    {
                        MouseStorage = res;
                    }
                }
                else
                {
                    if (Root.onDrop(mouse.Position, MouseStorage))
                    {
                        MouseStorage = null;
                    }
                }
                */
            }
        }

    }
}
