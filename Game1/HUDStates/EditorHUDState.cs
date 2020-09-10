using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omniplatformer.Components;
using Omniplatformer.Scenes;
using Omniplatformer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Objects.Interactibles;
using Omniplatformer.Objects.Terrain;
using Omniplatformer.Scenes.Subsystems;
using Omniplatformer.Services;
using Omniplatformer.Utility.DataStructs;
using Omniplatformer.Views.Editor;
using Omniplatformer.Views.HUD;
using Omniplatformer.Utility.Extensions;

namespace Omniplatformer.HUDStates
{
    public class EditorHUDState : HUDState
    {
        // editor's state
        public Dictionary<string, Func<Vector2, Vector2, Vector2, GameObject>> PositionalConstructors { get; set; }
        public string CurrentConstructor { get; set; }
        public bool PinMode { get; set; }
        public int current_tile = 2;
        int brush_size = 1;
        float current_block_width = 8;
        float current_block_height = 8;
        // whether the current tile applies to background
        public bool background = true;

        // mouse position on last tick
        Point last_position = Point.Zero;
        // object currently being mouse-dragged
        GameObject tele_obj = null;

        Dictionary<string, (Texture2D, Vector2?, bool, Color?)> textures = new Dictionary<string, (Texture2D, Vector2?, bool, Color?)>()
        {
            { "Ladder", (GameContent.Instance.ladder, new Vector2(0.5f, 0.5f), true, Color.White) },
            { "Chest", (null, new Vector2(0.5f, 0.5f), false, Color.Firebrick) }
        };

        public override void Tick()
        {
            if (brush_down)
                PlaceTiles();
            else if (eraser_down)
                PlaceTiles(0);
        }

        public EditorHUDState()
        {
            SetupControls();
            InitObjectConstructors();
            RegisterHandlers();
            var picker = new TilePicker();
            Root.RegisterChild(new HUDContainer());
            Root.RegisterChild(picker);
            SetupGUI();
        }

        public void RegisterHandlers()
        {
            Root.MouseDown += OnMouseDown;
            Root.MouseUp += OnMouseUp;
            Root.MouseMove += OnMouseMove;
            MouseWheelUp += EditorHUDState_MouseWheelUp;
            MouseWheelDown += EditorHUDState_MouseWheelDown;
        }

        private void EditorHUDState_MouseWheelDown(object sender, MouseEventArgs e)
        {
            ShrinkBrush();
        }

        private void EditorHUDState_MouseWheelUp(object sender, MouseEventArgs e)
        {
            EnlargeBrush();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            DragObject();
        }

        bool brush_down = false;
        bool eraser_down = false;
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                if (CurrentConstructor != null)
                    ApplyConstructor();
                else
                {
                    brush_down = true;
                }
            }
            else
            {
                eraser_down = true;
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                brush_down = false;
                StopDragging();
            }
            else
            {
                eraser_down = false;
                DeleteObject();
            }
        }

        public override void Draw()
        {
            if (CurrentConstructor != null)
                DrawCurrentBlock();
            else
                DrawCurrentTile();
            DrawLogger();
            // DrawStatus();
            base.Draw();
        }

        public void InitObjectConstructors()
        {
            PositionalConstructors = new Dictionary<string, Func<Vector2, Vector2, Vector2, GameObject>>()
            {
                { "SolidPlatform", (coords, halfsize, origin) => SolidPlatform.Create(coords, halfsize)},
                { "MovingPlatform", (coords, halfsize, origin) => MovingPlatform.Create(coords, halfsize)},
                { "DestructibleObject", (coords, halfsize, origin) => DestructibleObject.Create(coords, halfsize)},
                { "Liquid", (coords, halfsize, origin) => Liquid.Create(coords, halfsize)},
                { "ForegroundQuad", (coords, halfsize, origin) => ForegroundQuad.Create(coords, halfsize)},
                { "Ladder", (coords, halfsize, origin) => Ladder.Create(coords, halfsize)},
                { "Goblin", (coords, halfsize, origin) => Goblin.Create(coords) },
                { "GoblinShaman", (coords, halfsize, origin) => GoblinShaman.Create(coords)},
                { "Chest", (coords, halfsize, origin) => Chest.Create(coords, halfsize)},
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

        public override IEnumerable<string> GetStatusMessages()
        {
            yield return $"Current constructor: {CurrentConstructor}";
            yield return $"Current group: {CurrentGroupName}";
            yield return $"Current object: {Game.GetObjectAtCursor()}";
            foreach (var msg in StatusMessages)
            {
                yield return msg;
            }
        }

        public void DrawCurrentBlock()
        {
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null);
            var rect = new Rectangle(Mouse.GetState().Position, new Point((int)(current_block_width), (int)(current_block_height)));
            // rect = Game.GameToScreen(rect, new Vector2(0, 1));

            textures.TryGetValue(CurrentConstructor, out (Texture2D tex, Vector2? origin, bool tiled, Color? color) t);
            var tex = t.tex ?? GameContent.Instance.whitePixel;
            var origin = t.origin ?? Position.DefaultOrigin;
            var tiled = t.tiled;
            var color = t.color ?? Color.White;

            GraphicsService.DrawScreen(tex, rect, color, 0, origin, scale: Game.RenderSystem.Camera.Zoom, tiled: tiled);
            spriteBatch.End();
        }

        #region Tiles
        public void DrawCurrentTile()
        {
            Vector2 mouse_coords = Game.RenderSystem.ScreenToGame(mouse_pos);
            var x = (int)mouse_coords.X / PhysicsSystem.TileSize;
            var y = (int)mouse_coords.Y / PhysicsSystem.TileSize;
            for (int i = x; i < x + brush_size; i++)
            {
                for (int j = y; j < y + brush_size; j++)
                {
                    DrawTile(i, j);
                }
            }
        }
        public void DrawTile(int x, int y)
        {
            if (GameContent.Instance.atlas_meta.ContainsKey((short)current_tile))
            {
                var spriteBatch = GraphicsService.Instance;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, Game.RenderSystem.Camera.TranslationMatrix);

                Vector2 mouse_coords = new Vector2(x * PhysicsSystem.TileSize, y * PhysicsSystem.TileSize);
                var rect = new Rectangle(mouse_coords.ToPoint(), new Point(PhysicsSystem.TileSize));
                var tex = GameContent.Instance.atlas;
                var source_rect = GameContent.Instance.atlas_meta[(short)current_tile];
                GraphicsService.DrawGame(tex, rect, Color.White, 0, new Vector2(0.0f, 1f), source_rect: source_rect);
                spriteBatch.End();
            }
        }
        public void EnlargeBrush()
        {
            brush_size++;
        }

        public void ShrinkBrush()
        {
            brush_size = Math.Max(0, brush_size - 1);
        }

        public void PlaceTiles(short? type = null)
        {
            type = (short)(type ?? current_tile);
            var click_coords = GetInGameCoords(mouse_pos);
            var (x, y) = Game.PhysicsSystem.GetTileIndices(click_coords);
            bool rebuild = false;
            for (int i = x; i < x + brush_size; i++)
            {
                for (int j = y; j < y + brush_size; j++)
                {
                    rebuild = PlaceTile(i, j, type.Value) || rebuild;
                }
            }
            if (rebuild)
            {
                var tilemap = Game.MainScene.TileMap;
                var drawable = (TileMapRenderComponent)tilemap;
                drawable.RebuildBuffers(true);
            }
        }

        public bool PlaceTile(int x, int y, short type)
        {
            // var tile = new Tile() { Col = y, Row = x, Type = (type, 0)};
            var tilemap = Game.MainScene.TileMap;
            var tile_type = background ? (tilemap.Grid[x, y].Item1, type) : (type, tilemap.Grid[x, y].Item2);
            var tile = new Tile() { Col = y, Row = x, Type = tile_type };
            if (tilemap.Grid[x, y] != tile.Type)
            {
                tilemap.RegisterTile(tile);
                return true;
            }
            return false;
        }

        public void RemoveTile()
        {
            var pos = Mouse.GetState().Position;
            var click_coords = GetInGameCoords(pos);
            var (x, y) = Game.PhysicsSystem.GetTileIndices(click_coords);
            var tile = new Tile() { Col = y, Row = x };
            Game.MainScene.TileMap.RemoveTile(tile);
            var drawable = (TileMapRenderComponent)Game.MainScene.TileMap;
            drawable.RebuildBuffers(true);
        }
        #endregion


        int increment = 8;
        public void IncreaseWidth() => current_block_width += increment;
        public void DecreaseWidth() => current_block_width -= increment;
        public void IncreaseHeight() => current_block_height += increment;
        public void DecreaseHeight() => current_block_height -= increment;

        public void SetupControls()
        {
            const bool continuous_size = true;
            Action noop = delegate { };
            Controls = new Dictionary<Keys, (Action, Action, bool)>()
            {
                {  Keys.Pause, (Game.TogglePause, noop, false) },
                {  Keys.A, (MoveCameraLeft, noop, true) },
                {  Keys.D, (MoveCameraRight, noop, true) },
                {  Keys.S, (MoveCameraDown, noop, true) },
                {  Keys.W, (MoveCameraUp, noop, true) },

                {  Keys.Q, (SetPrevConstructor, noop, false) },
                {  Keys.E, (SetNextConstructor, noop, false) },
                {  Keys.R, (ClearConstructor, noop, false) },
                {  Keys.OemMinus, (Game.ZoomOut, noop, true) },
                {  Keys.OemPlus, (Game.ZoomIn, noop, true) },
                {  Keys.Home, (IncreaseHeight, noop, continuous_size) },
                {  Keys.End, (DecreaseHeight, noop, continuous_size) },
                {  Keys.PageUp, (IncreaseWidth, noop, continuous_size) },
                {  Keys.Insert, (DecreaseWidth, noop, continuous_size) },
                {  Keys.F11, (Game.CloseEditor, noop, false) },

                /*
                {  Keys.NumPad7, (PrevGroup, noop, false) },
                {  Keys.NumPad9, (NextGroup, noop, false) },
                */

                {  Keys.NumPad4, (MoveGroupLeft, noop, true) },
                {  Keys.NumPad6, (MoveGroupRight, noop, true) },
                {  Keys.NumPad8, (MoveGroupUp, noop, true) },
                {  Keys.NumPad2, (MoveGroupDown, noop, true) },
            };
        }

        public string CurrentGroupName { get; set; } = "default";
        Dictionary<string, List<GameObject>> Groups => Game.Groups;
        public List<GameObject> CurrentGroup => CurrentGroupName != null && Groups.ContainsKey(CurrentGroupName) ? Groups[CurrentGroupName] : null;

        public void SetGroup(string name)
        {
            CurrentGroupName = name;
            if (!Groups.ContainsKey(name))
                Groups.Add(name, new List<GameObject>());
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

        float cam_speed = 15;

        public void MoveCameraLeft()
        {
            Game.RenderSystem.Camera.Position += new Vector2(-cam_speed, 0);
        }

        public void MoveCameraRight()
        {
            Game.RenderSystem.Camera.Position += new Vector2(cam_speed, 0);
        }

        public void MoveCameraUp()
        {
            Game.RenderSystem.Camera.Position += new Vector2(0, cam_speed);
        }

        public void MoveCameraDown()
        {
            Game.RenderSystem.Camera.Position += new Vector2(0, -cam_speed);
        }

        public Vector2 GetInGameCoords(Point click_position)
        {
            var ingame_pos = Game.RenderSystem.ScreenToGame(click_position);
            if (PinMode && false)
            {
                var obj = Game.PhysicsSystem.GetObjectAtCoords(ingame_pos);
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
            var click_coords = GetInGameCoords(pos);
            var halfsize = new Vector2(current_block_width / 2, current_block_height / 2);
            if (halfsize.Length() > 0)
            {
                halfsize = new Vector2(Math.Abs(halfsize.X), Math.Abs(halfsize.Y));
                var origin = new Vector2(0, 1);
                Game.Log(click_coords.ToString());
                var obj = PositionalConstructors[CurrentConstructor](click_coords, halfsize, origin);
                CurrentGroup.Add(obj);
                Game.AddToMainScene(obj);
            }
        }

        public void DeleteObject()
        {
            var obj = Game.GetObjectAtCursor();
            var coords = Game.RenderSystem.ScreenToGame(Mouse.GetState().Position);
            if (obj != null)
            {
                Game.RemoveFromMainScene(obj);
                foreach (var (name, group) in Groups)
                {
                    group.Remove(obj);
                }
            }
            else
                RemoveTile();
        }
    }
}
