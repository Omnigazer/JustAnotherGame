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
using MonoGameConsole;

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
        public PhysicsSystem PhysicsSystem { get; set; }

        // Game objects
        public Player player;
        public Level CurrentLevel { get; set; }
        public List<GameObject> objects = new List<GameObject>();

        // Editor groups
        public List<List<GameObject>> Groups { get; set; } = new List<List<GameObject>>() { new List<GameObject>() };

        // HUD states & controls
        public HUDState HUDState { get; set; }
        HUDState defaultHUD;
        HUDState inventoryHUD;
        HUDState editorHUD;
        HUDState charHUD;
        bool game_over;

        public GameConsole console;

        public Queue<string> Logs { get; set; } = new Queue<string>();

        // Events
        public event EventHandler<InventoryEventArgs> onTargetInventoryOpen = delegate { };
        public event EventHandler onTargetInventoryClosed = delegate { };

        int CurrentSongIndex { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
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
            PhysicsSystem = new PhysicsSystem();
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
            // CurrentLevel.Load("blank");

            // Register player
            player = new Player(
                new Vector2(100, 500),
                // new Vector2(110, 192)
                // new Vector2(110, 192)
                new Vector2(15, 27)
                // new Vector2(11, 19.2f)
            );
            AddToMainScene(player);
            player._onDestroy += GameOver;
            // RenderSystem.drawables.Add((RenderComponent)player);

            foreach (var obj in GameContent.Instance.level.objects)
            {
                AddToMainScene(obj);
            }
            foreach (var character in GameContent.Instance.level.characters)
            {
                AddToMainScene(character);
            }

            LoadGroup("village", new Vector2(0, 0));
        }

        private void GameOver(object sender, EventArgs e)
        {
            game_over = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            var spriteBatch = new SpriteBatch(GraphicsDevice);
            GraphicsService.Init(spriteBatch, this);
            GameContent.Init(Content);

            // TODO: use this.Content to load your game content here

            LoadConsole(spriteBatch);
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
            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) && Keyboard.GetState().IsKeyDown(Keys.Q))
                Exit();
            StopMoving();
            if (!game_over && !console.Opened && IsActive)
            {
                HUDState.HandleControls();
            }

            Simulate(gameTime);
            // var song = GameContent.Instance.vampireKiller;
            if (MediaPlayer.State != MediaState.Playing && false)
            {
                MediaPlayer.Volume = 0.1f;
                MediaPlayer.Play(GameContent.Instance.Songs[CurrentSongIndex]);
                CurrentSongIndex = (CurrentSongIndex + 1) % GameContent.Instance.Songs.Count;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Update camera offset based on player position
            var pos = (PositionComponent)player;
            RenderSystem.SetCameraPosition(pos.WorldPosition.Center);
            // TODO: Add your drawing code here
            RenderSystem.Draw();
            base.Draw(gameTime);
        }

        public void Log(string message)
        {
            Logs.Enqueue(message);
            if (Logs.Count > 10)
            {
                Logs.Dequeue();
            }
        }

        public void AddToMainScene(GameObject obj)
        {
            objects.Add(obj);
            var physicable = (PhysicsComponent)obj;
            if (physicable != null)
                PhysicsSystem.Register(physicable);
            // PhysicsSystem.objects.Add(obj);
            var drawable = (RenderComponent)obj;
            if (drawable != null)
                RenderSystem.RegisterDrawable(drawable);
            obj._onDestroy += GameObject_onDestroy;
        }

        public void RemoveFromMainScene(GameObject obj)
        {
            objects.Remove(obj);
            var physicable = (PhysicsComponent)obj;
            PhysicsSystem.Unregister(physicable);
            RenderSystem.RemoveFromDrawables((RenderComponent)obj);
            obj._onDestroy -= GameObject_onDestroy;
        }

        private void GameObject_onDestroy(object sender, EventArgs e)
        {
            var obj = (GameObject)sender;
            RemoveFromMainScene(obj);
        }

        #region Simulate
        public void Simulate(GameTime gameTime)
        {
            float time_scale = (float)gameTime.ElapsedGameTime.Milliseconds * 60 / 1000;
            PhysicsSystem.Tick(time_scale);
            for (int j = objects.Count - 1; j >= 0; j--)
            {
                var obj = objects[j];
                obj.Tick(time_scale);
            }
        }
        #endregion

        #region Position logic
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
            return GetObjectAtCoords(Mouse.GetState().Position);
        }
        #endregion

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
                    var pos = (PositionComponent)obj;
                    if (player_pos.Overlaps(pos))
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

        #region Console
        void LoadConsole(SpriteBatch spriteBatch)
        {
            System.Windows.Forms.Form winGameWindow = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);
            winGameWindow.Show();
            winGameWindow.Hide();
            var x = System.Windows.Forms.Application.OpenForms;
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
                        foreach (var obj in Groups[second])
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
        #endregion

        #region Level code
        public void SaveLevel(string name)
        {
            Log("Saving level");
            string path = String.Format("Content/Data/{0}.json", name);
            CurrentLevel.Save(path);
        }

        public void ClearCurrentLevel()
        {
            // Clear everything
            objects.Clear();
            RenderSystem.drawables.Clear();
            PhysicsSystem.objects.Clear();
            PhysicsSystem.dynamics.Clear();
            CurrentLevel.objects.Clear();

            // Register the player back
            AddToMainScene(player);
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
                AddToMainScene(obj);
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
        #endregion
    }
}
