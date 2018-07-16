﻿using System;
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
        int ticks = 0;
        int amp = 300;

        IEnumerator behaviorGen()
        {
            var movable = GetComponent<CharMoveComponent>();
            while (true)
            {
                int walk_time = RandomGen.Next(150, 600);
                movable.move_direction = Direction.Right;
                for (int i = 0; i < walk_time; i++)
                {
                    yield return null;
                }

                movable.move_direction = Direction.None;
                for (int i = 0; i < RandomGen.Next(700, 2000); i++)
                {
                    yield return null;
                }

                movable.move_direction = Direction.Left;
                for (int i = 0; i < walk_time; i++)
                {
                    yield return null;
                }

                movable.move_direction = Direction.None;
                for (int i = 0; i < RandomGen.Next(700, 2000); i++)
                {
                    yield return null;
                }
            }
        }
        IEnumerator Behavior { get; set; }

        public bool Aggressive { get; set; }
        public Goblin(Vector2 coords)
        {
            Behavior = behaviorGen();
            Team = Team.Enemy;
            CurrentHitPoints = MaxHitPoints = 8;
            var halfsize = new Vector2(15, 20);
            Components.Add(new PositionComponent(this, coords, halfsize));
            Components.Add(new CharacterRenderComponent(this, GameContent.Instance.characterLeft, GameContent.Instance.characterRight, Color.Green));
            Components.Add(new CharMoveComponent(this, movespeed: 1.4f));
            Components.Add(new DamageHitComponent(this, damage: 3, knockback: new Vector2(5, 5)));
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
                // WalkAbout();
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