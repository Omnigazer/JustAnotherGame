using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components.Character;
using Omniplatformer.Content;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Scenes.Subsystems;
using Omniplatformer.Services;

namespace Omniplatformer.Views.HUD
{
    public class HealthBar : ViewControl, IUpdatable
    {
        Player Player => GameService.Player;

        public HealthBar()
        {

        }

        public override void SetupNode()
        {
            Width = 400;
            Height = 60;
            BorderThickness = 5;
            Padding = 15;
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
            DrawBorder(Color.Gray);
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            var damageable = (HitPointComponent)Player;
            Rectangle inner_rect = GlobalRect;
            inner_rect.Width = (int)(GlobalRect.Width * (damageable.CurrentHitPoints / damageable.MaxHitPoints));
            var source_rect = new Rectangle((int)(bar_loop / loop_period), 0, GameContent.Instance.testLiquid.Width / 2, GameContent.Instance.testLiquid.Height);
            ApplyDistort();

            spriteBatch.Draw(GameContent.Instance.testLiquid, inner_rect, source_rect, Color.Red);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        public override void DrawSelf()
        {
            DrawHealthBar();
        }

        void IUpdatable.Tick(float dt)
        {
            ContinueLoop();
        }
    }
}
