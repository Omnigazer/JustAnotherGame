using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    class ExperienceBar : ManaBar
    {
        public ExperienceBar(Point position, int width, int height) : base(position, width, height)
        {
        }

        public override void Draw()
        {
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            // int bar_border_thickness = 5;
            Point border_size = new Point(Thickness, Thickness);
            Point size = new Point(Width, Height);
            int current_experience = Player.CurrentExperience, max_experience = Player.MaxExperience;
            Point current_size = new Point((int)((Width - Thickness * 2) * ((float)current_experience / max_experience)),
                Height - Thickness * 2);

            Rectangle outer_rect = new Rectangle(Position, size);
            Rectangle inner_rect = new Rectangle(Position + border_size, current_size);

            var source_rect = new Rectangle((int)(bar_loop / loop_period), 0, GameContent.Instance.testLiquid.Width / 2, GameContent.Instance.testLiquid.Height);
            spriteBatch.Draw(GameContent.Instance.whitePixel, outer_rect, Color.Gray);

            // Apply the distort effect
            ApplyDistort();

            spriteBatch.Draw(GameContent.Instance.testLiquid, inner_rect, source_rect, Color.Beige);
            // draw caustics over the bar
            source_rect = new Rectangle(0, 0, GameContent.Instance.causticsMap.Width, GameContent.Instance.causticsMap.Height / 4);
            spriteBatch.Draw(GameContent.Instance.causticsMap, inner_rect, source_rect, Color.White);
            spriteBatch.End();
            base.Draw();
        }
    }
}
