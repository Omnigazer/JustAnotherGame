using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Enums;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Character
{
    class SkillComponent : Component
    {
        public int SkillPoints { get; set; }
        public Dictionary<Skill, int> Skills = new Dictionary<Skill, int>();

        public SkillComponent()
        {
            SkillPoints = 4;
            foreach (Skill skill in Enum.GetValues(typeof(Skill)))
            {
                Skills.Add(skill, 0);
            }
        }

        public int GetSkill(Skill skill, bool modified = true)
        {
            if (modified)
            {
                var bonusable = GetComponent<BonusComponent>();
                return Skills[skill] + bonusable.SkillBonuses[skill].Sum();
            }
            else
            {
                return Skills[skill];
            }
        }

        public void UpgradeSkill(Skill skill)
        {
            if (!Skills.ContainsKey(skill))
            {
                Skills.Add(skill, 0);
            }

            if (SkillPoints >= Skills[skill] + 1)
            {
                SkillPoints -= Skills[skill] + 1;
                Skills[skill]++;
            }
        }
    }
}
