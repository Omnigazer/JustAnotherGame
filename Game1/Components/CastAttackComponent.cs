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
using System.Reactive.Linq;
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
            var drawable = GetComponent<CharacterRenderComponent>();

            IsAttacking = true;
            drawable.onAnimationEnd.Where((animation_type) => animation_type == AnimationType.Cast)
                                    .FirstAsync().Subscribe((_) => CastAnimationFinished());
            drawable.StartAnimation(AnimationType.Cast, 30);
        }

        public void CastAnimationFinished()
        {
            var player_pos = GameService.Player.GetComponent<PositionComponent>();
            Spell.Cast(GetComponent<SpellCasterComponent>(), player_pos.WorldPosition);
            IsAttacking = false;
        }
    }
}
