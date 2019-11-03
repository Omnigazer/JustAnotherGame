using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Objects.Interactibles;
using Omniplatformer.Objects.Inventory;
using Omniplatformer.Objects.Items;
using Omniplatformer.Objects.Projectiles;
using Omniplatformer.Services;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;
using static Omniplatformer.Enums.Skill;

namespace Omniplatformer.Objects.Characters
{
    public class Player : Character
    {
        // Character constants
        const float max_hitpoints = 50;
        const float max_mana = 10;
        const float mana_regen_rate = 0.1f / 60;
        const int inv_frames = 100;

        public bool EquipLocked { get; set; }
        public bool Blocking { get; set; }

        // public WieldedItem WieldedItem { get => (WieldedItem)EquipSlots.RightHandSlot.Item; private set => WieldItem(value); }
        public WieldedItem WieldedItem => (WieldedItem)EquipSlots.RightHandSlot.Item;

        // Equipment and inventory
        public Inventory.Inventory Inventory { get; set; } = new Inventory.Inventory();
        public EquipSlotCollection EquipSlots { get; set; } = new EquipSlotCollection();


        //

        // RPG elements
        public int CurrentExperience { get; set; }
        public int MaxExperience { get; set; } = 1000; // first-level max-experience
        public int Level { get; set; }
        public int SkillPoints { get; set; }

        public Dictionary<Skill, int> Skills = new Dictionary<Skill, int>();

        // spans from 0.6 to 0.2 with a weight of 20
        public float MeleeAttackRate => (float)(60 - 40 * ((float)Skills[Melee] / (Skills[Melee] + 20))) / 100f;

        public Dictionary<ManaType, float> CurrentMana { get; set; } = new Dictionary<ManaType, float>();
        // public Dictionary<ManaType, float> MaxMana { get; set; }

        public Player()
        {
            InitComponents();
            Team = Team.Friend;
            MaxHitPoints = max_hitpoints;
            CurrentHitPoints = MaxHitPoints;
            SkillPoints = 4;

            foreach (Skill skill in Enum.GetValues(typeof(Skill)))
            {
                Skills.Add(skill, 0);
            }

            foreach (ManaType type in Enum.GetValues(typeof(ManaType)))
            {
                CurrentMana[type] = MaxMana(type);
            }
        }

        void InitComponents()
        {
            var halfsize = new Vector2(20, 36);
            var phys = new PlayerMoveComponent(this, Vector2.Zero, halfsize) { MaxMoveSpeed = 9, Acceleration = 0.5f };
            phys.AddAnchor(AnchorPoint.RightHand, new Position(new Vector2(0.4f, 0.21f), Vector2.Zero));
            phys.AddAnchor(AnchorPoint.LeftHand, new Position(new Vector2(-0.45f, -0.05f), Vector2.Zero, 0.6f));
            Components.Add(phys);
            Components.Add(new CharacterRenderComponent(this, GameContent.Instance.character));
            Components.Add(new BonusComponent(this));
        }

        public void StartBlocking()
        {
            if(EquipSlots.LeftHandSlot.Item == null || EquipLocked)
                return;

            Blocking = true;
            var pos1 = (PositionComponent)EquipSlots.RightHandSlot.Item;
            pos1?.SetParent(this, AnchorPoint.LeftHand);

            var pos2 = (PositionComponent)EquipSlots.LeftHandSlot.Item;
            pos2.SetParent(this, AnchorPoint.RightHand);
        }

        public void StopBlocking()
        {
            if (EquipSlots.LeftHandSlot.Item == null)
                return;

            Blocking = false;
            var pos1 = (PositionComponent)EquipSlots.RightHandSlot.Item;
            pos1?.SetParent(this, AnchorPoint.RightHand);

            var pos2 = (PositionComponent)EquipSlots.LeftHandSlot.Item;
            pos2.SetParent(this, AnchorPoint.LeftHand);
        }

        public float MaxMana(ManaType manaType)
        {
            var x = (Skill)Enum.Parse(typeof(Skill), manaType.ToString());
            if (Skills.ContainsKey(x))
                return GetSkill(x);
            return 0;
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
            SkillPoints += 4;
        }

        public int GetSkill(Skill skill, bool modified = true)
        {
            if (modified)
            {
                var bonusable = GetComponent<BonusComponent>();
                return Skills[skill] + bonusable.SkillBonuses[skill].Sum();
            }
            else
            {
                return Skills[skill];
            }
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
                var drawable = GetComponent<CharacterRenderComponent>();
                if (damage >= 0)
                {
                    drawable.StartAnimation(AnimationType.Hit, inv_frames);
                    Vulnerable = false;
                }
                CurrentHitPoints -= damage;
                drawable._onAnimationEnd += Drawable__onAnimationEnd;
                if (CurrentHitPoints <= 0)
                {
                    onDestroy();
                }
            }
        }

        private void Drawable__onAnimationEnd(object sender, AnimationEventArgs e)
        {
            var drawable = (CharacterRenderComponent)sender;
            if (e.animation == AnimationType.Hit)
            {
                drawable._onAnimationEnd -= Drawable__onAnimationEnd;
                Vulnerable = true;
            }
        }

        // Fit all per-frame instructions in here for now
        public override void Tick(float dt)
        {
            RegenerateMana(dt);
            base.Tick(dt);
        }

        public override object AsJson()
        {
            return new {
                Id,
                type = GetType().AssemblyQualifiedName,
                Position = PositionJson.ToJson(this),
                Inventory = InventoryJson.ToJson(this),
                EquipSlots = EquipSlotsJson.ToJson(this.EquipSlots)
            };
        }

        public static GameObject FromJson(Deserializer deserializer)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(deserializer.getData());
            var player = new Player();
            var pos = (PositionComponent)player;
            pos.SetLocalCoords(coords);
            foreach (Item item in InventoryJson.FromJson(deserializer.getData(), deserializer))
            {
                player.Inventory.AddItem(item);
            }
            player.EquipSlots = EquipSlotsJson.FromJson(deserializer.getData()["EquipSlots"], deserializer);
            // TODO: refactor this
            {
                GameService.Instance.MainScene.Player = player;
                foreach (var slot in player.EquipSlots.GetSlots().Where(x => x.Item != null))
                {
                    slot.OnItemAdd(slot.Item);
                }
            }
            return player;
        }

        #region Actions
        public void Fire(Position? x = null)
        {
            var pos = GetComponent<PositionComponent>();

            /*
            var coords = pos.WorldPosition;
            coords.Coords += new Vector2(coords.face_direction == HorizontalDirection.Left ? -1 : 1, 0);
            Spells.FireBolt.Cast(this, x ?? coords);
            return;
            */

            if (x != null)
            {
                var direction = x.Value.Coords - pos.WorldPosition.Coords;
                direction.Normalize();
                direction *= 20;
                var boulder = new Boulder((x ?? pos.WorldPosition).Coords);
                Game.AddToMainScene(boulder);
                ((DynamicPhysicsComponent)boulder).ApplyImpulse(direction);
            }
            // Spells.LifeDrain.Cast(this);
        }

        public void PerformItemAction(Item item, bool is_down)
        {

        }

        public void PerformItemAction(WieldedItem item, bool is_down)
        {
            if (!is_down)
            {
                if (!Blocking && TryCooldown("Melee", (int)(30 * MeleeAttackRate)))
                {
                    EquipLocked = true;
                    var drawable = GetComponent<CharacterRenderComponent>();
                    drawable._onAnimationHit += onAttackend;
                    drawable.StartAnimation(AnimationType.Attack, 10);
                    // drawable.StartAnimation(Animation.Attack, 10);
                }
            }
        }

        public void PerformItemAction(Shield item, bool is_down)
        {
            if (is_down)
                StartBlocking();
            else
                StopBlocking();
        }
        public void MeleeHit(WieldedItem weapon)
        {
            GameObject obj = GetMeleeTarget(range: 60);
            var damager = (HitComponent)weapon;
            if (obj != null)
                damager?.Hit(obj);
        }

        public GameObject GetMeleeTarget(float range)
        {
            var pos = GetComponent<PositionComponent>();
            return pos.GetClosestObject(new Vector2(range * (int)pos.WorldPosition.face_direction, 0), x => x.Hittable && x.GameObject.Team != Team.Friend);
        }

        private void onAttackend(object sender, AnimationEventArgs e)
        {
            var drawable = GetComponent<CharacterRenderComponent>();
            drawable._onAnimationHit -= onAttackend;
            if (e.animation == AnimationType.Attack)
            {
                MeleeHit(WieldedItem);
                EquipLocked = false;
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

        public void RegenerateMana(float dt)
        {
            foreach (ManaType type in Enum.GetValues(typeof(ManaType)))
            {
                var bonusable = GetComponent<BonusComponent>();
                CurrentMana[type] += (mana_regen_rate + bonusable.ManaRegenBonuses[type].Sum()) * dt;
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
                Inventory.AddItem(x);
                Game.RemoveFromMainScene(item);
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
        #endregion
    }
}