using Omniplatformer.Components.Physics;
using Omniplatformer.Enums;
using Omniplatformer.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omniplatformer.Objects;
using Omniplatformer.Services;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Objects.Projectiles;

namespace Omniplatformer.Components.Behavior
{
    public class GoblinBehaviorComponent : BehaviorComponent {

        float current_dt;
        IEnumerator Behavior { get; }

        public GoblinBehaviorComponent(GameObject obj) : base(obj)
        {
            Behavior = WalkBehavior();
        }
        IEnumerator WalkBehavior()
        {
            var movable = GetComponent<DynamicPhysicsComponent>();
            while (true)
            {
                float walk_time = RandomGen.NextFloat(150, 600);
                movable.MoveDirection = Direction.Right;
                for (float t = 0; t < walk_time; t += current_dt)
                {
                    yield return null;
                }

                movable.MoveDirection = Direction.None;
                for (float t = 0; t < RandomGen.NextFloat(700, 2000); t += current_dt)
                {
                    yield return null;
                }

                movable.MoveDirection = Direction.Left;
                for (float t = 0; t < walk_time; t += current_dt)
                {
                    yield return null;
                }

                movable.MoveDirection = Direction.None;
                for (float t = 0; t < RandomGen.NextFloat(700, 2000); t += current_dt)
                {
                    yield return null;
                }
            }
        }

        public void AttackPlayer()
        {
            var movable = GetComponent<CharMoveComponent>();
            var player_pos = GameService.Player.GetComponent<PositionComponent>();
            var pos = GetComponent<PositionComponent>();
            var attackable = GetComponent<RangedAttackComponent>();

            if (attackable.CanAttack())
                attackable.Attack();

            if (!attackable.IsAttacking)
            {
                movable.MoveDirection = pos.WorldPosition.Center.X < player_pos.WorldPosition.Center.X ? Direction.Right : Direction.Left;
            }
            else
            {
                movable.MoveDirection = Direction.None;
            }
        }

        public bool CheckStun()
        {
            var cooldownable = GetComponent<CooldownComponent>();

            var movable = GetComponent<CharMoveComponent>();
            if (cooldownable.Cooldowns.TryGetValue("Stun", out float val) && val > 0)
            {
                movable.MoveDirection = Direction.None;
                return true;
            }

            return false;
        }

        public override void Tick(float dt)
        {
            if (CheckStun())
                return;
            current_dt = dt;
            if (Aggressive)
            {
                AttackPlayer();
            }
            else
            {
                Behavior.MoveNext();
            }
        }
    }
}
