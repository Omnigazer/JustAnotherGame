using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Omniplatformer.Spells
{
    class FireBolt
    {
        /*
        public static void Cast(Player player)
        {
            if (player.SpendMana(ManaType.Chaos, 1))
            {
                PositionComponent pos = (PositionComponent)player;
                var projectile = new FireBoltProjectile(pos.WorldPosition.Center, new Vector2(5, 5), player);
                var movable = (CharMoveComponent)player;
                var proj_movable = projectile.GetComponent<ProjectileMoveComponent>();
                int dir_sign = (int)pos.WorldPosition.face_direction;
                proj_movable.direction = new Vector2(15 * dir_sign, 0);
                GameService.Instance.RegisterObject(projectile);
            }
        }
        */

        public static void Cast(Character character)
        {
            if (character.SpendMana(ManaType.Chaos, 1))
            {
                PositionComponent pos = (PositionComponent)character;
                var projectile = new FireBoltProjectile(pos.WorldPosition.Center, new Vector2(5, 5), character);
                var movable = (CharMoveComponent)character;
                var proj_movable = projectile.GetComponent<ProjectileMoveComponent>();
                int dir_sign = (int)pos.WorldPosition.face_direction;
                proj_movable.direction = new Vector2(15 * dir_sign, 0);
                GameService.Instance.RegisterObject(projectile);
            }
        }
    }
}
