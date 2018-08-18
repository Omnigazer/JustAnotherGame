using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omniplatformer.Characters;
using Omniplatformer.Components;
using Omniplatformer.HUD;
using System;
using System.Collections.Generic;
using Omniplatformer.HUDStates;
using Microsoft.Xna.Framework.Media;
// using MonoGameConsole;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Debug;
using EmptyKeys.UserInterface.Generated;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Media;
using EmptyKeys.UserInterface.Media.Effects;
using EmptyKeys.UserInterface.Media.Imaging;
using GameUILibrary;
using EmptyKeys.UserInterface.Renderers;
using EmptyKeys.UserInterface.Controls;

namespace Omniplatformer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // Graphics objects
        public GraphicsDeviceManager graphics;
        public RenderSystem RenderSystem { get; set; }

        // Game objects
        public Player player;
        public Level CurrentLevel { get; set; }
        // possibly should put this into the player's class
        public List<GameObject> objects = new List<GameObject>();
        // public List<Character> characters = new List<Character>();
        public List<Projectile> projectiles = new List<Projectile>();

        // Editor groups
        public List<List<GameObject>> Groups { get; set; } = new List<List<GameObject>>() { new List<GameObject>() };

        // HUD states & controls
        public HUDState HUDState { get; set; }
        HUDState defaultHUD;
        HUDState inventoryHUD;
        HUDState editorHUD;
        HUDState charHUD;
        bool game_over;

        // public GameConsole console;

        private MainWindow basicUI;
        // private Root root;
        public MainWindow root;
        private DebugViewModel debug;
        private BasicUIViewModel viewModel;

        public List<string> Logs { get; set; } = new List<string>() { "test" };

        // Events
        public event EventHandler<InventoryEventArgs> onTargetInventoryOpen = delegate { };
        public event EventHandler onTargetInventoryClosed = delegate { };

        // mouse position on last tick
        Point last_position = Point.Zero;
        // object currently being mouse-dragged
        GameObject tele_obj = null;
        Engine engine;

        int CurrentSongIndex { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 2560;
            graphics.PreferredBackBufferHeight = 1440;
            graphics.DeviceCreated += onDeviceCreated;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
        }

        private void onDeviceCreated(object sender, EventArgs e)
        {
            engine = new MonoGameEngine(GraphicsDevice, 2560, 1440);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
            RenderSystem = new RenderSystem(this);
            // FontManager.Instance.LoadFonts(Content);
            InitServices();
            var playerHUD = new HUDContainer();
            InitGameObjects();

            defaultHUD = new DefaultHUDState(playerHUD);
            inventoryHUD = new InventoryHUDState(playerHUD, player.inventory);
            charHUD = new CharHUDState(playerHUD);
            editorHUD = new EditorHUDState(playerHUD);
            HUDState = defaultHUD;
        }

        public void InitServices()
        {
            GameService.Init(this);
        }

        void InitGameObjects()
        {
            CurrentLevel = GameContent.Instance.level;
            CurrentLevel.Load("blank");

            // Register player
            player = new Player(
                new Vector2(100, 500),
                // new Vector2(110, 192)
                // new Vector2(110, 192)
                new Vector2(15, 27)
                // new Vector2(11, 19.2f)
            );
            RenderSystem.drawables.Add((RenderComponent)player);
            player._onDestroy += GameOver;

            foreach (var obj in GameContent.Instance.level.objects)
            {
                RegisterObject(obj);
            }
            foreach (var character in GameContent.Instance.level.characters)
            {
                RegisterObject(character);
            }

            LoadGroup("village", new Vector2(1300, 0));
        }

        private void GameOver(object sender, EventArgs e)
        {
            game_over = true;
            RenderSystem.drawables.Remove((RenderComponent)player);
        }

        public void Log(string message)
        {
            Logs.Add(message);
        }

        public void ToggleTestUI()
        {
            if (root.Visibility == Visibility.Visible)
                root.Visibility = Visibility.Hidden;
            else
                root.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            IsMouseVisible = true;
            // Create a new SpriteBatch, which can be used to draw textures.
            var spriteBatch = new SpriteBatch(GraphicsDevice);
            GraphicsService.Init(spriteBatch, this);
            GameContent.Init(Content);
            FontManager.DefaultFont = engine.Renderer.CreateFont(GameContent.Instance.defaultFont);

            Viewport viewport = GraphicsDevice.Viewport;
            // var y = InputManager.Current;
            // UIRoot x = new UIRoot(viewport.Width, viewport.Height);
            // basicUI = new MainWindow(viewport.Width, viewport.Height);
            // root = new Root();
            root = new MainWindow();
            FontManager.Instance.LoadFonts(Content);
            ImageManager.Instance.LoadImages(Content);
            SoundManager.Instance.LoadSounds(Content);
            EffectManager.Instance.LoadEffects(Content, "Effects");
            // var x = EffectManager.Instance.GetEffect("distorteffect");
            // var y = x.GetNativeEffect();
            // basicUI = new MainWindow();
            viewModel = new BasicUIViewModel();
            viewModel.ItemCount = 60;
            root.DataContext = viewModel;

            // viewModel.Tetris = new TetrisController(basicUI.TetrisContainer, basicUI.TetrisNextContainer);
            // basicUI.DataContext = viewModel;


            // debug = new DebugViewModel(basicUI);

            // TODO: use this.Content to load your game content here

            // LoadConsole(spriteBatch);
            // Window.Handle
        }

        /*
        void LoadConsole(SpriteBatch spriteBatch)
        {
            System.Windows.Forms.Form winGameWindow = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);
            winGameWindow.Show();
            winGameWindow.Hide();
            var x = System.Windows.Forms.Application.OpenForms;
            //System.Windows.Forms.Application.
            // System.Windows.Forms.Form form;
            // form.
            Services.AddService(typeof(SpriteBatch), spriteBatch);
            console = new GameConsole(this, spriteBatch, new GameConsoleOptions
            {
                ToggleKey = 192,
                Font = Content.Load<SpriteFont>("ConsoleFont"),
                FontColor = Color.LawnGreen,
                Prompt = "~>",
                PromptColor = Color.Crimson,
                CursorColor = Color.OrangeRed,
                BackgroundColor = new Color(Color.Black, 150),
                PastCommandOutputColor = Color.Aqua,
                BufferColor = Color.Gold
            });

            console.AddCommand("ping", a =>
            {
                // TODO your logic
                return String.Format("pong");
            });

            console.AddCommand("merge", a =>
            {
                if (a.Length == 2 && int.TryParse(a[0], out int first) && int.TryParse(a[1], out int second))
                {
                    if (Groups.Count > Math.Max(first, second))
                    {
                        foreach(var obj in Groups[second])
                        {
                            Groups[first].Add(obj);
                        }
                        return String.Format("Merged group {0} into {1}", second, first);
                    }
                    else
                    {
                        return "Index out of bounds";
                    }
                }
                return String.Format("invalid args");
            });

            console.AddCommand("savelevel", a =>
            {
                if (a.Length == 1)
                {
                    var name = a[0];
                    SaveLevel(name);
                    return ("Level saved.");
                }
                else
                    return String.Format("invalid args");
            });

            console.AddCommand("clearlevel", a =>
            {
                ClearCurrentLevel();
                return "Level cleared.";
            });

            console.AddCommand("savegroup", a =>
            {
                if (a.Length == 2 && int.TryParse(a[0], out int index))
                {
                    string name = a[1];
                    SaveGroup(index, name);
                    return "Group saved.";
                }
                else
                    return String.Format("invalid args");
            });

            console.AddCommand("loadgroup", a =>
            {
                if (a.Length == 1)
                {
                    LoadGroup(a[0]);
                    return "Loaded.";
                }
                else
                    return String.Format("invalid args");
            });
        }
        */

        public void SaveLevel(string name)
        {
            Log("Saving level");
            string path = String.Format("{0}.json", name);
            CurrentLevel.Save(path);
        }

        public void ClearCurrentLevel()
        {
            // Clear everything
            objects.Clear();
            RenderSystem.drawables.Clear();
            CurrentLevel.objects.Clear();

            // Register the player back
            RenderSystem.drawables.Add((RenderComponent)player);
        }

        /// <summary>
        /// Load group of objects, or "location"
        /// </summary>
        /// <param name="name"></param>
        public void LoadGroup(string name, Vector2? origin = null)
        {
            Log(String.Format("Loading group '{0}'", name));
            string path = String.Format("Content/Data/{0}.json", name);

            var group = GameContent.Instance.level.LoadGroup(path, origin ?? ((PositionComponent)player).WorldPosition.Coords);
            foreach (var obj in group)
            {
                RegisterObject(obj);
                CurrentLevel.objects.Add(obj);
            }
            Groups.Add(group);
        }

        public void SaveGroup(int index, string name)
        {
            Log(String.Format("Saving current group {0}", index));
            string path = String.Format("{0}.json", name);
            CurrentLevel.SaveGroup(Groups[index], path);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.LeftAlt) && Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Q))
                // Exit();
            StopMoving();
            if (!game_over && IsActive)
                HUDState.HandleControls();
            Simulate();
            // var song = GameContent.Instance.vampireKiller;
            if (MediaPlayer.State != MediaState.Playing && false)
            {
                MediaPlayer.Volume = 0.1f;
                MediaPlayer.Play(GameContent.Instance.Songs[CurrentSongIndex]);
                CurrentSongIndex = (CurrentSongIndex + 1) % GameContent.Instance.Songs.Count;
            }

            // basicUI.Draw(gameTime.ElapsedGameTime.Milliseconds);
            root.UpdateInput(gameTime.ElapsedGameTime.TotalMilliseconds);
            viewModel.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            root.UpdateLayout(gameTime.ElapsedGameTime.TotalMilliseconds);

            base.Update(gameTime);
        }

        public void RegisterObject(GameObject obj)
        {
            objects.Add(obj);
            RenderSystem.RegisterDrawable(obj);
            obj._onDestroy += GameObject_onDestroy;
        }

        private void GameObject_onDestroy(object sender, EventArgs e)
        {
            var obj = (GameObject)sender;
            objects.Remove(obj);
        }

        public void RegisterObject(Projectile projectile)
        {
            projectiles.Add(projectile);
            RenderSystem.RegisterDrawable(projectile);
            projectile._onDestroy += Projectile_onDestroy;
        }

        private void Projectile_onDestroy(object sender, EventArgs e)
        {
            var projectile = (Projectile)sender;
            projectiles.Remove(projectile);
        }

        /*
        public void RegisterObject(Character character)
        {
            characters.Add(character);
            RenderSystem.RegisterDrawable(character);
            character._onDestroy += Character_onDestroy;
        }

        private void Character_onDestroy(object sender, EventArgs e)
        {
            var character = (Character)sender;
            characters.Remove(character);
        }
        */

        public void Simulate()
        {
            RenderSystem.Tick();
            var player_collisions = new List<(Direction, GameObject)>();
            var player_pos = (PositionComponent)player;
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                var obj = objects[i];
                var collision_direction = player_pos.Collides(obj);
                if (collision_direction != Direction.None)
                    player_collisions.Add((collision_direction, obj));
            }
            // for (int j = characters.Count - 1; j >= 0; j--)
            for (int j = objects.Count - 1; j >= 0; j--)
            {
                var collisions = new List<(Direction, GameObject)>();
                var obj = objects[j];
                var pos = (PositionComponent)obj;
                Direction collision_direction;

                foreach (var other_obj in objects)
                {
                    // TODO: look into how we access components
                    collision_direction = pos.Collides(other_obj);
                    if (collision_direction != Direction.None)
                        collisions.Add((collision_direction, other_obj));
                }

                collision_direction = pos.Collides(player);
                // TODO: collision with a player should trump / complement other collisions here
                if (collision_direction != Direction.None)
                    collisions.Add((collision_direction, player));

                // TODO: move this into the tick
                var obj_movable = (MoveComponent)obj;
                obj_movable?.ProcessCollisionInteractions(collisions);
                obj_movable?.Move();

                obj.Tick();
            }

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var projectile_collisions = new List<(Direction, GameObject)>();
                var projectile = projectiles[i];
                var projectile_pos = (PositionComponent)projectile;

                var collision_direction = projectile_pos.Collides(player);
                if (collision_direction != Direction.None)
                    projectile_collisions.Add((collision_direction, player));

                for (int j = objects.Count - 1; j >= 0; j--)
                {
                    var platform = objects[j];
                    collision_direction = projectile_pos.Collides(platform);
                    if (collision_direction != Direction.None)
                        projectile_collisions.Add((collision_direction, platform));
                }

                var movable = (MoveComponent)projectile;
                movable.ProcessCollisionInteractions(projectile_collisions);
                projectile.Tick();
            }

            var player_movable = (MoveComponent)player;
            player_movable.ProcessCollisionInteractions(player_collisions);
            player_movable.Move();
            player.Tick();
        }

        public GameObject GetObjectAtCoords(Point pt)
        {
            var game_click_position = RenderSystem.ScreenToGame(pt);
            foreach (var platform in objects)
            {
                // if (!platform.Draggable)
                // {
                //     continue;
                // }
                var platform_pos = (PositionComponent)platform;
                if (platform_pos.Contains(game_click_position))
                {
                    return platform;
                }
            }
            return null;
        }

        public IEnumerable<GameObject> GetObjectsAroundPosition(Position position, int radius)
        {
            foreach (var obj in objects)
            {
                var pos = (PositionComponent)obj;
                var vector = pos.WorldPosition.Center - position.Center;
                if (vector.Length() < radius)
                {
                    yield return obj;
                }
            }
        }

        public GameObject GetObjectAtCursor()
        {
            return GetObjectAtCoords(Microsoft.Xna.Framework.Input.Mouse.GetState().Position);
        }

        #region Player Actions

        public void OpenInventory()
        {
            if (HUDState != inventoryHUD)
            {
                HUDState = inventoryHUD;
            }
            else
            {
                throw new Exception("Inventory already opened");
            }
        }

        public void CloseInventory()
        {
            if (HUDState == inventoryHUD)
            {
                CloseTargetInventory();
                HUDState = defaultHUD;
            }
            else
            {
                throw new Exception("Called while not in an inventory");
            }
        }

        public void OpenEditor()
        {
            HUDState = editorHUD;
            Log("Editor mode");
        }

        public void CloseEditor()
        {
            HUDState = defaultHUD;
            Log("Default mode");
        }

        public void OpenChar()
        {
            HUDState = charHUD;
            Log("Char mode");
        }

        // TODO: maybe all of these should be "CloseMenu()"
        public void CloseChar()
        {
            HUDState = defaultHUD;
            Log("Default mode");
        }

        public void OpenTargetInventory(Inventory inv)
        {
            onTargetInventoryOpen(this, new InventoryEventArgs(inv));
        }

        public void CloseTargetInventory()
        {
            onTargetInventoryClosed(this, new EventArgs());
        }

        // TODO: refactor this
        public void OpenChest()
        {
            var player_pos = (PositionComponent)player;

            foreach(var obj in objects)
            {
                if (obj is Chest)
                {
                    if (player_pos.Overlaps(obj))
                    {
                        HUDState = inventoryHUD;
                        OpenTargetInventory(((Chest)obj).Inventory);
                        break;
                    }
                }
            }
        }

        public void CloseChest()
        {
            CloseTargetInventory();
        }

        public void WalkLeft()
        {
            var movable = (PlayerMoveComponent)player;
            movable.move_direction = Direction.Left;
        }

        public void WalkRight()
        {
            var movable = (PlayerMoveComponent)player;
            movable.move_direction = Direction.Right;
        }

        public void GoUp()
        {
            var movable = (PlayerMoveComponent)player;
            if (movable.CanClimb)
            {
                movable.StartClimbing();
            }
            movable.move_direction = Direction.Up;
        }

        public void GoDown()
        {
            var movable = (PlayerMoveComponent)player;
            if (movable.CanClimb)
            {
                movable.StartClimbing();
            }
            movable.move_direction = Direction.Down;
        }

        public void StopMoving()
        {
            var movable = (PlayerMoveComponent)player;
            movable.move_direction = Direction.None;
        }

        public void Jump()
        {
            var movable = (PlayerMoveComponent)player;
            movable.Jump();
        }

        public void StopJumping()
        {
            var movable = (PlayerMoveComponent)player;
            movable.StopJumping();
        }

        public void DragObject()
        {
            var obj = GetObjectAtCursor();
            if (tele_obj == null && obj != null)
                tele_obj = obj;
            if (tele_obj != null)
            {
                var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
                if (last_position == Point.Zero)
                {
                    last_position = mouseState.Position;
                }
                else
                {
                    // TODO: extract this funny shit
                    // TODO: too many TODOs
                    var dp = (mouseState.Position - last_position).ToVector2() / RenderSystem.Camera.Zoom;
                    // Have to manually transform vector for delta between coords
                    dp.Y = -dp.Y;
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

            /*
            if (obj != null)
                obj.color = Color.Green;
            */
        }

        public void ReleaseDraggedObject()
        {
            last_position = Point.Zero;
            tele_obj = null;
        }

        public void Fire()
        {
            player.Fire();
        }

        public void Swing()
        {
            player.Swing();
        }

        // fps is assumed to be 30 while we're tick-based
        float zoom_adjust_rate = 0.2f / 30;

        public void ZoomIn()
        {
            RenderSystem.Camera.AdjustZoom(zoom_adjust_rate);
        }

        public void ZoomOut()
        {
            RenderSystem.Camera.AdjustZoom(-zoom_adjust_rate);
        }

        #endregion

        public void SetCameraPosition()
        {
            // Update camera offset based on player position
            var pos = (PositionComponent)player;
            RenderSystem.Camera.Position = pos.WorldPosition.Center;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here

            bool with_light = true, with_foreground = true;

            SetCameraPosition();
            // Draw foreground into the secretTarget
            RenderSystem.DrawToRevealingMask();
            if (with_foreground)
                RenderSystem.DrawToForegroundLayer();
            // Draw light masks into the lightsTarget
            if (with_light)
                RenderSystem.DrawLightMasks();
            // Draw everything into the mainTarget
            RenderSystem.DrawToMainLayer();
            RenderSystem.DrawToHUD();
            // TODO: move hud drawing into the hud layer
            RenderSystem.RenderLayers();

            root.VisualPosition = new PointF(300, 300);
            root.Draw(gameTime.ElapsedGameTime.TotalMilliseconds);

            base.Draw(gameTime);
        }
    }
}
