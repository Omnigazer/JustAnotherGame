using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Services;

namespace Omniplatformer.Components.Character
{
    public class YieldExperienceComponent : Component
    {
        public int Value { get; set; }

        public override void Compile()
        {
            GameObject.GetComponent<DestructibleComponent>().OnDestroy.Subscribe((obj) => YieldExperience());
        }

        public void YieldExperience()
        {
            var expable = GameService.Player.GetComponent<ExperienceComponent>();
            // TODO: find another approach for earning exp
            expable.EarnExperience(Value);
        }
    }
}
