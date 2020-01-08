using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Enums;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Character
{
    public class ExperienceComponent : Component
    {
        // RPG elements
        public int CurrentExperience { get; set; }
        public int MaxExperience { get; set; } = 1000; // first-level max-experience
        public int Level { get; set; }

        public ExperienceComponent(GameObject obj) : base(obj)
        {

        }

        public void EarnExperience(int value)
        {
            CurrentExperience += value;
            while (CurrentExperience > MaxExperience)
            {
                LevelUp();
            }
        }

        public void LevelUp()
        {
            Level++;
            CurrentExperience -= MaxExperience;
            MaxExperience += 1000 * Level;

            // Increase some basic stats
            var hit_points = GetComponent<HitPointComponent>();
            hit_points.MaxHitPoints += 2;
            hit_points.CurrentHitPoints += 2;

            // Increase skill points
            GetComponent<SkillComponent>().SkillPoints += 4;
        }
    }
}
