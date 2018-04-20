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
        public List<GameObject> platforms = new List<GameObject>();
        public List<Character> characters = new List<Character>();
        public List<Projectile> projectiles = new List<Projectile>();

        public HUDState HUDState { get; set; }
        HUDState defaultHUD;
        HUDState inventoryHUD;
        bool game_over;

        public event EventHandler<InventoryEventArgs> onTargetInventoryOpen = delegate { };
        public event EventHandler onTargetInventoryClosed = delegate { };

        // mouse position on last tick
        Point last_position = Point.Zero;
        // object currently being mouse-dragged
        GameObject tele_obj = null;

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
            InitServices();
            var playerHUD = new HUDContainer();
            InitGameObjects();

            defaultHUD = new DefaultHUDState(playerHUD);
            inventoryHUD = new InventoryHUDState(playerHUD, player.inventory);
            HUDState = defaultHUD;
            HUDState = new EditorHUDState(playerHUD);
        }

        public void InitServices()
        {
            GameService.Init(this);
        }

        void InitGameObjects()
        {
            CurrentLevel = GameContent.Instance.level;
            CurrentLevel.TestLoad();

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

            // Register characters

            /*
            for (int i = 0; i < 5;i++)
            {
                RegisterObject(new Zombie(
                new Vector2(-600 - 200 * i, 100),
                new Vector2(15, 20)
                ));
            }

            RegisterObject(new Dawg(
                new Vector2(-200, 850),
                new Vector2(70, 25)
                ));

            /*
            var sword = new WieldedItem(1);
            RegisterObject(sword);
            sword.Hide();
            // sword.SetPosition(200, 100);

            RegisterObject(new Chest(new Vector2(200, 100), new Vector2(40, 20), sword));
            */
            // GameContent.Instance.level.Save("");


            foreach (var obj in GameContent.Instance.level.objects)
            {
                RegisterObject(obj);
            }
            foreach (var character in GameContent.Instance.level.characters)
            {
                RegisterObject(character);
            }
            /*
            CurrentLevel.objects.Clear();
            foreach (var obj in platforms)
            {
                CurrentLevel.objects.Add(obj);
            }

            CurrentLevel.Save("");
            */
        }

        private void GameOver(object sender, EventArgs e)
        {
            game_over = true;
            RenderSystem.drawables.Remove((RenderComponent)player);
        }


        public List<string> Logs { get; set; } = new List<string>() { "test" };
        public void Log(string message)
        {
            Logs.Add(message);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            GraphicsService.Init(new SpriteBatch(GraphicsDevice), this);
            GameContent.Init(Content);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        int song_index;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) && Keyboard.GetState().IsKeyDown(Keys.Q))
                Exit();
            if (!game_over)
                HUDState.HandleControls();
            Simulate();
            // var song = GameContent.Instance.vampireKiller;
            if (MediaPlayer.State != MediaState.Playing && false)
            {
                MediaPlayer.Volume = 0.1f;
                MediaPlayer.Play(GameContent.Instance.Songs[song_index]);
                song_index = (song_index + 1) % GameContent.Instance.Songs.Count;
            }


            base.Update(gameTime);
        }

        public void RegisterObject(GameObject obj)
        {
            platforms.Add(obj);
            RenderSystem.RegisterDrawable(obj);
            obj._onDestroy += GameObject_onDestroy;
        }

        private void GameObject_onDestroy(object sender, EventArgs e)
        {
            var obj = (GameObject)sender;
            platforms.Remove(obj);
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

        public void Simulate()
        {
            RenderSystem.Tick();
            var player_collisions = new List<(Direction, GameObject)>();
            var player_pos = (PositionComponent)player;
            for (int i = platforms.Count - 1; i >= 0; i--)
            {
                var platform = platforms[i];
                var collision_direction = player_pos.Collides(platform);
                if (collision_direction != Direction.None)
                    player_collisions.Add((collision_direction, platform));
                // var movable = (MoveComponent)platform;
                // movable?.Move();
                // platform.Tick();
            }

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var projectile_collisions = new List<(Direction, GameObject)>();
                var projectile = projectiles[i];

                for (int j = platforms.Count - 1; j >= 0; j--)
                {
                    var platform = platforms[j];
                    var projectile_pos = (PositionComponent)projectile;
                    var collision_direction = projectile_pos.Collides(platform);

                    if (collision_direction != Direction.None)
                        projectile_collisions.Add((collision_direction, platform));
                }

                for (int j = characters.Count - 1; j >= 0; j--)
                {
                    var character = characters[j];
                    var projectile_pos = (PositionComponent)projectile;
                    var collision_direction = projectile_pos.Collides(character);
                    if (collision_direction != Direction.None)
                        projectile_collisions.Add((collision_direction, character));
                }
                // var movable = (MoveComponent)projectile;
                // movable.ProcessCollisionInteractions(projectile_collisions);
                // projectile.Tick();
            }
            for (int j = characters.Count - 1; j >= 0; j--)
            {
                var character_collisions = new List<(Direction, GameObject)>();
                var character = characters[j];
                var pos = (PositionComponent)character;
                Direction collision_direction;

                foreach (var platform in platforms)
                {
                    // TODO: look into how we access components
                    collision_direction = pos.Collides(platform);
                    if (collision_direction != Direction.None)
                        character_collisions.Add((collision_direction, platform));
                }

                foreach (var other_char in characters)
                {
                    if (other_char == character)
                        continue;
                    // TODO: look into how we access components
                    collision_direction = pos.Collides(other_char);
                    if (collision_direction != Direction.None)
                        character_collisions.Add((collision_direction, other_char));
                }

                collision_direction = pos.Collides(player);
                // TODO: collision with a player should trump / complement other collisions here
                if (collision_direction != Direction.None)
                    character_collisions.Add((collision_direction, player));

                // TODO: move this into the tick
                var char_movable = (MoveComponent)character;
                // char_movable.ProcessCollisionInteractions(character_collisions);
                // char_movable.Move();

                // character.Tick();
            }

            var player_movable = (MoveComponent)player;
            player_movable.ProcessCollisionInteractions(player_collisions);
            player_movable.Move();
            player.Tick();
        }

        public GameObject GetObjectAtCoords(Point pt)
        {
            var game_click_position = RenderSystem.ScreenToGame(pt);
            foreach (var platform in platforms)
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

        public GameObject GetObjectAtCursor()
        {
            return GetObjectAtCoords(Mouse.GetState().Position);
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

            foreach(var obj in platforms)
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
                var mouseState = Mouse.GetState();
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

        protected void HandleControls()
        {

        }

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
            base.Draw(gameTime);
        }

        public Rectangle GameToScreen(Rectangle rect, Vector2 clamped_origin)
        {
            // rect.Location = new Point(rect.Location.X, -rect.Location.Y);
            rect.Location = new Point(rect.Location.X, - rect.Location.Y - rect.Height);
            // rect.Location = new Point(rect.Location.X, -rect.Location.Y - rect.Height + (int)(2 * rect.Height * clamped_origin.Y));

            return rect;
        }

        /*

        */
    }
}
