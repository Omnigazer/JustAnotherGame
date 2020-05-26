using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;
using Omniplatformer.Utility;
using Omniplatformer.Utility.Extensions;

namespace Omniplatformer.Components
{
    public class CooldownComponent : Component
    {
        public Dictionary<string, float> Cooldowns { get; set; } = new Dictionary<string, float>();

        public bool TryCooldown(string key, int value)
        {
            if (!Cooldowns.ContainsKey(key) || Cooldowns[key] <= 0)
            {
                Cooldowns.SetOrAdd(key, value);
                return true;
            }
            return false;
        }

        public override void Tick(float dt)
        {
            foreach (var (key, ticks) in Cooldowns.ToList())
            {
                Cooldowns[key] = Math.Max(ticks - dt, 0);
            }
        }
    }
}
