using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using Omniplatformer.Components.Character;
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
        const int inv_frames = 100;

        public bool EquipLocked { get; set; }
        public bool Blocking { get; set; }

        // public WieldedItem WieldedItem { get => (WieldedItem)EquipSlots.RightHandSlot.Item; private set => WieldItem(value); }
        public WieldedItem WieldedItem => (WieldedItem)EquipSlots.RightHandSlot.Item;

        // Equipment and inventory
        public Inventory.Inventory Inventory { get; set; } = new Inventory.Inventory();
        public EquipSlotCollection EquipSlots { get; set; } = new EquipSlotCollection();

        // spans from 0.6 to 0.2 with a weight of 20
        public float MeleeAttackRate => (float)(60 - 40 * ((float)Skills[Melee] / (Skills[Melee] + 20))) / 100f;

        // Helpers

        // TODO: possibly cache this
        public Dictionary<Skill, int> Skills => GetComponent<SkillComponent>().Skills;

        public Player()
        {
            InitComponents();
            Team = Team.Friend;
        }

        void InitComponents()
        {
            var halfsize = new Vector2(20, 36);
            var phys = new PlayerMoveComponent(this, Vector2.Zero, halfsize) { MaxMoveSpeed = 9, Acceleration = 0.5f };
            phys.AddAnchor(AnchorPoint.RightHand, new Position(new Vector2(0.4f, 0.21f), Vector2.Zero));
            phys.AddAnchor(AnchorPoint.LeftHand, new Position(new Vector2(-0.45f, -0.05f), Vector2.Zero, 0.6f));
            Components.Add(phys);
            Components.Add(new CharacterRenderComponent(this, Color.Gray, GameContent.Instance.character));
            Components.Add(new BonusComponent(this));
            Components.Add(new SkillComponent(this));
            Components.Add(new ManaComponent(this));
            Components.Add(new ExperienceComponent(this));

            var damageable = new HitPointComponent(this, max_hitpoints);
            damageable._onDamage += OnDamage;
            damageable._onBeginDestroy += (sender, e) => onDestroy();
            Components.Add(damageable);
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

        public void OnDamage(object sender, DamageEventArgs e)
        {
            var hit_points = GetComponent<HitPointComponent>();
            var drawable = GetComponent<CharacterRenderComponent>();
            if (e.Damage >= 0)
            {
                drawable.StartAnimation(AnimationType.Hit, inv_frames);
                hit_points.Vulnerable = false;
            }
            drawable._onAnimationEnd += Drawable__onAnimationEnd;
        }

        private void Drawable__onAnimationEnd(object sender, AnimationEventArgs e)
        {
            var hit_points = GetComponent<HitPointComponent>();
            var drawable = (CharacterRenderComponent)sender;
            if (e.animation == AnimationType.Hit)
            {
                drawable._onAnimationEnd -= Drawable__onAnimationEnd;
                hit_points.Vulnerable = true;
            }
        }

        // Fit all per-frame instructions in here for now
        public override void Tick(float dt)
        {
            var manable = GetComponent<ManaComponent>();
            manable.RegenerateMana(dt);
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
                CurrentScene.RegisterObject(boulder);
                ((DynamicPhysicsComponent)boulder).ApplyImpulse(direction);
            }
            // Spells.LifeDrain.Cast(this);
        }

        public void PerformItemAction(Item item, bool is_down)
        {

        }

        public void PerformItemAction(WieldedItem item, bool is_down)
        {
            var cooldownable = GetComponent<CooldownComponent>();
            if (!is_down)
            {
                if (!Blocking && cooldownable.TryCooldown("Melee", (int)(30 * MeleeAttackRate)))
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

        private void onAttackend(object sender, AnimationEventArgs e)
        {
            var drawable = GetComponent<CharacterRenderComponent>();
            drawable._onAnimationHit -= onAttackend;
            if (e.animation == AnimationType.Attack)
            {
                var damager = (MeleeDamageHitComponent)WieldedItem;
                damager.MeleeHit();
                EquipLocked = false;
            }
        }

        #endregion

        #region Gameplay logic

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
                CurrentScene.UnregisterObject(item);
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