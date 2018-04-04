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

        public bool ItemLocked { get; set; }
        public WieldedItem WieldedItem { get; private set; }

        public Dictionary<ManaType, float> CurrentMana { get; set; }
        public Dictionary<ManaType, float> MaxMana { get; set; }

        public Player(Vector2 center, Vector2 halfsize)
        {
            Team = Team.Friend;            
            MaxHitPoints = max_hitpoints;
            CurrentHitPoints = MaxHitPoints;
            CurrentMana = new Dictionary<ManaType, float>();
            MaxMana = new Dictionary<ManaType, float>();
            foreach (ManaType type in Enum.GetValues(typeof(ManaType)))
            {
                MaxMana[type] = max_mana;
                CurrentMana[type] = MaxMana[type];
            }

            Components.Add(new PositionComponent(this, center, halfsize));
            Components.Add(new CharacterRenderComponent(this, GameContent.Instance.characterLeft, GameContent.Instance.characterRight));
            Components.Add(new PlayerMoveComponent(this));
        }

        public override void ApplyDamage(float damage)
        {
            if (Vulnerable)
            {
                Vulnerable = false;
                CurrentHitPoints -= damage;
                var drawable = GetComponent<CharacterRenderComponent>();
                drawable._onAnimationEnd += Drawable__onAnimationEnd;
                drawable.StartAnimation(Animation.Hit, inv_frames);
                // TODO: test
                Knockback(new Vector2(-20, 10));                             
                
                if (CurrentHitPoints <= 0)
                {
                    onDestroy();
                }
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
            int dir_sign = (int)pos.WorldPosition.face_direction;            
            proj_movable.direction = new Vector2(15 * dir_sign, 0);            
            return x;
        }

        // TODO: extract this somewhere
        public void Knockback(Vector2 vector)
        {
            var movable = GetComponent<CharMoveComponent>();
            var pos = GetComponent<PositionComponent>();            
            vector.X *= (int)pos.WorldPosition.face_direction;
            movable.CurrentMovement += vector;
        }

        public void Swing()
        {
            if (WieldedItem != null)
            {
                ItemLocked = true;
                var drawable = GetComponent<CharacterRenderComponent>();
                drawable._onAnimationEnd += onAttackend;
                drawable.StartAnimation(Animation.Attack, 10);
            }            
        }    

        public void MeleeHit()
        {            
            GameObject obj = GetMeleeTarget(range: 60);            
            // TODO: get some weapon values here
            var (damage, knockback) = (WieldedItem.Damage, new Vector2(3, 3));
            // TODO: extract ApplyDamage and add relevant values to it
            if (obj != null)
            {
                // damaging the target
                obj.ApplyDamage(damage);
                // applying knockback
                var movable = (MoveComponent)obj;
                var pos = GetComponent<PositionComponent>();
                movable?.AdjustSpeed(new Vector2((int)pos.WorldPosition.face_direction * knockback.X, knockback.Y));
            }           
        }           

        public GameObject GetMeleeTarget(float range)
        {
            var pos = GetComponent<PositionComponent>();            
            return pos.GetClosestObject(new Vector2(range * (int)pos.WorldPosition.face_direction, 0));
        }

        private void onAttackend(object sender, AnimatedRenderComponent.AnimationEventArgs e)
        {
            var drawable = GetComponent<CharacterRenderComponent>();
            drawable._onAnimationEnd -= onAttackend;
            if (e.animation == Animation.Attack)
            {
                MeleeHit();
                ItemLocked = false;
            }            
        }

        #endregion

        #region Gameplay logic   

        public void ReplenishMana(ManaType type, float amount)
        {
            CurrentMana[type] += amount;
            CurrentMana[type] = Math.Min(CurrentMana[type], MaxMana[type]);
        }

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

        public void ToggleItem(WieldedItem item)
        {
            if (WieldedItem == item)
            {
                UnwieldItem();
            }
            else
            {
                WieldItem(item);
            }
        }

        public void WieldItem(WieldedItem item)
        {
            if (!ItemLocked)
            {
                WieldedItem = item;
                var item_pos = (PositionComponent)item;
                item_pos.SetParent(this, AnchorPoint.Hand);
            }                        
        }

        public void UnwieldItem()
        {
            if (!ItemLocked)
            {
                var item_pos = (PositionComponent)WieldedItem;
                item_pos.ClearParent();
                WieldedItem = null;
            }            
        }


        #endregion
    }
}