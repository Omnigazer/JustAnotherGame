using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Omniplatformer.Spells
{
    class LifeDrain
    {
        public static void Cast(Player player)
        {
            if (player.SpendMana(ManaType.Death, 1))
            {
                PositionComponent pos = (PositionComponent)player;
                var projectile = new LifeDrainProjectile(pos.WorldPosition.Center, new Vector2(20, 5), player);
                var movable = (CharMoveComponent)player;
                var proj_movable = projectile.GetComponent<ProjectileMoveComponent>();
                int dir_sign = (int)pos.WorldPosition.face_direction;
                proj_movable.Direction = new Vector2(5 * dir_sign, 0);
                GameService.Instance.AddToMainScene(projectile);
            }
        }
    }
}
