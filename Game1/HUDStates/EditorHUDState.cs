using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omniplatformer.Characters;
using Omniplatformer.Components;
using Omniplatformer.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUDStates
{
    public class EditorHUDState : HUDState
    {
        HUDContainer playerHUD;
        Game1 Game => GameService.Instance;

        // editor's state
        public Dictionary<string, Func<Vector2, Vector2, Vector2, GameObject>> PositionalConstructors;// = new Dictionary<string, Func<GameObject>>();
        public string CurrentConstructor { get; set; }
        public bool PinMode { get; set; }

        public Dictionary<Keys, (Action, Action, bool)> Controls { get; set; } = new Dictionary<Keys, (Action, Action, bool)>();

        public EditorHUDState(HUDContainer hud)
        {
            playerHUD = hud;
            SetupControls();
            InitObjectConstructors();
        }

        public override void Draw()
        {
            playerHUD.Draw();
            DrawCurrentBlock();
            DrawLogger();
        }

        public void InitObjectConstructors()
        {
            PositionalConstructors = new Dictionary<string, Func<Vector2, Vector2, Vector2, GameObject>>()
            {
                { "SolidPlatform", (coords, halfsize, origin) => { return new SolidPlatform(coords, halfsize, origin); } },
                { "Liquid", (coords, halfsize, origin) => { return new Liquid(coords, halfsize, origin); } },
                { "ForegroundQuad", (coords, halfsize, origin) => { return new ForegroundQuad(coords, halfsize, origin); } },
                { "Zombie", (coords, halfsize, origin) => { return new Zombie(coords); } },
                { "Chest", (coords, halfsize, origin) => { return new Chest(coords, halfsize, new WieldedItem(5)); } },
            };
            CurrentConstructor = PositionalConstructors.Keys.First();
        }

        public void DrawLogger()
        {
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

        public void DrawCurrentBlock()
        {
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            var rect = new Rectangle(Mouse.GetState().Position, new Point((int)(current_block_width * Game.RenderSystem.Camera.Zoom), (int)(current_block_height * Game.RenderSystem.Camera.Zoom)));
            // rect = Game.GameToScreen(rect, new Vector2(0, 1));
            GraphicsService.Instance.Draw(GameContent.Instance.whitePixel, rect, Color.Gray);
            spriteBatch.End();
        }

        int increment = 4;
        public void IncreaseWidth() => current_block_width += increment;
        public void DecreaseWidth() => current_block_width -= increment;
        public void IncreaseHeight() => current_block_height += increment;
        public void DecreaseHeight() => current_block_height -= increment;

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
                {  Keys.O, (Save, noop, false) },
                {  Keys.I, (Game.OpenInventory, noop, false) },
                {  Keys.Z, (Game.Fire, noop, false) },
                {  Keys.X, (Game.Swing, noop, false) },
                {  Keys.C, (Game.OpenChest, noop, false) },
                {  Keys.Q, (SetPrevConstructor, noop, false) },
                {  Keys.E, (SetNextConstructor, noop, false) },
                {  Keys.OemMinus, (Game.ZoomOut, noop, true) },
                {  Keys.OemPlus, (Game.ZoomIn, noop, true) },
                {  Keys.Home, (IncreaseHeight, noop, true) },
                {  Keys.End, (DecreaseHeight, noop, true) },
                {  Keys.PageUp, (IncreaseWidth, noop, true) },
                {  Keys.Insert, (DecreaseWidth, noop, true) },
            };
        }

        public void SetPrevConstructor()
        {
            var keys = PositionalConstructors.Keys.ToList();
            var i = (keys.IndexOf(CurrentConstructor) - 1 + keys.Count) % keys.Count;
            CurrentConstructor = keys[i];
            Game.Log($"Current constructor: {CurrentConstructor}");
            // CurrentConstructor = PositionalConstructors.
        }

        public void SetNextConstructor()
        {
            var keys = PositionalConstructors.Keys.ToList();
            var i = (keys.IndexOf(CurrentConstructor) + 1 + keys.Count) % keys.Count;
            CurrentConstructor = keys[i];
            Game.Log($"Current constructor: {CurrentConstructor}");
            // CurrentConstructor = PositionalConstructors.
        }

        public void Save()
        {
            Game.Log("Saving level");
            Game.CurrentLevel.Save("");
        }

        public Vector2 GetInGameCoords(Point click_position)
        {
            var ingame_pos = Game.RenderSystem.ScreenToGame(click_position);
            if (PinMode || true)
            {
                var obj = Game.GetObjectAtCoords(click_position);
                if (obj == null)
                    return ingame_pos;
                var obj_pos = (PositionComponent)obj;
                var rect = obj_pos.GetRectangle();

                float? current_length = null;
                Vector2? current_pt = null;
                foreach(var pt in obj_pos.GetRectPoints())
                {
                    float length = (pt - ingame_pos).Length();
                    if ((!current_length.HasValue || current_length > length) && length < 60)
                    {
                        current_length = length;
                        current_pt = pt;
                    }
                }
                return current_pt ?? ingame_pos;
            }
            else
            {
                return ingame_pos;
            }
        }

        float current_block_width = 8;
        float current_block_height = 8;

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
                if (!lmb_pressed)
                {
                    var pos = Mouse.GetState().Position;
                    lmb_pressed = true;
                    click_pos = pos;
                }
                //    Game.DragObject();
            }
            else
            {
                if (lmb_pressed)
                {
                    var pos = Mouse.GetState().Position;
                    // var (coords, halfsize, origin) = (Game.RenderSystem.ScreenToGame(click_pos), (pos - click_pos).ToVector2() / (2 * Game.RenderSystem.Camera.Zoom), new Vector2(0, 1));
                    var click_coords = GetInGameCoords(click_pos);
                    // var end_coords = GetInGameCoords(pos);

                    // var halfsize = (end_coords - click_coords) / 2;//.ToVector2() / (2 * Game.RenderSystem.Camera.Zoom);
                    var halfsize = new Vector2(current_block_width / 2, current_block_height / 2);
                    if (halfsize.Length() > 0)
                    {
                        halfsize = new Vector2(Math.Abs(halfsize.X), Math.Abs(halfsize.Y));
                        // var (or_x, or_y) = (pos.X > click_pos.X ? 0 : 1, pos.Y > click_pos.Y ? 1 : 0);
                        // var origin = new Vector2(or_x, or_y);
                        var origin = new Vector2(0, 1);
                        // var obj = new SolidPlatform(coords, halfsize, origin);
                        var obj = PositionalConstructors[CurrentConstructor](click_coords, halfsize, origin);
                        Game.RegisterObject(obj);
                        Game.CurrentLevel.objects.Add(obj);
                    }
                }
                lmb_pressed = false;
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                if (!rmb_pressed)
                {
                    var obj = Game.GetObjectAtCursor();
                    Game.CurrentLevel.objects.Remove(obj);
                    obj?.onDestroy();
                    rmb_pressed = true;
                }
            }
            else
            {
                rmb_pressed = false;
            }
        }
    }
}
