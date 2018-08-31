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

        // mouse position on last tick
        Point last_position = Point.Zero;
        // object currently being mouse-dragged
        GameObject tele_obj = null;

        Dictionary<string, (Texture2D, Vector2, bool, Color?)> textures = new Dictionary<string, (Texture2D, Vector2, bool, Color?)>()
        {
            { "Ladder", (GameContent.Instance.ladder, new Vector2(0.5f, 0.5f), true, Color.White) },
            { "Chest", (null, new Vector2(0.5f, 0.5f), false, Color.Firebrick) }
        };

        public EditorHUDState(HUDContainer hud)
        {
            playerHUD = hud;
            SetupControls();
            InitObjectConstructors();
            RegisterHandlers();
        }

        public void RegisterHandlers()
        {
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            DragObject();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                if (CurrentConstructor != null)
                    ApplyConstructor();
                else
                    StartDragging();
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
                StopDragging();
            else
                DeleteObject();
        }

        public override void Draw()
        {
            playerHUD.Draw();
            if (CurrentConstructor != null)
                DrawCurrentBlock();
            DrawLogger();
            base.Draw();
        }

        public void InitObjectConstructors()
        {
            PositionalConstructors = new Dictionary<string, Func<Vector2, Vector2, Vector2, GameObject>>()
            {
                { "SolidPlatform", (coords, halfsize, origin) => { return new SolidPlatform(coords, halfsize, origin); } },
                { "Liquid", (coords, halfsize, origin) => { return new Liquid(coords, halfsize, origin); } },
                { "ForegroundQuad", (coords, halfsize, origin) => { return new ForegroundQuad(coords, halfsize, origin); } },
                { "Ladder", (coords, halfsize, origin) => { return new Ladder(coords, halfsize); } },
                { "Zombie", (coords, halfsize, origin) => { return new Zombie(coords); } },
                { "Goblin", (coords, halfsize, origin) => { return new Goblin(coords); } },
                { "GoblinShaman", (coords, halfsize, origin) => { return new GoblinShaman(coords); } },
                { "Chest", (coords, halfsize, origin) => { return new Chest(coords, halfsize); } },
            };
            // CurrentConstructor = PositionalConstructors.Keys.First();
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
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null);
            var mouse_pos = Mouse.GetState().Position;
            mouse_pos = new Point(mouse_pos.X, -mouse_pos.Y);
            // var rect = new Rectangle(Mouse.GetState().Position, new Point((int)(current_block_width * Game.RenderSystem.Camera.Zoom), (int)(current_block_height * Game.RenderSystem.Camera.Zoom)));
            var rect = new Rectangle(Mouse.GetState().Position, new Point((int)(current_block_width), (int)(current_block_height)));
            // rect = Game.GameToScreen(rect, new Vector2(0, 1));

            textures.TryGetValue(CurrentConstructor, out (Texture2D tex, Vector2 origin, bool tiled, Color? color) t);
            var tex = t.tex ?? GameContent.Instance.whitePixel;
            var origin = t.origin;
            var tiled = t.tiled;
            var color = t.color ?? Color.White;

            GraphicsService.DrawScreen(tex, rect, color, 0, origin, scale: Game.RenderSystem.Camera.Zoom, tiled: tiled);
            // GraphicsService.DrawScreen(tex, rect, Color.Gray, 0, new Vector2(0.5f, 0.5f), scale: 1, tiled: tiled);
            spriteBatch.End();
        }

        int increment = 8;
        public void IncreaseWidth() => current_block_width += increment;
        public void DecreaseWidth() => current_block_width -= increment;
        public void IncreaseHeight() => current_block_height += increment;
        public void DecreaseHeight() => current_block_height -= increment;

        public void SetupControls()
        {
            bool continuous_size = true;
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
                {  Keys.C, (Game.OpenChest, noop, false) },
                {  Keys.Q, (SetPrevConstructor, noop, false) },
                {  Keys.E, (SetNextConstructor, noop, false) },
                {  Keys.R, (ClearConstructor, noop, false) },
                {  Keys.OemMinus, (Game.ZoomOut, noop, true) },
                {  Keys.OemPlus, (Game.ZoomIn, noop, true) },
                {  Keys.Home, (IncreaseHeight, noop, continuous_size) },
                {  Keys.End, (DecreaseHeight, noop, continuous_size) },
                {  Keys.PageUp, (IncreaseWidth, noop, continuous_size) },
                {  Keys.Insert, (DecreaseWidth, noop, continuous_size) },
                {  Keys.Back, (Game.CloseEditor, noop, true) },

                {  Keys.NumPad7, (PrevGroup, noop, false) },
                {  Keys.NumPad9, (NextGroup, noop, false) },

                {  Keys.NumPad4, (MoveGroupLeft, noop, true) },
                {  Keys.NumPad6, (MoveGroupRight, noop, true) },
                {  Keys.NumPad8, (MoveGroupUp, noop, true) },
                {  Keys.NumPad2, (MoveGroupDown, noop, true) },
            };
        }

        public int CurrentGroupIndex { get; set; }
        List<List<GameObject>> Groups => Game.Groups;
        public List<GameObject> CurrentGroup => CurrentGroupIndex < Groups.Count ? Groups[CurrentGroupIndex] : null;

        public void NextGroup()
        {
            CurrentGroupIndex++;
            Game.Log($"Current Group: {CurrentGroupIndex}");
            if (CurrentGroup == null)
            {
                Groups.Add(new List<GameObject>());
                // Maybe should switch to dictionary
                // Groups[CurrentGroupIndex] = ;
            }
        }

        public void PrevGroup()
        {
            CurrentGroupIndex = Math.Max(0, CurrentGroupIndex - 1);
            Game.Log($"Current Group: {CurrentGroupIndex}");
            /*
            if (CurrentGroup == null)
            {
                Groups[CurrentGroupIndex] = new List<GameObject>();
            }
            */
        }

        public void MoveGroup(Vector2 d)
        {
            if (CurrentGroup != null)
            {
                foreach (var obj in CurrentGroup)
                {
                    var pos = (PositionComponent)obj;
                    pos.AdjustPosition(d);
                }
            }
        }

        int group_move_speed = 3;

        public void MoveGroupLeft()
        {
            MoveGroup(new Vector2(-group_move_speed, 0));
        }

        public void MoveGroupRight()
        {
            MoveGroup(new Vector2(group_move_speed, 0));
        }

        public void MoveGroupUp()
        {
            MoveGroup(new Vector2(0, group_move_speed));
        }

        public void MoveGroupDown()
        {
            MoveGroup(new Vector2(0, -group_move_speed));
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

        public void ClearConstructor()
        {
            CurrentConstructor = null;
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

        Point initial_position;
        public void StartDragging()
        {
            var mouseState = Mouse.GetState();
            var obj = Game.GetObjectAtCursor();
            if (tele_obj == null && obj != null)
            {
                tele_obj = obj;
                initial_position = mouseState.Position;
            }
        }

        public void DragObject()
        {
            var mouseState = Mouse.GetState();
            if (tele_obj != null)
            {
                if (last_position == Point.Zero)
                {
                    last_position = mouseState.Position;
                }
                else
                {
                    // TODO: extract this funny shit
                    // TODO: too many TODOs
                    var total_dp = (mouseState.Position - initial_position).ToVector2() / Game.RenderSystem.Camera.Zoom;
                    var dp = (mouseState.Position - last_position).ToVector2() / Game.RenderSystem.Camera.Zoom;
                    // Have to manually transform vector for delta between coords
                    dp.Y = -dp.Y;
                    dp.X = (float)Math.Round(dp.X);
                    dp.Y = (float)Math.Round(dp.Y);
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                    {
                        if (Math.Abs(total_dp.X) >= Math.Abs(total_dp.Y))
                        {
                            dp.Y = 0;
                        }
                        else if (Math.Abs(total_dp.X) < Math.Abs(total_dp.Y))
                        {
                            dp.X = 0;
                        }
                    }

                    // TODO: refactor this
                    var tele_pos = (PositionComponent)tele_obj;
                    if (tele_pos != null)
                    {
                        tele_pos.AdjustPosition(dp);
                        // tele_obj.center += dp;
                    }
                    last_position = mouseState.Position;
                }
            }
        }

        public void StopDragging()
        {
            last_position = Point.Zero;
            tele_obj = null;
        }

        float current_block_width = 8;
        float current_block_height = 8;

        public void HandleDragMode()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                DragObject();
            }
            else
            {
                StopDragging();
            }
        }

        public void ApplyConstructor()
        {
            var pos = Mouse.GetState().Position;
            // var (coords, halfsize, origin) = (Game.RenderSystem.ScreenToGame(click_pos), (pos - click_pos).ToVector2() / (2 * Game.RenderSystem.Camera.Zoom), new Vector2(0, 1));
            var click_coords = GetInGameCoords(pos);
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
                CurrentGroup.Add(obj);
                Game.AddToMainScene(obj);
                Game.CurrentLevel.objects.Add(obj);
            }
        }

        public void DeleteObject()
        {
            var obj = Game.GetObjectAtCursor();
            Game.CurrentLevel.objects.Remove(obj);
            foreach (var group in Groups)
            {
                group.Remove(obj);
            }
            obj?.onDestroy();
        }

        public override void HandleControls()
        {
            // TODO: possibly refactor this
            // reset the player's "intention to move" (move_direction) by default as a workaround
            Game.StopMoving();
            base.HandleControls();
        }

    }
}
