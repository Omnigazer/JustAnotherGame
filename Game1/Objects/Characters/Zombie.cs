using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Services;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Characters
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
            Components.Add(new CharacterRenderComponent(this, GameContent.Instance.character));
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

        public void WalkAbout(float dt)
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
            ticks = (ticks + dt) % amp;
        }

        public override void ApplyDamage(float damage)
        {
            Aggressive = true;
            base.ApplyDamage(damage);
        }

        public override void Tick(float dt)
        {
            base.Tick(dt);
            if (Aggressive)
            {
                MoveTowardsPlayer();
            }
            else
            {
                WalkAbout(dt);
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
