using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Objects.Projectiles;
using Omniplatformer.Services;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Characters
{
    public class Goblin : Character
    {
        // internal counters for "random movement"
        /*
        float ticks = 0;
        int amp = 300;
        */

        float current_dt;
        IEnumerator behaviorGen()
        {
            var movable = GetComponent<DynamicPhysicsComponent>();
            while (true)
            {
                float walk_time = RandomGen.NextFloat(150, 600);
                movable.move_direction = Direction.Right;
                for (float t = 0; t < walk_time; t += current_dt)
                {
                    yield return null;
                }

                movable.move_direction = Direction.None;
                for (float t = 0; t < RandomGen.NextFloat(700, 2000); t += current_dt)
                {
                    yield return null;
                }

                movable.move_direction = Direction.Left;
                for (float t = 0; t < walk_time; t += current_dt)
                {
                    yield return null;
                }

                movable.move_direction = Direction.None;
                for (float t = 0; t < RandomGen.NextFloat(700, 2000); t += current_dt)
                {
                    yield return null;
                }
            }
        }
        IEnumerator Behavior { get; set; }

        public Goblin(Vector2 coords)
        {
            Behavior = behaviorGen();
            Team = Team.Enemy;
            CurrentHitPoints = MaxHitPoints = 8;
            var halfsize = new Vector2(20, 26);
            // Components.Add(new PositionComponent(this, coords, halfsize));
            Components.Add(new CharMoveComponent(this, coords, halfsize, movespeed: 1.8f));
            Components.Add(new CharacterRenderComponent(this, GameContent.Instance.character, Color.Green));
            Components.Add(new DamageHitComponent(this, damage: 2, knockback: new Vector2(3, 2)));
        }

        public void MoveTowardsPlayer()
        {
            var player_pos = GameService.Player.GetComponent<PositionComponent>();
            var pos = GetComponent<PositionComponent>();
            var movable = GetComponent<CharMoveComponent>();
            var drawable = GetComponent<CharacterRenderComponent>();
            if (TryCooldown("Cast", 120))
            {
                float force = 30;
                var distance = player_pos.WorldPosition.Coords - pos.WorldPosition.Coords;
                var direction = BallisticsHelper.GetThrowVector(force, Boulder.InverseMass, distance.X, distance.Y);

                drawable.StartAnimation(AnimationType.Cast, 20);
                EventHandler<AnimationEventArgs> handler = null;
                handler = (sender, e) =>
                {
                    if (e.animation == AnimationType.Cast)
                    {
                        if (direction != null)
                        {
                            var boulder = new Boulder((pos.WorldPosition).Coords) { Team = Team.Enemy };
                            Game.AddToMainScene(boulder);
                            var b_movable = (DynamicPhysicsComponent)boulder;
                            b_movable.ApplyImpulse(direction.Value);
                        }

                        // Spells.FireBolt.Cast(this, player_pos.WorldPosition);
                        drawable._onAnimationEnd -= handler;
                    }
                };
                drawable._onAnimationEnd += handler;
            }
            if (Cooldowns.TryGetValue("Stun", out float val) && val > 0)
            {
                movable.move_direction = Direction.None;
            }
            else
            {
                movable.move_direction = pos.WorldPosition.Center.X < player_pos.WorldPosition.Center.X ? Direction.Right : Direction.Left;
            }
            movable.move_direction = Direction.None;
        }

        /*
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
        */

        public override void ApplyDamage(float damage)
        {
            Aggressive = true;
            var pos = GetComponent<PositionComponent>();

            // Aggravate everyone in the 1000 radius
            foreach (var obj in CurrentScene.PhysicsSystem.GetObjectsAroundPosition(pos.WorldPosition, 1000))
            {
                if (obj is Character)
                {
                    var character = obj as Character;
                    character.Aggressive = true;
                }
            }
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
                // WalkAbout();
                current_dt = dt;
                Behavior.MoveNext();
            }
        }

        public static GameObject FromJson(Deserializer deserializer)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(deserializer.getData());
            var goblin = new Goblin(coords);
            // SerializeService.Instance.RegisterObject(zombie);
            return goblin;
        }
    }
}
