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
        public static void Cast(Character caster, Position target)
        {
            if (caster.SpendMana(ManaType.Chaos, 0))
            {
                PositionComponent pos = (PositionComponent)caster;
                var projectile = new FireBoltProjectile(pos.WorldPosition.Center, new Vector2(5, 5), caster);
                // var movable = (CharMoveComponent)caster;
                var proj_movable = projectile.GetComponent<ProjectileMoveComponent>();
                // int dir_sign = (int)pos.WorldPosition.face_direction;
                // Vector2 direction = new Vector2()
                float speed = 20;
                Vector2 direction = target.Coords - pos.WorldPosition.Coords;
                direction.Normalize();
                // proj_movable.direction = new Vector2(15 * dir_sign, 0);
                proj_movable.Rotate(-(float)Math.Atan2(direction.Y, direction.X));
                proj_movable.Direction = speed * direction;
                GameService.Instance.AddToMainScene(projectile);
            }
        }
    }
}
