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


namespace Omniplatformer.Spells
{
    class LifeDrain
    {
        public static void Cast(Player player)
        {
            if (player.SpendMana(ManaType.Death, 1))
            {
                PositionComponent pos = (PositionComponent)player;
                int dir_sign = (int)pos.WorldPosition.face_direction;
                var projectile = new LifeDrainProjectile(pos.WorldPosition.Center, new Vector2(5 * dir_sign, 0), player);
                GameService.Instance.AddToMainScene(projectile);
            }
        }
    }
}
