using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omniplatformer.Components;
using Omniplatformer.Enums;
using Omniplatformer.HUDStates;
using System;
using System.Collections.Generic;

namespace Omniplatformer.HUD
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
