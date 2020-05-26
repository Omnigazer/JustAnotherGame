using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Utility;

namespace Omniplatformer.Components.Character
{
    public class ManaComponent : Component
    {
        const float mana_regen_rate = 0.1f / 60;

        public Dictionary<ManaType, float> CurrentMana { get; set; } = new Dictionary<ManaType, float>();
        // public Dictionary<ManaType, float> MaxMana { get; set; }

        // TODO: possibly cache this
        [JsonIgnore]
        public Dictionary<Skill, int> Skills => GetComponent<SkillComponent>().Skills;

        public ManaComponent()
        {
            foreach (ManaType type in Enum.GetValues(typeof(ManaType)))
            {
                CurrentMana.Add(type, 0);
            }
        }

        public float MaxMana(ManaType manaType)
        {
            var x = (Skill)Enum.Parse(typeof(Skill), manaType.ToString());
            if (Skills.ContainsKey(x))
                return GetComponent<SkillComponent>().GetSkill(x);
            return 0;
        }

        public bool SpendMana(ManaType type, float amount)
        {
            if (CurrentMana[type] >= amount)
            {
                CurrentMana[type] -= amount;
                return true;
            }
            else
                return false;
        }

        public void ReplenishMana(ManaType type, float amount)
        {
            CurrentMana[type] += amount;
            CurrentMana[type] = CurrentMana[type].LimitToRange(0, MaxMana(type));
        }

        public void RegenerateMana(float dt)
        {
            foreach (ManaType type in Enum.GetValues(typeof(ManaType)))
            {
                var bonusable = GetComponent<BonusComponent>();
                CurrentMana[type] += (mana_regen_rate + bonusable.ManaRegenBonuses[type].Sum()) * dt;
                CurrentMana[type] = Math.Min(CurrentMana[type], MaxMana(type));
            }
        }

        public override void Tick(float dt)
        {
            RegenerateMana(dt);
        }
    }
}
