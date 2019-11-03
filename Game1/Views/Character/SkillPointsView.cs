using Microsoft.Xna.Framework;
using Omniplatformer.Content;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Services;

namespace Omniplatformer.Views.Character
{
    public class SkillPointsView : ViewControl
    {
        Player Player => GameService.Instance.Player;
        public SkillPointsView()
        {

        }

        public override void SetupNode()
        {
            Width = 700;
            Height = 70;
            Margin = 5;
        }

        public override void DrawSelf()
        {
            DrawAvailablePoints();
        }

        public void DrawAvailablePoints()
        {
            var spriteBatch = GraphicsService.Instance;
            var points = Player.SkillPoints;
            var rect = GlobalRect;
            spriteBatch.Draw(GameContent.Instance.whitePixel, rect, Color.Gray);
            rect.Inflate(-5, -5);
            spriteBatch.DrawString(GameContent.Instance.defaultFont, "Skill Points", rect.Location.ToVector2(), Color.White);
            spriteBatch.DrawString(GameContent.Instance.defaultFont, points.ToString(), rect.Location.ToVector2() + new Vector2(200, 0), Color.White);
        }
    }
}
