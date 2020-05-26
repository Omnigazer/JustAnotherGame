using Omniplatformer.Components.Character;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Services;
using Omniplatformer.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public class CastAttackComponent : RangedAttackComponent
    {
        SpellComponent Spell { get; set; }
        public CastAttackComponent(SpellComponent spell)
        {
            Spell = spell;
        }

        public override bool CanAttack()
        {
            // float force = 30;
            var cooldownable = GetComponent<CooldownComponent>();
            if (cooldownable.TryCooldown("Cast", 120))
                return true;
            return false;
        }

        public override void Attack()
        {
            var player_pos = GameService.Player.GetComponent<PositionComponent>();
            var drawable = GetComponent<CharacterRenderComponent>();

            IsAttacking = true;
            drawable.StartAnimation(AnimationType.Cast, 30);
            void Handler(object sender, AnimationEventArgs e)
            {
                if (e.animation == AnimationType.Cast)
                {
                    Spell.Cast(GetComponent<SpellCasterComponent>(), player_pos.WorldPosition);
                    IsAttacking = false;
                    drawable._onAnimationEnd -= Handler;
                }
            };
            drawable._onAnimationEnd += Handler;
            // drawable.on
        }
    }
}
