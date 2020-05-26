using Omniplatformer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Physics;
using Microsoft.Xna.Framework;
using Omniplatformer.Objects.Projectiles;
using Omniplatformer.Services;

namespace Omniplatformer.Components
{
    public class FireBoltSpellComponent : SpellComponent
    {
        public override void Cast(SpellCasterComponent caster, Position target)
        {
            var manable = caster.GetComponent<ManaComponent>();
            if (!(manable?.SpendMana(ManaType.Chaos, 0) ?? true))
                return;

            PositionComponent pos = caster.GetComponent<PositionComponent>();
            Vector2 direction = target.Coords - pos.WorldPosition.Coords;
            var projectile = FireBoltProjectile.Create(caster.GameObject);
            projectile.GetComponent<PositionComponent>().SetLocalCoords(pos.WorldPosition.Coords);
            projectile.SetDirection(direction);

            GameService.Instance.AddToMainScene(projectile);
        }
    }
}
