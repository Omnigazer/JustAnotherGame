using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components;
using Omniplatformer.Items;
using Omniplatformer.Utility;

namespace Omniplatformer.Characters
{
    public class GoblinShaman : Character
    {
        // internal counters for "random movement"
        int ticks = 0;
        int amp = 300;

        /// <summary>
        /// Busy flag, such as when it's casting a spell
        /// </summary>
        bool Busy { get; set; } = false;

        IEnumerator behaviorGen()
        {
            var movable = GetComponent<CharMoveComponent>();
            while (true)
            {
                int walk_time = RandomGen.Next(100, 150);
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

        public GoblinShaman(Vector2 coords)
        {
            Behavior = behaviorGen();
            Team = Team.Enemy;
            CurrentHitPoints = MaxHitPoints = 8;
            var halfsize = new Vector2(15, 20);
            // Components.Add(new PositionComponent(this, coords, halfsize));
            Components.Add(new CharMoveComponent(this, coords, halfsize, movespeed: 1.4f));
            Components.Add(new CharacterRenderComponent(this, GameContent.Instance.characterLeft, GameContent.Instance.characterRight, Color.Orange));
            Components.Add(new DamageHitComponent(this, damage: 3, knockback: new Vector2(5, 5)));
        }

        public void MoveTowardsPlayer()
        {
            var player_pos = GameService.Player.GetComponent<PositionComponent>();
            var pos = GetComponent<PositionComponent>();
            var movable = GetComponent<CharMoveComponent>();
            var drawable = GetComponent<CharacterRenderComponent>();
            if (Busy)
                movable.move_direction = Direction.None;
            else
                movable.move_direction = pos.WorldPosition.Center.X < player_pos.WorldPosition.Center.X ? Direction.Right : Direction.Left;
            if (TryCooldown("Cast", 120))
            {
                Busy = true;
                drawable.StartAnimation(AnimationType.Cast, 30);
                EventHandler<AnimationEventArgs> handler = null;
                handler = (sender, e) =>
                {
                    if (e.animation == AnimationType.Cast)
                    {
                        Spells.FireBolt.Cast(this, player_pos.WorldPosition);
                        Busy = false;
                        drawable._onAnimationEnd -= handler;
                    }
                };
                drawable._onAnimationEnd += handler;
                // drawable.on

            }
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

        public override void onDestroy()
        {
            // TODO: extract this into a drop component
            // WieldedItem drop = new WieldedItem(50);
            Item drop = new ChaosOrb();
            var pos = (PhysicsComponent)drop;
            pos.SetLocalCoords(GetComponent<PositionComponent>().WorldPosition.Coords);
            pos.Pickupable = true;
            Game.AddToMainScene(drop);

            base.onDestroy();
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
                Behavior.MoveNext();
            }
        }

        public static GameObject FromJson(Deserializer deserializer)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(deserializer.getData());
            var shaman = new GoblinShaman(coords);
            // SerializeService.Instance.RegisterObject(zombie);
            return shaman;
        }
    }
}
