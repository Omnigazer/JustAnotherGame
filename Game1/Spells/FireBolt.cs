using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Components.Physics;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Projectiles;
using Omniplatformer.Services;
using Omniplatformer.Components.Character;

namespace Omniplatformer.Spells
{
    class FireBolt
    {
        public static void Cast(GameObject caster, Position target)
        {
            var manable = caster.GetComponent<ManaComponent>();
            if (manable?.SpendMana(ManaType.Chaos, 0) ?? true)
            {
                PositionComponent pos = (PositionComponent)caster;
                Vector2 direction = target.Coords - pos.WorldPosition.Coords;
                var projectile = new FireBoltProjectile(pos.WorldPosition.Center, direction, caster);

                GameService.Instance.AddToMainScene(projectile);
            }
        }
    }
}
