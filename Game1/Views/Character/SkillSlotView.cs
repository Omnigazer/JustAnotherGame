using Microsoft.Xna.Framework;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.HUDStates;
using Omniplatformer.Services;

namespace Omniplatformer.Views.Character
{
    public class SkillSlotView : ViewControl
    {
        public Skill Skill { get; set; }

        public SkillSlotView(Skill skill)
        {
            Skill = skill;
            MouseClick += SkillSlotView_MouseClick;
        }

        public override void SetupNode()
        {
            Width = 700;
            Height = 70;
            Margin = 5;
        }

        private void SkillSlotView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                GameService.Player.UpgradeSkill(Skill);
            }
        }

        public override void DrawSelf()
        {
            DrawSkill();
        }

        public void DrawSkill()
        {
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Draw(GameContent.Instance.whitePixel, GlobalRect, Color.Gray);
            spriteBatch.DrawString(GameContent.Instance.defaultFont, Skill.ToString(), GlobalRect.Location.ToVector2(), Color.White);
            spriteBatch.DrawString(GameContent.Instance.defaultFont, GameService.Player.GetSkill(Skill, false).ToString(), GlobalRect.Location.ToVector2() + new Vector2(200, 0), Color.White);
        }
    }
}
