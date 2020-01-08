using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Components.Physics;
using Omniplatformer.Enums;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Objects.Projectiles;
using Omniplatformer.Services;
using Omniplatformer.Objects;
using Omniplatformer.Components.Character;

namespace Omniplatformer.Spells
{
    class LifeDrain
    {
        public static void Cast(GameObject caster)
        {
            var manable = caster.GetComponent<ManaComponent>();
            if (manable.SpendMana(ManaType.Death, 1))
            {
                PositionComponent pos = (PositionComponent)caster;
                int dir_sign = (int)pos.WorldPosition.FaceDirection;
                var projectile = new LifeDrainProjectile(pos.WorldPosition.Center, new Vector2(5 * dir_sign, 0), caster);
                GameService.Instance.AddToMainScene(projectile);
            }
        }
    }
}
