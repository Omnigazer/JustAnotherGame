using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Projectiles;
using Omniplatformer.Services;
using Omniplatformer.Utility;

namespace Omniplatformer.Components
{
    public class ThrowAttackComponent : RangedAttackComponent
    {
        public override bool CanAttack()
        {
            float force = 30;
            var cooldownable = GetComponent<CooldownComponent>();
            var player_pos = GameService.Player.GetComponent<PositionComponent>();
            var pos = GetComponent<PositionComponent>();
            var distance = player_pos.WorldPosition.Coords - pos.WorldPosition.Coords;
            var direction = BallisticsHelper.GetThrowVector(force, Boulder.InverseMass, distance.X, distance.Y);
            if (direction == null || !cooldownable.TryCooldown("Cast", 120))
                return false;
            return true;
        }

        public override void Attack()
        {
            float force = 30;
            var player_pos = GameService.Player.GetComponent<PositionComponent>();
            var pos = GetComponent<PositionComponent>();
            var distance = player_pos.WorldPosition.Coords - pos.WorldPosition.Coords;
            var direction = BallisticsHelper.GetThrowVector(force, Boulder.InverseMass, distance.X, distance.Y);
            var drawable = GetComponent<CharacterRenderComponent>();
            var movable = GetComponent<CharMoveComponent>();

            drawable.StartAnimation(AnimationType.Cast, 20);
            IsAttacking = true;

            void Handler(object sender, AnimationEventArgs e)
            {
                if (e.animation == AnimationType.Cast)
                {
                    if (direction != null)
                    {
                        var boulder = Boulder.Create();
                        boulder
                            .GetComponent<PositionComponent>()
                            .SetLocalCoords((pos.WorldPosition).Coords);
                        boulder.Team = Team.Enemy;
                        GameService.Instance.AddToMainScene(boulder);
                        var b_movable = (DynamicPhysicsComponent)boulder;
                        b_movable.ApplyImpulse(direction.Value);
                    }

                    IsAttacking = false;
                    drawable._onAnimationEnd -= Handler;
                }
            }

            drawable._onAnimationEnd += Handler;
        }
    }
}
