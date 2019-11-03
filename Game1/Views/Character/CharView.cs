using System;
using Omniplatformer.Enums;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Services;

namespace Omniplatformer.Views.Character
{
    /// <summary>
    /// Handles the inventory view logic
    /// </summary>
    public class CharView : ViewControl
    {
        Player Player => GameService.Player;

        public CharView()
        {
            RegisterChild(new SkillPointsView());
            foreach (Skill skill in Enum.GetValues(typeof(Skill)))
            {
                RegisterChild(new SkillSlotView(skill));
            }
        }

        public override void SetupNode()
        {
            Width = 700;
        }
    }
}
