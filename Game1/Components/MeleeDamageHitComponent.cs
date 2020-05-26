using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Physics;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Characters;

namespace Omniplatformer.Components
{
    /// <summary>
    /// Takes into account melee skill of the player
    /// </summary>
    class MeleeDamageHitComponent : DamageHitComponent
    {
        public float Range { get; set; }

        public MeleeDamageHitComponent() { }
        public MeleeDamageHitComponent(int damage, float range = 0, Vector2? knockback = null) : base(damage, knockback)
        {
            // Range = range;
            Range = 60;
        }

        public void MeleeHit()
        {
            GameObject obj = GetMeleeTarget(Range);
            if (obj != null)
                Hit(obj);
        }

        protected GameObject GetMeleeTarget(float range)
        {
            var pos = GetComponent<PositionComponent>();
            return pos.GetClosestObject(new Vector2(range * (int)pos.WorldPosition.FaceDirection, 0), x => x.Hittable && x.GameObject.Team != Team.Friend);
        }

        protected override int DetermineDamage()
        {
            // TODO: could extract this into Skills.GetBonusDamage(Skill.Melee) or something
            int skill_value = 0;
            var skillable = this.GameObject.Source?.GetComponent<SkillComponent>();
            if (skillable != null)
            {
                skill_value = skillable.Skills.ContainsKey(Skill.Melee) ? skillable.Skills[Skill.Melee] : 0;
            }
            return base.DetermineDamage() + skill_value;
        }
    }
}
