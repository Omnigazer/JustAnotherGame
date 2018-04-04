using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;

namespace Omniplatformer.Characters
{
    public class Zombie : Character
    {
        // internal counters for "random movement"
        int ticks = 0;
        int amp = 300;

        public bool Aggressive { get; set; }
        public Zombie(Vector2 center, Vector2 halfsize)
        {
            Team = Team.Enemy;
            CurrentHitPoints = MaxHitPoints = 8;

            Components.Add(new PositionComponent(this, center, halfsize));
            Components.Add(new CharacterRenderComponent(this, GameContent.Instance.characterLeft, GameContent.Instance.characterRight));
            Components.Add(new CharMoveComponent(this, movespeed: 1.4f));
            Components.Add(new DamageHitComponent(this, damage: 3));            
        }       

        public void MoveTowardsPlayer()
        {
            var player_pos = GameService.Player.GetComponent<PositionComponent>();
            var pos = GetComponent<PositionComponent>();
            var movable = GetComponent<CharMoveComponent>();
            movable.move_direction = pos.WorldPosition.Center.X < player_pos.WorldPosition.Center.X ? Direction.Right : Direction.Left;
        }

        public void WalkAbout()
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
            ticks = (ticks + 1) % amp;
        }

        public override void ApplyDamage(float damage)
        {
            Aggressive = true;
            base.ApplyDamage(damage);
        }

        public override void Tick()
        {            
            base.Tick();
            if (Aggressive)
            {
                MoveTowardsPlayer();
            }
            else
            {
                WalkAbout();
            }
        }        
    }
}
