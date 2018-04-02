using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;

namespace Omniplatformer
{
    public class Player : Character
    {
        // Character constants
        const float max_hitpoints = 10;
        const float max_mana = 10;
        const float mana_regen_rate = 0.01f;
        const int inv_frames = 40;

        public Dictionary<ManaType, float> CurrentMana { get; set; }
        public Dictionary<ManaType, float> MaxMana { get; set; }

        public Player(Vector2 center, Vector2 halfsize)
        {
            Components.Add(new PositionComponent(this, center, halfsize));
            Components.Add(new CharacterRenderComponent(this, GameContent.Instance.characterLeft, GameContent.Instance.characterRight));
            Components.Add(new PlayerMoveComponent(this));
            MaxHitPoints = max_hitpoints;
            CurrentHitPoints = MaxHitPoints;
            CurrentMana = new Dictionary<ManaType, float>();
            MaxMana = new Dictionary<ManaType, float>();
            foreach (ManaType type in Enum.GetValues(typeof(ManaType)))
            {
                MaxMana[type] = max_mana;
                CurrentMana[type] = MaxMana[type];
            }
        }

        public override void ApplyDamage(float damage)
        {
            if (Vulnerable)
            {
                Vulnerable = false;
                var drawable = GetComponent<CharacterRenderComponent>();
                drawable._onAnimationEnd += Drawable__onAnimationEnd;
                drawable.StartAnimation(Animation.Hit, inv_frames);
                // TODO: test
                var movable = GetComponent<CharMoveComponent>();
                var pos = GetComponent<PositionComponent>();
                int dir_sign = pos.WorldPosition.face_direction == Direction.Left ? -1 : 1;
                movable.CurrentMovement += new Vector2(-20 * dir_sign, 10);

                base.ApplyDamage(damage);
            }
        }

        private void Drawable__onAnimationEnd(object sender, AnimatedRenderComponent.AnimationEventArgs e)
        {
            var drawable = (CharacterRenderComponent)sender;
            if (e.animation == Animation.Hit)
            {
                drawable._onAnimationEnd -= Drawable__onAnimationEnd;
                Vulnerable = true;
            }
        }

        // Fit all per-frame instructions in here for now
        public override void Tick()
        {
            RegenerateMana();
            base.Tick();
        }

        #region Actions
        public Projectile Fire()
        {
            SpendMana(ManaType.Chaos, 1);

            PositionComponent pos = GetComponent<PositionComponent>();
            var x = new TestProjectile(pos.WorldPosition.Center, new Vector2(5, 5));
            // TODO: face_direction probably should be somewhere else
            var movable = GetComponent<CharMoveComponent>();
            var proj_movable = x.GetComponent<ProjectileMoveComponent>();
            if (pos.WorldPosition.face_direction == Direction.Left)
            {
                proj_movable.direction = new Vector2(-15, 0);
            }
            else
            {
                proj_movable.direction = new Vector2(15, 0);
            }
            return x;
        }

        #endregion

        #region Gameplay logic   

        public void SpendMana(ManaType type, float amount)
        {
            CurrentMana[type] -= amount;
            CurrentMana[type] = Math.Max(CurrentMana[type], 0);
        }

        public void RegenerateMana()
        {
            foreach (ManaType type in Enum.GetValues(typeof(ManaType)))
            {
                CurrentMana[type] += mana_regen_rate;
                CurrentMana[type] = Math.Min(CurrentMana[type], MaxMana[type]);
            }

        }

        // should be applying this to a component instead
        public void Pickup(Collectible item)
        {
            GetBonus(item.Bonus);
            item.onDestroy();
        }

        public void GetBonus(Bonus bonus)
        {
            switch (bonus)
            {
                case Bonus.Jump:
                    {
                        // TODO: Reference to a concrete class
                        var movable = GetComponent<PlayerMoveComponent>();
                        movable.max_jumps++;
                        break;
                    }
            }
        }


        #endregion
    }
}