using Omniplatformer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;

namespace Omniplatformer.Components
{
    public class BonusComponent : Component
    {
        public Dictionary<Skill, List<int>> SkillBonuses { get; set; } = new Dictionary<Skill, List<int>>();
        public Dictionary<ManaType, List<float>> ManaRegenBonuses { get; set; } = new Dictionary<ManaType, List<float>>();

        public BonusComponent() { }
        public BonusComponent(GameObject obj) : base(obj)
        {
            foreach (Skill skill in Enum.GetValues(typeof(Skill)))
            {
                SkillBonuses[skill] = new List<int>();
            }

            foreach (ManaType type in Enum.GetValues(typeof(ManaType)))
            {
                ManaRegenBonuses[type] = new List<float>();
            }
        }
    }
}
