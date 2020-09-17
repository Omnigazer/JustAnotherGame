using Newtonsoft.Json;
using Omniplatformer.Animations;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Objects.InventoryNS;
using Omniplatformer.Objects.Items;
using Omniplatformer.Objects.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using static Omniplatformer.Enums.Skill;

namespace Omniplatformer.Components.Character
{
    class PlayerActionComponent : Component
    {
        public bool EquipLocked { get; set; }
        public bool Blocking { get; set; }

        // Helpers
        // spans from 0.6 to 0.2 with a weight of 20
        public float MeleeAttackRate => (float)(60 - 40 * ((float)Skills[Melee] / (Skills[Melee] + 20))) / 100f;

        // TODO: possibly cache this
        [JsonIgnore]
        public Dictionary<Skill, int> Skills => GetComponent<SkillComponent>()?.Skills;

        [JsonIgnore]
        public WoodenClub WieldedItem => (WoodenClub)EquipSlots.RightHandSlot.Item;

        [JsonIgnore]
        public EquipSlotCollection EquipSlots => GetComponent<EquipComponent>().EquipSlots;

        public void PerformItemAction(Item item, bool is_down)
        {
        }

        public void PerformItemAction(WoodenClub item, bool is_down)
        {
            if (!is_down)
            {
                TrySwing();
            }
        }

        public void TrySwing()
        {
            var cooldownable = GetComponent<CooldownComponent>();
            if (!Blocking && cooldownable.TryCooldown("Melee", (int)(30 * MeleeAttackRate)))
            {
                EquipLocked = true;
                var drawable = GetComponent<CharacterRenderComponent>();
                drawable.onAnimationState
                        .Where((animation) => ((animation.type == AnimationType.Attack) && (animation.state == AnimationState.AttackHit))).Take(1)
                        .Subscribe((_) =>
                        {
                            var damager = (MeleeDamageHitComponent)WieldedItem;
                            damager.MeleeHit();
                            EquipLocked = false;
                        });
                drawable.StartAnimation(AnimationType.Attack, 10);
            }
        }

        public void PerformItemAction(Shield item, bool is_down)
        {
            if (is_down)
                StartBlocking();
            else
                StopBlocking();
        }

        public void StartBlocking()
        {
            if (EquipSlots.LeftHandSlot.Item == null || EquipLocked)
                return;

            Blocking = true;
            var pos1 = (PositionComponent)EquipSlots.RightHandSlot.Item;
            pos1?.SetParent(GameObject, AnchorPoint.LeftHand);

            var pos2 = (PositionComponent)EquipSlots.LeftHandSlot.Item;
            pos2.SetParent(GameObject, AnchorPoint.RightHand);
        }

        public void StopBlocking()
        {
            if (EquipSlots.LeftHandSlot.Item == null)
                return;

            Blocking = false;
            var pos1 = (PositionComponent)EquipSlots.RightHandSlot.Item;
            pos1?.SetParent(GameObject, AnchorPoint.RightHand);

            var pos2 = (PositionComponent)EquipSlots.LeftHandSlot.Item;
            pos2.SetParent(GameObject, AnchorPoint.LeftHand);
        }

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
                var boulder = Boulder.Create();
                boulder.GetComponent<PositionComponent>().SetLocalCoords((x ?? pos.WorldPosition).Coords);
                Scene.RegisterObject(boulder);
                ((DynamicPhysicsComponent)boulder).ApplyImpulse(direction);
            }
            // Spells.LifeDrain.Cast(this);
        }
    }
}
