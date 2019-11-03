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


namespace Omniplatformer.Spells
{
    class FireBolt
    {
        public static void Cast(Character caster, Position target)
        {
            if (caster.SpendMana(ManaType.Chaos, 0))
            {
                PositionComponent pos = (PositionComponent)caster;                
                Vector2 direction = target.Coords - pos.WorldPosition.Coords;
                var projectile = new FireBoltProjectile(pos.WorldPosition.Center, direction, caster);
                
                GameService.Instance.AddToMainScene(projectile);
            }
        }
    }
}
