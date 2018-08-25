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

        public static void Cast(Character caster, Position target)
        {
            if (caster.SpendMana(ManaType.Chaos, 1))
            {
                PositionComponent pos = (PositionComponent)caster;
                var projectile = new FireBoltProjectile(pos.WorldPosition.Center, new Vector2(5, 5), caster);
                // var movable = (CharMoveComponent)caster;
                var proj_movable = projectile.GetComponent<ProjectileMoveComponent>();
                // int dir_sign = (int)pos.WorldPosition.face_direction;
                // Vector2 direction = new Vector2()
                float speed = 15;
                Vector2 direction = target.Coords - pos.WorldPosition.Coords;
                direction.Normalize();
                // proj_movable.direction = new Vector2(15 * dir_sign, 0);
                proj_movable.Direction = speed * direction;
                GameService.Instance.RegisterObject(projectile);
            }
        }
    }
}
