using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omniplatformer.Enums;

namespace Omniplatformer.Components
{
    /// <summary>
    /// Takes into account melee skill of the player
    /// </summary>
    class MeleeDamageHitComponent : DamageHitComponent
    {
        public MeleeDamageHitComponent(GameObject obj, int damage) : base(obj, damage)
        {
        }

        public MeleeDamageHitComponent(GameObject obj, int damage, Vector2 knockback) : base(obj, damage, knockback)
        {
        }

        protected override int DetermineDamage()
        {
            // TODO: be ready to access the component's ultimate source and get the skill from there (if any)
            // also could extract this into Skills.GetBonusDamage(Skill.Melee) or something
            // GameService.Player.Skills.TryGetValue(Skill.Melee, out int skill_value);
            int skill_value = 0;
            // TODO: possibly extract skill to characters
            var source = this.GameObject.Source as Player;
            if (source != null)
            {
                skill_value = source.Skills.ContainsKey(Skill.Melee) ? source.Skills[Skill.Melee] : 0;
            }
            return base.DetermineDamage() + skill_value;
        }
    }
}
