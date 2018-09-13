using System;
using System.Collections;
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
    public class Goblin : Character
    {
        // internal counters for "random movement"
        /*
        float ticks = 0;
        int amp = 300;
        */

        float current_time_scale;
        IEnumerator behaviorGen()
        {
            var movable = GetComponent<DynamicPhysicsComponent>();
            while (true)
            {
                float walk_time = RandomGen.NextFloat(150, 600);
                movable.move_direction = Direction.Right;
                for (float t = 0; t < walk_time; t += current_time_scale)
                {
                    yield return null;
                }

                movable.move_direction = Direction.None;
                for (float t = 0; t < RandomGen.NextFloat(700, 2000); t += current_time_scale)
                {
                    yield return null;
                }

                movable.move_direction = Direction.Left;
                for (float t = 0; t < walk_time; t += current_time_scale)
                {
                    yield return null;
                }

                movable.move_direction = Direction.None;
                for (float t = 0; t < RandomGen.NextFloat(700, 2000); t += current_time_scale)
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
            var halfsize = new Vector2(15, 20);
            // Components.Add(new PositionComponent(this, coords, halfsize));
            Components.Add(new CharMoveComponent(this, coords, halfsize, movespeed: 1.8f));
            Components.Add(new CharacterRenderComponent(this, GameContent.Instance.characterLeft, GameContent.Instance.characterRight, Color.Green));
            Components.Add(new DamageHitComponent(this, damage: 3, knockback: new Vector2(5, 5)));
        }

        public void MoveTowardsPlayer()
        {
            var player_pos = GameService.Player.GetComponent<PositionComponent>();
            var pos = GetComponent<PositionComponent>();
            var movable = GetComponent<CharMoveComponent>();
            if (Cooldowns.TryGetValue("Stun", out float val) && val > 0)
            {
                movable.move_direction = Direction.None;
            }
            else
            {
                movable.move_direction = pos.WorldPosition.Center.X < player_pos.WorldPosition.Center.X ? Direction.Right : Direction.Left;
            }
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
            foreach (var obj in Game.GetObjectsAroundPosition(pos.WorldPosition, 1000))
            {
                if (obj is Character)
                {
                    var character = obj as Character;
                    character.Aggressive = true;
                }
            }
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
                // WalkAbout();
                current_time_scale = time_scale;
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
