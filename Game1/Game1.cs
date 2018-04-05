using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omniplatformer.Characters;
using Omniplatformer.Components;
using Omniplatformer.HUD;
using System;
using System.Collections.Generic;

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
        public List<GameObject> platforms = new List<GameObject>();
        public List<Character> characters = new List<Character>();
        public List<Projectile> projectiles = new List<Projectile>();

        // check if these keys were released prior to this tick
        bool space_released = true;
        bool fire_released = true;
        bool attack_released = true;
        bool wield_released = true;
        bool game_over;

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
            InitGameObjects();
            InitServices();
        }

        public void InitServices()
        {
            GameService.Init(this);
        }

        WieldedItem sword;

        void InitGameObjects()
        {
            player = new Player(
                new Vector2(100, 500),
                // new Vector2(221, 385)
                // new Vector2(110, 192)
                new Vector2(15, 20)
                // new Vector2(11, 19.2f)
            );
            RenderSystem.drawables.Add((RenderComponent)player);
            player._onDestroy += GameOver;

            var ladder = new Ladder(
                new Vector2(0, 0),
                new Vector2(15, 40),
                player
            );

            RegisterObject(ladder);

            sword = new WieldedItem(damage: 1);
            // sword = new WieldedItem(player, new Vector2(12, 30), 0);
            RegisterObject(sword);

            for (int i = 0; i < 5;i++)
            {
                RegisterObject(new Zombie(
                new Vector2(-600 - 200 * i, 100),
                new Vector2(15, 20)
                ));
            }

            RegisterObject(new SolidPlatform(
                new Vector2(400, 800),
                new Vector2(5, 100)
            ));

            RegisterObject(new SolidPlatform(
                new Vector2(50, 800),
                new Vector2(400, 30)
            ));

            RegisterObject(new ForegroundQuad(
                new Vector2(50, 860),
                new Vector2(400, 30)
            ));

            RegisterObject(new SolidPlatform(
                new Vector2(500, 50),
                new Vector2(1000, 30)
            ));

            RegisterObject(new SolidPlatform(
                new Vector2(0, 0),
                new Vector2(10000, 20)
            ));

            RegisterObject(new DestructibleObject(
                new Vector2(1000, 480),
                new Vector2(30, 400)
            ));

            RegisterObject(new SolidPlatform(
                new Vector2(800, 550),
                new Vector2(30, 400)
            ));


            RegisterObject(new Ladder(
                new Vector2(500, 650),
                new Vector2(15, 550)
            ));

            RegisterObject(new Liquid(
                new Vector2(2015, 50),
                new Vector2(515, 300)
            ));

            RegisterObject(new MovingPlatform(new Vector2(200, 200)));
            RegisterObject(new MovingPlatform(new Vector2(600, 150)));
            RegisterObject(new MovingPlatform(new Vector2(200, 300)));
            RegisterObject(new MovingPlatform(new Vector2(600, 350)));
            RegisterObject(new MovingPlatform(new Vector2(200, 450)));
            RegisterObject(new MovingPlatform(new Vector2(600, 550)));

            RegisterObject(new Collectible(new Vector2(200, 200), new Vector2(20, 20)));
        }

        private void GameOver(object sender, EventArgs e)
        {
            game_over = true;
            RenderSystem.drawables.Remove((RenderComponent)player);
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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            HandleControls();
            Simulate();

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
                var movable = (MoveComponent)platform;
                movable?.Move();
                platform.Tick();
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
                var movable = (MoveComponent)projectile;
                movable.ProcessCollisionInteractions(projectile_collisions);
                projectile.Tick();
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
                char_movable.ProcessCollisionInteractions(character_collisions);
                char_movable.Move();

                character.Tick();
            }

            var player_movable = (MoveComponent)player;
            player_movable.ProcessCollisionInteractions(player_collisions);
            player_movable.Move();
            player.Tick();
        }

        public GameObject GetObjectAtCursor()
        {
            var game_click_position = RenderSystem.ScreenToGame(Mouse.GetState().Position);
            foreach (var platform in platforms)
            {
                var platform_pos = (PositionComponent)platform;
                if (platform_pos.Contains(game_click_position))
                {
                    return platform;
                }
            }
            return null;
        }

        protected void HandleControls()
        {
            if (game_over)
                return;
            // TODO: referencing a concrete implementation
            var movable = (PlayerMoveComponent)player;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                movable.move_direction = Direction.Left;
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                movable.move_direction = Direction.Right;
            else if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (movable.CanClimb)
                {
                    movable.StartClimbing();
                }
                movable.move_direction = Direction.Up;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (movable.CanClimb)
                {
                    movable.StartClimbing();
                }
                movable.move_direction = Direction.Down;
            }
            else
                movable.move_direction = Direction.None;

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (space_released)
                {
                    space_released = false;
                    movable.Jump();
                }
            }
            else
            {
                space_released = true;
                movable.StopJumping();
            }


            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
            {
                // var pos = (PositionComponent)player;
                var pos = (PositionComponent)sword;
                pos.Rotate(0.1f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6))
            {
                // var pos = (PositionComponent)player;
                var pos = (PositionComponent)sword;
                pos.Rotate(-0.1f);
            }

            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                var obj = GetObjectAtCursor();
                if (tele_obj == null && obj != null)
                    tele_obj = obj;
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
            else
            {
                last_position = Point.Zero;
                tele_obj = null;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                //var p = new TestProjectile(new Vector2(200, 200), new Vector2(5,5));
                //p.direction = new Vector2(3, 0);
                //projectiles.Add(p);
                if (fire_released)
                {
                    fire_released = false;
                    // projectiles.Add(player.Fire());
                    RegisterObject(player.Fire());
                }
            }
            else
            {
                fire_released = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                //var p = new TestProjectile(new Vector2(200, 200), new Vector2(5,5));
                //p.direction = new Vector2(3, 0);
                //projectiles.Add(p);
                if (attack_released)
                {
                    attack_released = false;
                    // projectiles.Add(player.Fire());
                    player.Swing();
                }
            }
            else
            {
                attack_released = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                //var p = new TestProjectile(new Vector2(200, 200), new Vector2(5,5));
                //p.direction = new Vector2(3, 0);
                //projectiles.Add(p);
                if (wield_released)
                {
                    wield_released = false;
                    // projectiles.Add(player.Fire());
                    player.ToggleItem(sword);
                }
            }
            else
            {
                wield_released = true;
            }

            // fps is assumed to be 30 while we're tick-based
            float zoom_adjust_rate = 0.2f / 30;

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                RenderSystem.Camera.AdjustZoom(-zoom_adjust_rate);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                RenderSystem.Camera.AdjustZoom(zoom_adjust_rate);
            }
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

        public Rectangle GameToScreen(Rectangle rect)
        {
            rect.Location = new Point(rect.Location.X, - rect.Location.Y - rect.Height);
            return rect;
        }

        /*

        */
    }
}
