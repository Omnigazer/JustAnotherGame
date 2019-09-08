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
using Omniplatformer.Scenes;

namespace Omniplatformer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // Graphics objects
        public GraphicsDeviceManager graphics;
        public Level MainScene { get; set; }
        public RenderSystem RenderSystem => MainScene.RenderSystem;
        public PhysicsSystem PhysicsSystem => MainScene.PhysicsSystem;

        // Game objects
        public Player Player => MainScene.Player;
        // public Level CurrentLevel { get; set; }

        // Editor groups
        // public List<List<GameObject>> Groups { get; set; } = new List<List<GameObject>>() { new List<GameObject>() };
        public Dictionary<string, List<GameObject>> Groups => MainScene.Groups;

        // HUD states & controls
        public HUDState HUDState { get; set; }
        HUDState defaultHUD;
        HUDState inventoryHUD;
        EditorHUDState editorHUD;
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
            // IsFixedTimeStep = false;
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
            InitServices();
            var playerHUD = new HUDContainer();
            LoadLevel(false);

            defaultHUD = new DefaultHUDState(playerHUD);
            inventoryHUD = new InventoryHUDState(playerHUD, Player.Inventory);
            charHUD = new CharHUDState(playerHUD);
            editorHUD = new EditorHUDState(playerHUD);
            HUDState = defaultHUD;
        }

        public void InitServices()
        {
            GameService.Init(this);
        }

        void LoadLevel(bool bitmap)
        {
            MainScene = new Level();
            MainScene.RenderSystem = new RenderSystem(this);
            MainScene.PhysicsSystem = new PhysicsSystem();
            // TODO: refactor this
            MainScene.Subsystems.Add(MainScene.RenderSystem);
            MainScene.Subsystems.Add(MainScene.PhysicsSystem);
            MainScene.Subsystems.Add(new SimulationSystem());
            if (bitmap)
                MainScene.LoadFromBitmap("level1");
            else
                MainScene.Load("bsave6");
            // TODO: extract this
            Player._onDestroy += GameOver;
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
            Services.AddService(typeof(SpriteBatch), spriteBatch);
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
            RenderSystem.Draw();
            GraphicsService.Instance.Begin(SpriteSortMode.Immediate);
            // GraphicsService.Instance.DrawString(GameContent.Instance.defaultFont, gameTime.ElapsedGameTime.Milliseconds.ToString(), new Vector2(50, 50), Color.White);
            GraphicsService.Instance.DrawString(GameContent.Instance.defaultFont, PhysicsSystem.objects.Count.ToString(), new Vector2(50, 250), Color.White);

            // GraphicsService.Instance.DrawString(GameContent.Instance.defaultFont, phys_time.ToString(), new Vector2(50, 350), Color.White);
            // GraphicsService.Instance.DrawString(GameContent.Instance.defaultFont, elapsed_frames.ToString(), new Vector2(50, 450), Color.White);
            GraphicsService.Instance.End();
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
            AddToScene(obj, MainScene);
        }

        public void AddToScene(GameObject obj, Scene scene)
        {
            scene.RegisterObject(obj);
        }

        public void RemoveFromMainScene(GameObject obj)
        {
            MainScene.UnregisterObject(obj);
            // TODO: refactor this
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
            float time_scale = 60.0f / 1000;
            float dt = time_scale * (float)gameTime.ElapsedGameTime.Milliseconds;
            MainScene.ProcessSubsystems(dt);
            // TODO: include this as a subsystem
            HUDState.Tick();
        }
        #endregion

        #region Position logic
        public GameObject GetObjectAtCursor()
        {
            var coords = RenderSystem.ScreenToGame(Mouse.GetState().Position);
            return PhysicsSystem.GetObjectAtCoords(coords);
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

        public void OpenChest()
        {
            var player_pos = (PositionComponent)Player;

            foreach (var obj in PhysicsSystem.GetOverlappingObjects(player_pos))
            {
                // TODO: refactor this into a component
                if (obj is Chest)
                {
                    HUDState = inventoryHUD;
                    OpenTargetInventory(((Chest)obj).Inventory);
                    break;
                }
            }
        }

        public void CloseChest()
        {
            CloseTargetInventory();
        }

        public void WalkLeft()
        {
            var movable = (PlayerMoveComponent)Player;
            movable.move_direction = Direction.Left;
        }

        public void WalkRight()
        {
            var movable = (PlayerMoveComponent)Player;
            movable.move_direction = Direction.Right;
        }

        public void GoUp()
        {
            var movable = (PlayerMoveComponent)Player;
            if (movable.CanClimb)
            {
                movable.StartClimbing();
            }
            movable.move_direction = Direction.Up;
        }

        public void GoDown()
        {
            var movable = (PlayerMoveComponent)Player;
            if (movable.CanClimb)
            {
                movable.StartClimbing();
            }
            else
            {
                Duck();
            }
            movable.move_direction = Direction.Down;
        }

        public void StopMoving()
        {
            var movable = (PlayerMoveComponent)Player;
            movable.move_direction = Direction.None;
        }

        public void Jump()
        {
            var movable = (PlayerMoveComponent)Player;
            movable.Jump();
        }

        public void StopJumping()
        {
            var movable = (PlayerMoveComponent)Player;
            movable.StopJumping();
        }

        public void Fire()
        {
            Player.Fire();
        }

        public void Stand()
        {
            var x = (PositionComponent)Player;
            x.SetLocalHalfsize(x.WorldPosition.halfsize * 2);
        }

        public void Duck()
        {
            var x = (PositionComponent)Player;
            x.SetLocalHalfsize(x.WorldPosition.halfsize / 2);
        }

        public void Swing()
        {
            Player.Swing();
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

        public void ResetZoom()
        {
            RenderSystem.Camera.ResetZoom();
        }

        #endregion

        #region Console
        void LoadConsole(SpriteBatch spriteBatch)
        {
            System.Windows.Forms.Form winGameWindow = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Window.Handle);
            winGameWindow.Show();
            winGameWindow.Hide();

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

            console.AddCommand("upload_buffers", a =>
            {
                // TODO your logic
                RenderSystem.tilemap.UploadBuffers();
                return String.Format("pong");
            });

            console.AddCommand("merge", a =>
            {
                if (a.Length == 2)
                {
                    string first = a[0], second = a[1];
                    if (Groups.ContainsKey(first) && Groups.ContainsKey(second))
                    {
                        foreach (var obj in Groups[second])
                        {
                            Groups[first].Add(obj);
                        }
                        return String.Format("Merged group {0} into {1}", second, first);
                    }
                    else
                    {
                        return "Group not found";
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

            /*
            console.AddCommand("clearlevel", a =>
            {
                ClearCurrentLevel();
                return "Level cleared.";
            });
            */

            console.AddCommand("showgroups", a =>
            {
                if (a.Length == 0)
                {
                    return "Groups: " + String.Join(" ", Groups.Keys);
                }
                else
                    return String.Format("invalid args");
            });

            console.AddCommand("setgroup", a =>
            {
                if (a.Length == 1)
                {
                    string name = a[0];
                    editorHUD.SetGroup(name);
                    return "Group set.";
                }
                else
                    return String.Format("invalid args");
            });

            console.AddCommand("savegroup", a =>
            {
                if (a.Length == 2 && int.TryParse(a[0], out int index))
                {
                    string name = a[1];
                    SaveGroup(name);
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

            console.AddCommand("unloadlevel", a =>
            {
                if (a.Length == 0)
                {
                    // UnloadLevel();
                    // return "Level unloaded.";
                    return "Not implemented.";
                }
                else
                    return String.Format("invalid args");
            });

            console.AddCommand("load", a =>
            {
                if (a.Length == 1)
                {
                    string arg = a[0];
                    LoadLevel(arg == "bitmap");
                    /*
                    TestScene.Player = Player;
                    MainScene = TestScene;
                    AddToMainScene(Player);
                    MainScene.LoadPlayer(Player);
                     */

                    return "Done.";
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
            MainScene.Save(path);
        }

        /// <summary>
        /// Load group of objects, or "location"
        /// </summary>
        /// <param name="name"></param>
        public void LoadGroup(string name, Vector2? origin = null)
        {
            Log(String.Format("Loading group '{0}'", name));
            string path = String.Format("Content/Data/{0}.json", name);

            var group = LevelLoader.LoadGroup(path, origin ?? ((PositionComponent)Player).WorldPosition.Coords);
            foreach (var obj in group)
            {
                AddToMainScene(obj);
                // CurrentLevel.objects.Add(obj);
            }
            Groups.Add(name, group);
        }

        public void SaveGroup(string name)
        {
            Log(String.Format("Saving group {0}", name));
            string path = String.Format("Content/Data/{0}.json", name);
            LevelLoader.SaveGroup(Groups[name], path);
        }
        #endregion
    }
}
