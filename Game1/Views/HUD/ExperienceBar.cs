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
    public class ExperienceBar : ViewControl, IUpdatable
    {
        public ExperienceComponent Player => GameService.Player.GetComponent<ExperienceComponent>();

        public ExperienceBar() { }

        public override void SetupNode()
        {
            Width = 400;
            Height = 30;
            BorderThickness = 5;
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

        void DrawBar()
        {
            DrawBorder(Color.Gray);
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            int current_experience = Player.CurrentExperience, max_experience = Player.MaxExperience;

            Rectangle inner_rect = GlobalRect;
            inner_rect.Width = (int)(Width * ((float)current_experience / max_experience));

            var source_rect = new Rectangle((int)(bar_loop / loop_period), 0, GameContent.Instance.testLiquid.Width / 2, GameContent.Instance.testLiquid.Height);
            ApplyDistort();

            spriteBatch.Draw(GameContent.Instance.testLiquid, inner_rect, source_rect, Color.Beige);
            // draw caustics over the bar
            source_rect = new Rectangle(0, 0, GameContent.Instance.causticsMap.Width, GameContent.Instance.causticsMap.Height / 4);
            spriteBatch.Draw(GameContent.Instance.causticsMap, inner_rect, source_rect, Color.White);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        public override void DrawSelf()
        {
            DrawBar();
        }

        void IUpdatable.Tick(float dt)
        {
            ContinueLoop();
        }
    }
}
