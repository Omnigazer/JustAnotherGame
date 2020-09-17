using Omniplatformer.Components.Character;
using Omniplatformer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components.Items
{
    public class HealingConsumableComponent : Component
    {
        public float HealingValue { get; set; }

        public void ApplyEffect() { GameService.Player.GetComponent<HitPointComponent>().ApplyDamage(-HealingValue); }
    }
}
