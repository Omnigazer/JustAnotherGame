using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using Omniplatformer.HUDStates;
using Microsoft.Xna.Framework.Media;
using MonoGameConsole;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Objects.Interactibles;
using Omniplatformer.Objects.Inventory;
using Omniplatformer.Objects.Items;
using Omniplatformer.Scenes;
using Omniplatformer.Scenes.Subsystems;
using Omniplatformer.Services;
using Omniplatformer.Utility;
using Omniplatformer.Components.Character;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Linq;
using Omniplatformer.Components.Interactibles;

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

        private HUDState defaultHUD;
        private HUDState inventoryHUD;
        private EditorHUDState editorHUD;
        private HUDState charHUD;
        private bool game_over;
        private bool game_paused;

        public GameConsole console;

        public Queue<string> Logs { get; set; } = new Queue<string>();

        // Events
        public Subject<Inventory> onTargetInventoryOpen = new Subject<Inventory>();

        int CurrentSongIndex { get; set; }
        public Subject<Inventory> onTargetInventoryClosed = new Subject<Inventory>();

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
            LoadLevel();
            defaultHUD = new DefaultHUDState();
            inventoryHUD = new InventoryHUDState(Player.Inventory);
            charHUD = new CharHUDState();
            editorHUD = new EditorHUDState();
            HUDState = defaultHUD;
        }

        public void InitServices()
        {
            GameService.Init(this);
        }

        private void LoadLevel()
        {
            MainScene = new Level();
            MainScene.RenderSystem = new RenderSystem(this);
            MainScene.PhysicsSystem = new PhysicsSystem();
            // TODO: refactor this
            MainScene.Subsystems.Add(MainScene.RenderSystem);
            MainScene.Subsystems.Add(MainScene.PhysicsSystem);
            MainScene.Subsystems.Add(new SimulationSystem());
            MainScene.Load("default_level");
            // TODO: extract this
            Player.GetComponent<DestructibleComponent>().OnDestroy.Take(1).Subscribe((obj) => GameOver());
        }

        public void TogglePause()
        {
            game_paused = !game_paused;
        }
        private void GameOver()
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
            MainScene.ProcessRemovals();
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
        }

        #region Simulate

        public void Simulate(GameTime gameTime)
        {
            float time_scale = 60.0f / 1000;
            float dt = time_scale * (float)gameTime.ElapsedGameTime.Milliseconds;
            MainScene.ProcessSubsystems(dt);
            if (!game_paused)
                MainScene.ProcessSubsystems(dt);
            // TODO: include this as a subsystem
            var drawable = (TileMapRenderComponent)MainScene.TileMap;
            drawable.RebuildBuffers();
            HUDState.Tick();
        }

        #endregion Simulate

        #region Position logic

        public GameObject GetObjectAtCursor()
        {
            var coords = RenderSystem.ScreenToGame(Mouse.GetState().Position);
            return PhysicsSystem.GetObjectAtCoords(coords);
        }

        #endregion Position logic

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
            HUDState = inventoryHUD;
            onTargetInventoryOpen.OnNext(inv);
        }

        public void CloseTargetInventory()
        {
            onTargetInventoryClosed.OnNext(null);
        }

        public void Interact()
        {
            var player_pos = (PositionComponent)Player;

            var obj = PhysicsSystem.GetOverlappingObjects(player_pos).First(x => x.GetComponent<InteractibleComponent>() != null);
            if (obj != null)
            {
                obj.GetComponent<InteractibleComponent>().Interact();
            }
        }

        public void CloseChest()
        {
            CloseTargetInventory();
        }

        public void WalkLeft()
        {
            var movable = (PlayerMoveComponent)Player;
            movable.MoveDirection = Direction.Left;
        }

        public void WalkRight()
        {
            var movable = (PlayerMoveComponent)Player;
            movable.MoveDirection = Direction.Right;
        }

        public void GoUp()
        {
            var movable = (PlayerMoveComponent)Player;
            if (movable.CanClimb)
            {
                movable.StartClimbing();
            }
            movable.MoveDirection = Direction.Up;
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
            movable.MoveDirection = Direction.Down;
        }

        public void StopMoving()
        {
            var movable = (PlayerMoveComponent)Player;
            movable.MoveDirection = Direction.None;
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
            Player.GetComponent<PlayerActionComponent>().Fire();
        }

        public void Stand()
        {
            var x = (PositionComponent)Player;
            x.SetLocalHalfsize(x.WorldPosition.Halfsize * 2);
        }

        public void Duck()
        {
            var x = (PositionComponent)Player;
            x.SetLocalHalfsize(x.WorldPosition.Halfsize / 2);
        }

        // fps is assumed to be 30 while we're tick-based
        private float zoom_adjust_rate = 0.2f / 30;

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

        public void PerformMouseAction(MouseEventArgs e, bool is_down)
        {
            Item item = e.Button switch
            {
                MouseButton.Left => Player.GetComponent<EquipComponent>().EquipSlots.RightHandSlot.Item,
                MouseButton.Right => Player.GetComponent<EquipComponent>().EquipSlots.LeftHandSlot.Item,
                _ => null
            };
            if (item != null)
                Player.GetComponent<PlayerActionComponent>().PerformItemAction((dynamic)item, is_down);
        }

        #endregion Player Actions

        #region Console

        private void LoadConsole(SpriteBatch spriteBatch)
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

            /*
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
            */

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
        }

        #endregion Console

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

        /*
        public void SaveGroup(string name)
        {
            Log(String.Format("Saving group {0}", name));
            string path = String.Format("Content/Data/{0}.json", name);
            LevelLoader.SaveGroup(Groups[name], path);
        }
        */

        #endregion Level code
    }
}
