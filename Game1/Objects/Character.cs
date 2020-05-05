using Omniplatformer.Components;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Services;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;
using System;

namespace Omniplatformer.Objects
{
    public abstract class Character : GameObject
    {
        // TODO: refactor this
        public virtual int ExpReward => 300;

        public Character() { }

        public override void onDestroy()
        {
            var expable = GameService.Player.GetComponent<ExperienceComponent>();
            // TODO: find another approach for earning exp
            expable.EarnExperience(ExpReward);
            base.onDestroy();
        }

        public void ApplyStun(float duration)
        {
            var cooldownable = GetComponent<CooldownComponent>();
            cooldownable.Cooldowns.SetOrAdd("Stun", duration);
        }
    }
}
