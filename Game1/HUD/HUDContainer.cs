using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Omniplatformer.HUD
{
    /// <summary>
    /// Containing class for all the default HUD elements, such as health/mana bars
    /// </summary>
    public class HUDContainer
    {
        public HUDContainer()
        {
            healthLightsTarget = new RenderTarget2D(
            GraphicsDevice, GameContent.Instance.healthBarLightMask.Width, GameContent.Instance.healthBarLightMask.Height);
            mana_bars = new Dictionary<ManaType, ManaBar>();

            int bar_height = 60;
            mana_bars[ManaType.Chaos] = new ChaosManaBar(new Point(150, 120), 400, bar_height);
            mana_bars[ManaType.Nature] = new NatureManaBar(new Point(150, 190), 400, bar_height);
            mana_bars[ManaType.Life] = new LifeManaBar(new Point(150, 260), 400, bar_height);
            mana_bars[ManaType.Death] = new DeathManaBar(new Point(150, 330), 400, bar_height);
            mana_bars[ManaType.Sorcery] = new SorceryManaBar(new Point(150, 400), 400, bar_height);
            exp_bar = new ExperienceBar(new Point(560, 50), 800, 30);
        }

        Dictionary<ManaType, ManaBar> mana_bars;
        ExperienceBar exp_bar;

        // Player Player { get; set; }
        Player Player => GameService.Player;
        GraphicsDevice GraphicsDevice { get { return GraphicsService.GraphicsDevice; } }
        RenderTarget2D healthLightsTarget;

        void DrawHealthBarLightMask()
        {
            var lightMask = GameContent.Instance.healthBarLightMask;
            var spriteBatch = GraphicsService.Instance;
            GraphicsDevice.SetRenderTarget(healthLightsTarget);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            //var size_rect = new Rectangle(0, 0, 400, bar_height * 15);
            // spriteBatch.Draw(lightMask, size_rect, Color.White);
            // spriteBatch.Draw(lightMask, Vector2.Zero, GetLightColor());
            spriteBatch.Draw(lightMask, Vector2.Zero, Color.White);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        void DrawCursor()
        {
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            Point cursor_position = Mouse.GetState().Position;
            Point cursor_size = new Point(24, 48);
            var rect = new Rectangle(cursor_position, cursor_size);
            // Draw directly via the SpriteBatch instance bypassing y-axis flip
            GraphicsService.Instance.Draw(GameContent.Instance.cursor, rect, Color.White);
            spriteBatch.End();
        }

        public void Draw()
        {
            // TODO: maybe group some/all of this into a single spriteBatch
            var spriteBatch = GraphicsService.Instance;
            // DrawHealthBarLightMask();
            DrawCursor();
            DrawHealthBar();
            foreach (var bar in mana_bars)
            {
                bar.Value.Draw();
            }
            exp_bar.Draw();
            /*
            foreach (ManaType type in Enum.GetValues(typeof(ManaType)))
            {
                DrawManaBar(type);
            }
            */
        }

        protected int bar_loop = 0;
        protected int loop_period = 8;
        protected float distort_loop = 0;
        protected float distort_amp = 200; // merely a technical value
        protected float distort_speed = 0.25f;

        void ContinueLoop()
        {
            bar_loop = (bar_loop + 1) % (400 * loop_period);
            distort_loop = (distort_loop + distort_speed) % distort_amp;
        }

        void ApplyDistort()
        {
            var distortEffect = GameContent.Instance.DistortEffect;
            distortEffect.Parameters["OffsetPower"].SetValue(0.05f);
            // float lower_angle = (float)(-Math.PI);
            float lower_angle = 0;
            float upper_angle = 2 * (float)(Math.PI);
            float amp = upper_angle - lower_angle;
            float angle = (float)(lower_angle + amp * ((float)distort_loop / distort_amp));
            distortEffect.Parameters["angle"].SetValue(angle);
            distortEffect.CurrentTechnique.Passes[0].Apply();
        }

        void DrawHealthBar()
        {
            ContinueLoop();
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            Point bar_position = new Point(150, 50);
            int bar_border_thickness = 5;
            Point border_size = new Point(bar_border_thickness, bar_border_thickness);
            int width = 400, height = 60;
            Point size = new Point(width, height);
            Point current_size = new Point((int)((width - bar_border_thickness * 2) * (Player.CurrentHitPoints / Player.MaxHitPoints)),
                height - bar_border_thickness * 2);

            Rectangle outer_rect = new Rectangle(bar_position, size);
            Rectangle inner_rect = new Rectangle(bar_position + border_size, current_size);

            // Apply the light effect
            var lightEffect = GameContent.Instance.AdditiveEffect;
            lightEffect.Parameters["lightMask"].SetValue(healthLightsTarget);
            // lightEffect.CurrentTechnique.Passes[0].Apply();

            var source_rect = new Rectangle((int)(bar_loop / loop_period), 0, GameContent.Instance.testLiquid.Width / 2, GameContent.Instance.testLiquid.Height);
            spriteBatch.Draw(GameContent.Instance.whitePixel, outer_rect, Color.Gray);

            // Apply the distort effect
            ApplyDistort();

            spriteBatch.Draw(GameContent.Instance.testLiquid, inner_rect, source_rect, Color.Red);
            spriteBatch.End();
        }
    }
}
