using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components;
using Omniplatformer.Utility;

namespace Omniplatformer.Characters
{
    public class Zombie : Character
    {
        // internal counters for "random movement"
        float ticks = 0;
        int amp = 300;

        public Zombie(Vector2 coords)
        {
            Team = Team.Enemy;
            CurrentHitPoints = MaxHitPoints = 8;
            var halfsize = new Vector2(15, 20);
            // Components.Add(new PositionComponent(this, coords, halfsize));
            Components.Add(new CharacterRenderComponent(this, GameContent.Instance.characterLeft, GameContent.Instance.characterRight));
            Components.Add(new CharMoveComponent(this, coords, halfsize, movespeed: 1.4f));
            Components.Add(new DamageHitComponent(this, damage: 3, knockback: new Vector2(5, 5)));
        }

        public void MoveTowardsPlayer()
        {
            var player_pos = GameService.Player.GetComponent<PositionComponent>();
            var pos = GetComponent<PositionComponent>();
            var movable = GetComponent<CharMoveComponent>();
            movable.move_direction = pos.WorldPosition.Center.X < player_pos.WorldPosition.Center.X ? Direction.Right : Direction.Left;
        }

        public void WalkAbout(float time_scale)
        {
            var movable = GetComponent<CharMoveComponent>();

            if (ticks > amp / 2)
            {
                movable.move_direction = Direction.Right;
            }
            else
            {
                movable.move_direction = Direction.Left;
            }
            ticks = (ticks + time_scale) % amp;
        }

        public override void ApplyDamage(float damage)
        {
            Aggressive = true;
            base.ApplyDamage(damage);
        }

        public override void Tick(float time_scale)
        {
            base.Tick(time_scale);
            if (Aggressive)
            {
                MoveTowardsPlayer();
            }
            else
            {
                WalkAbout(time_scale);
            }
        }

        public static GameObject FromJson(Deserializer deserializer)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(deserializer.getData());
            var zombie = new Zombie(coords);
            // SerializeService.Instance.RegisterObject(zombie);
            return zombie;
        }
    }
}
