using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Utility;

namespace Omniplatformer.Components.Character
{
    public class ManaComponent : Component
    {
        const float mana_regen_rate = 0.1f / 60;

        float _currentHitPoints;
        public float CurrentHitPoints { get => _currentHitPoints; set => _currentHitPoints = value.LimitToRange(0, MaxHitPoints); }
        public float MaxHitPoints { get; set; }

        public Dictionary<ManaType, float> CurrentMana { get; set; } = new Dictionary<ManaType, float>();
        // public Dictionary<ManaType, float> MaxMana { get; set; }

        // TODO: possibly cache this
        public Dictionary<Skill, int> Skills => GetComponent<SkillComponent>().Skills;

        public ManaComponent(GameObject obj) : base(obj)
        {
            foreach (ManaType type in Enum.GetValues(typeof(ManaType)))
            {
                CurrentMana[type] = MaxMana(type);
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
    }
}
