using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using Omniplatformer.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public class RenderSystem
    {
        public Game1 Game { get; set; }
        public Camera Camera { get; set; }
        public GraphicsDeviceManager graphics => Game.graphics;
        // public Mouse

        RenderTarget2D lightsTarget = null;
        RenderTarget2D mainTarget = null;
        RenderTarget2D secretTarget = null;
        RenderTarget2D alphaMaskTarget = null;
        RenderTarget2D HUDTarget = null;
        // Revealable foreground layer (preliminary)

        // Internal counters
        float light_loop = 0;
        const float light_loop_length = 100;
        float day_loop = 0;
        const float day_loop_length = 3600;

        GraphicsDevice GraphicsDevice { get; set; }

        public List<RenderComponent> drawables = new List<RenderComponent>();

        public RenderSystem(Game1 game)
        {
            Game = game;
            GraphicsDevice = game.GraphicsDevice;
            Camera = new Camera();
            SetResolution(
                graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width,
                graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height
                );
            InitRenderTargets();
        }

        public void SetCameraPosition(Vector2 coords)
        {
            Camera.Position = coords;
        }

        public void SetResolution(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();
        }

        public (int, int) GetResolution()
        {
            return (graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        public void Tick(float time_scale)
        {
            light_loop = (light_loop + time_scale) % light_loop_length;
            // day_loop = (day_loop + 1) % day_loop_length;
        }

        public void Draw()
        {
            bool with_light = true, with_foreground = true;
            // Draw foreground into the secretTarget
            DrawToRevealingMask();
            if (with_foreground)
                DrawToForegroundLayer();
            // Draw light masks into the lightsTarget
            if (with_light)
                DrawLightMasks();
            // Draw everything into the mainTarget
            DrawToMainLayer();
            DrawToHUD();
            // TODO: move hud drawing into the hud layer
            RenderLayers();
        }

        public void RegisterDrawable(RenderComponent drawable)
        {
            drawables.Add(drawable);
            drawables = drawables.OrderBy(x => x.ZIndex).ToList();
        }

        public void RemoveFromDrawables(RenderComponent drawable)
        {
            drawables.Remove(drawable);
        }

        public void DrawToForegroundLayer()
        {
            var spriteBatch = GraphicsService.Instance;
            GraphicsDevice.SetRenderTarget(secretTarget);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Camera.TranslationMatrix);
            // spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Matrix.Identity);

            // sample foreground
            // spriteBatch.Draw(GameContent.Instance.greenPixel, new Rectangle((new Vector2(-600, -1500)).ToPoint(), new Point(400, 400)), Color.White);
            //var location = new Point(200, 200);
            foreach (var drawable in drawables)
            {
                drawable.DrawToForeground();
            }
            /*
            var location = ScreenToView(new Point(200, 200)).ToPoint();
            var size = new Point(400, 400);
            var green_rect = new Rectangle(location, size);
            spriteBatch.Draw(GameContent.Instance.greenPixel, green_rect, Color.White);
            */
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, new MinAlphaBlendState());
            spriteBatch.Draw(alphaMaskTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public void DrawLightMasks()
        {
            var lightMask = GameContent.Instance.lightMask;
            var spriteBatch = GraphicsService.Instance;
            GraphicsDevice.SetRenderTarget(lightsTarget);
            // GraphicsDevice.Clear(Color.DarkSlateGray);
            GraphicsDevice.Clear(GetAmbientLightColor());

            // Draw to the light mask (world coords)
            // Draw light from registered projectiles
            // DrawObjectsLightMasks(projectiles);
            DrawObjectsLightMasks();

            // Draw to the light mask (screen coords)
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Camera.NonTranslationMatrix);
            Vector2 mask_halfsize = new Vector2(400, 300);
            // Draw light mask in the center
            spriteBatch.Draw(lightMask, ScreenToView(Camera.ViewportCenter.ToPoint()) - mask_halfsize, GetLightColor());
            // Draw light mask for the cursor
            spriteBatch.Draw(lightMask, ScreenToView(Mouse.GetState().Position) - mask_halfsize, GetLightColor());
            spriteBatch.End();
        }

        // public void DrawObjectsLightMasks(List<Projectile> projectiles)
        public void DrawObjectsLightMasks()
        {
            var lightMask = GameContent.Instance.lightMask;
            Vector2 mask_halfsize = new Vector2(400, 300); // TODO: Magic number / hardcoded
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Camera.TranslationMatrix);
            foreach (var drawable in drawables)
            {
                drawable.DrawToLightMask();
            }
            spriteBatch.End();
        }

        public void InitRenderTargets()
        {
            var pp = GraphicsDevice.PresentationParameters;
            // Light mask
            lightsTarget = new RenderTarget2D(
            GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

            //healthLightsTarget = new RenderTarget2D(
            //GraphicsDevice, GameContent.Instance.healthBarLightMask.Width, GameContent.Instance.healthBarLightMask.Height);

            // Main scene
            mainTarget = new RenderTarget2D(
            GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

            secretTarget = new RenderTarget2D(
            GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

            alphaMaskTarget = new RenderTarget2D(
            GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

            HUDTarget = new RenderTarget2D(
            GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        public void DrawToRevealingMask()
        {
            var spriteBatch = GraphicsService.Instance;
            var alphaMask = GameContent.Instance.alphaMask;
            GraphicsDevice.SetRenderTarget(alphaMaskTarget);
            GraphicsDevice.Clear(Color.Black);
            // TODO: refactor this to a batch of draw calls to in-game objects, including cursor
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, null, Camera.NonTranslationMatrix);
            // Render revealing cursor-based alpha mask
            Vector2 mask_halfsize = new Vector2(150, 150);
            var location = (ScreenToView(Mouse.GetState().Position) - mask_halfsize).ToPoint();
            // location = Mouse.GetState().Position - mask_halfsize.ToPoint();
            var size = (mask_halfsize * 2).ToPoint();
            var rect = new Rectangle(location, size);
            spriteBatch.Draw(alphaMask, rect, Color.White);
            spriteBatch.End();

            // Pseudocode for object-based revealing draw calls
            /*
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, null, camera.TranslationMatrix);
            foreach (var drawable in drawables)
            {
                drawable.DrawToRevealingMask();
            }
            spriteBatch.Draw(alphaMask, rect, Color.White);
            spriteBatch.End();
            */
        }

        public void DrawToMainLayer()
        {
            var spriteBatch = GraphicsService.Instance;
            // Main layer
            GraphicsDevice.SetRenderTarget(mainTarget);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, Camera.TranslationMatrix);
            // spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            foreach (var drawable in drawables.Where(x => !x.Hidden))
            {
                drawable.Draw();
            }

            spriteBatch.End();

            /*
            spriteBatch.Begin(SpriteSortMode.Immediate, new MultiplyBlendState());
            spriteBatch.Draw(lightsTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
            */

        }

        public void DrawToHUD()
        {
            // TODO: resolve adding subscenes
            GraphicsDevice.SetRenderTarget(HUDTarget);
            GraphicsDevice.Clear(Color.Transparent);
            Game.HUDState.Draw();
            // DrawCursor();
        }

        public void RenderLayers()
        {
            bool with_light = true, with_foreground = true;
            var spriteBatch = GraphicsService.Instance;
            // Draw everything into the final scene
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.TranslationMatrix);

            if (with_light && false)
            {
                // Apply prerendered lightsTarget as a light mask to the main scene
                var lightEffect = GameContent.Instance.MultiplyEffect;
                lightEffect.Parameters["lightMask"].SetValue(lightsTarget);
                lightEffect.CurrentTechnique.Passes[0].Apply();
            }

            // Draw the main scene
            spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);


            // var alphaEffect = GameContent.Instance.AlphaEffect;
            // alphaEffect.Parameters["alphaMask"].SetValue(alphaMaskTarget);
            // alphaEffect.CurrentTechnique.Passes[0].Apply();
            // Draw the foreground on top of main scene
            if (with_foreground)
                spriteBatch.Draw(secretTarget, Vector2.Zero, Color.White);
            // spriteBatch.Draw(finalSecretTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, new MultiplyBlendState());
            spriteBatch.Draw(lightsTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(HUDTarget, Vector2.Zero, Color.White);
            // spriteBatch.Draw(lightsTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        //public Rectangle ScreenToGame(Rectangle rect)
        //{

            /*
            var viewport = GraphicsDevice.Viewport;
            // Flip the camera
            rect.Location = new Point(rect.Location.X, viewport.Height - rect.Location.Y + rect.Height);
            rect.Location -= new Point(viewport.Width / 2, viewport.Height / 2);
            var position = ((PositionComponent)player).center.ToPoint();
            rect.Location += position;
            return rect;
            */
        //}

        public Vector2 ScreenToView(Point point)
        {
            Vector2 v = Vector2.Transform(point.ToVector2(), Matrix.Invert(Camera.NonTranslationMatrix));
            return new Vector2(v.X, v.Y);
        }

        public Vector2 ScreenToGame(Point point)
        {
            Vector2 v = Vector2.Transform(point.ToVector2(), Matrix.Invert(Camera.TranslationMatrix));
            return new Vector2(v.X, -v.Y);
        }

        public Color GetLightColor()
        {
            return GetLightColor(Color.LightGoldenrodYellow);
        }

        public Color GetLightColor(Color base_color, float from = 0.65f, float to = 0.35f)
        {
            float alpha = from + Math.Abs(((light_loop_length / 2 - light_loop) / (light_loop_length / 2)) * to);
            return base_color * alpha;
        }

        public Color GetAmbientLightColor()
        {
            float offset_loop = Math.Abs(day_loop - day_loop_length / 2);
            float dark_percentage = 0.2f;
            offset_loop -= day_loop_length * dark_percentage / 2;
            int brightness = (int)((Math.Max(offset_loop, 0) / (day_loop_length / 2)) * 255);
            return Color.FromNonPremultiplied(brightness, brightness, brightness, 255);
        }
    }
}
