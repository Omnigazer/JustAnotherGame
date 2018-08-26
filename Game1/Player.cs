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
using Omniplatformer.Enums;
using Omniplatformer.Items;
using static Omniplatformer.Enums.Skill;

namespace Omniplatformer
{
    public class Player : Character
    {
        // Character constants
        const float max_hitpoints = 5;
        const float max_mana = 10;
        const float mana_regen_rate = 0.05f / 30;
        const int inv_frames = 100;

        public bool ItemLocked { get; set; }
        public WieldedItem WieldedItem { get => (WieldedItem)EquipSlots.HandSlot.Item; private set => WieldItem(value); }

        // Equipment and inventory
        public Inventory inventory;
        public EquipSlotCollection EquipSlots;


        //

        // RPG elements
        public int CurrentExperience { get; set; }
        public int MaxExperience { get; set; } = 1000; // first-level max-experience
        public int Level { get; set; }
        public int SkillPoints { get; set; }

        public Dictionary<Skill, int> Skills = new Dictionary<Skill, int>();

        // spans from 0.6 to 0.2 with a weight of 20
        public float MeleeAttackRate => (float)(60 - 40 * ((float)Skills[Melee] / (Skills[Melee] + 20))) / 100f;

        public Dictionary<ManaType, float> CurrentMana { get; set; }
        // public Dictionary<ManaType, float> MaxMana { get; set; }

        public float MaxMana(ManaType manaType)
        {
            var x = (Skill)Enum.Parse(typeof(Skill), manaType.ToString());
            // var x = typeof(Enum.Parse(Skill, manaType.ToString()));
            if (Skills.ContainsKey(x))
                return Skills[x];
            return 0;
        }

        public Player(Vector2 center, Vector2 halfsize)
        {
            foreach (Skill skill in Enum.GetValues(typeof(Skill)))
            {
                Skills.Add(skill, 0);
            }
            EquipSlots = new EquipSlotCollection();
            inventory = new Inventory();
            // TODO: test
            var item = new WieldedItem(damage: 1);
            Game.RegisterObject(item);
            item.Hide();
            inventory.AddItem(item);

            Team = Team.Friend;
            MaxHitPoints = max_hitpoints;
            CurrentHitPoints = MaxHitPoints;
            CurrentMana = new Dictionary<ManaType, float>();
            SkillPoints = 10;
            //MaxMana = new Dictionary<ManaType, float>();
            foreach (ManaType type in Enum.GetValues(typeof(ManaType)))
            {
                //MaxMana[type] = max_mana;
                CurrentMana[type] = MaxMana(type);
            }

            InitPos(center, halfsize);
            Components.Add(new CharacterRenderComponent(this, GameContent.Instance.characterLeft, GameContent.Instance.characterRight));
            Components.Add(new PlayerMoveComponent(this) { MaxMoveSpeed = 9, Acceleration = 0.5f });
        }

        public void InitPos(Vector2 center, Vector2 halfsize)
        {
            var pos = new PositionComponent(this, center, halfsize);
            pos.AddAnchor(AnchorPoint.Hand, new Position(new Vector2(0.4f, 0.21f), Vector2.Zero));
            Components.Add(pos);
        }

        public void EarnExperience(int value)
        {
            CurrentExperience += value;
            while (CurrentExperience > MaxExperience)
            {
                LevelUp();
            }
        }

        public void LevelUp()
        {
            Level++;
            CurrentExperience -= MaxExperience;
            MaxExperience += 1000 * Level;

            // Increase some basic stats
            MaxHitPoints += 2;
            CurrentHitPoints += 2;

            // Increase skill points
            SkillPoints += 5;

            /*
            if (!Skills.ContainsKey(Skill.Melee))
                Skills.Add(Skill.Melee, 0);
            Skills[Skill.Melee] += 3;
            */
        }

        public void UpgradeSkill(Skill skill)
        {
            if (!Skills.ContainsKey(skill))
            {
                Skills.Add(skill, 0);
            }

            if (SkillPoints >= Skills[skill] + 1)
            {
                SkillPoints -= Skills[skill] + 1;
                Skills[skill]++;
            }
        }

        public override void ApplyDamage(float damage)
        {
            if (Vulnerable || damage <= 0)
            {
                if (damage >= 0)
                    Vulnerable = false;
                CurrentHitPoints -= damage;
                var drawable = GetComponent<CharacterRenderComponent>();
                drawable._onAnimationEnd += Drawable__onAnimationEnd;
                drawable.StartAnimation(Animation.Hit, inv_frames);

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
        public void Fire()
        {
            // Spells.FireBolt.Cast(this);
            Spells.LifeDrain.Cast(this);
        }

        public void Swing()
        {
            if (WieldedItem != null && TryCooldown("Melee", (int)(30 * MeleeAttackRate)))
            {
                ItemLocked = true;
                var drawable = GetComponent<CharacterRenderComponent>();
                drawable._onAnimationHit += onAttackend;
                drawable.StartAnimation(Animation.Attack, Cooldowns["Melee"]);
                // drawable.StartAnimation(Animation.Attack, 10);
            }
        }

        public void MeleeHit()
        {
            GameObject obj = GetMeleeTarget(range: 60);
            var damager = (HitComponent)WieldedItem;
            if (obj != null)
                damager?.Hit(obj);


            /*
            var (damage, knockback) = (WieldedItem.Damage, WieldedItem.Knockback);
            if (obj != null)
            {
                // damaging the target
                obj.ApplyDamage(damage);
                // applying knockback
                var movable = (MoveComponent)obj;
                var pos = GetComponent<PositionComponent>();
                movable?.AdjustSpeed(new Vector2((int)pos.WorldPosition.face_direction * knockback.X, knockback.Y));
            }
            */

        }

        public GameObject GetMeleeTarget(float range)
        {
            var pos = GetComponent<PositionComponent>();
            return pos.GetClosestObject(new Vector2(range * (int)pos.WorldPosition.face_direction, 0), x => x.Hittable && x.Team != Team.Friend);
        }

        private void onAttackend(object sender, AnimatedRenderComponent.AnimationEventArgs e)
        {
            var drawable = GetComponent<CharacterRenderComponent>();
            drawable._onAnimationHit -= onAttackend;
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
            CurrentMana[type] = Math.Min(CurrentMana[type], MaxMana(type));
        }

        public override bool SpendMana(ManaType type, float amount)
        {
            if (CurrentMana[type] >= amount)
            {
                CurrentMana[type] -= amount;
                return true;
            }
            else
                return false;
        }

        public void RegenerateMana()
        {
            foreach (ManaType type in Enum.GetValues(typeof(ManaType)))
            {
                CurrentMana[type] += mana_regen_rate;
                CurrentMana[type] = Math.Min(CurrentMana[type], MaxMana(type));
            }

        }

        // should be applying this to a component instead
        // public void Pickup(Collectible item)
        // TODO: find a way to avoid a check for collective/item
        public void Pickup(GameObject item)
        {
            if (item is Collectible)
            {
                var x = item as Collectible;
                GetBonus(x.Bonus);
                x.onDestroy();
            }
            else if(item is Item)
            {
                // TODO: move the inventory into the player class
                // all this code should be in player's pickup
                var x = item as Item;
                PickUp(x);
                x.Pickupable = false;
                item.Hide();
            }
            // GetBonus(item.Bonus);
            // item.onDestroy();
        }

        public void GetBonus(Bonus bonus)
        {
            switch (bonus)
            {
                case Bonus.Jump:
                    {
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
                if (WieldedItem != null)
                    UnwieldItem();
                EquipSlots.HandSlot.Item = item;
                // WieldedItem = item;
                item.OnEquip(this);
            }
        }

        public void UnwieldItem()
        {
            if (!ItemLocked)
            {
                // TODO: should only be executed if it's actually hidden in the inventory or something
                EquipSlots.HandSlot.Item = null;
                // WieldedItem = null;
            }
        }

        public void PickUp(Item item)
        {
            inventory.AddItem(item);
        }

        public void WieldCurrentSlot()
        {
            // inventory.AddItem(sword);
            if (!ItemLocked)
            {
                // inventory.AddItem(new WieldedItem(10, GameContent.Instance.bolt));
                var item = inventory.CurrentSlot.Item;
                inventory.CurrentSlot.Item = WieldedItem;
                if (item != null)
                {
                    WieldItem((WieldedItem)item);
                }
                else if (WieldedItem != null)
                {
                    UnwieldItem();
                }
            }
        }

        #endregion
    }
}