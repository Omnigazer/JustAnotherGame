using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
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

namespace Omniplatformer.Objects.Characters
{
    public class Player : Character
    {
        // Character constants
        const float max_hitpoints = 50;

        const int inv_frames = 100;

        [JsonIgnore]
        public Inventory.Inventory Inventory => GetComponent<InventoryComponent>().Inventory;

        public Player()
        {
            Team = Team.Friend;
            // TODO: refactor this
            GameService.Instance.MainScene.Player = this;
            RegisterComponent(new InvFramesComponent() { InvFrames = inv_frames });
            RegisterComponent(new DestructibleComponent());
        }

        public override void InitializeCustomComponents()
        {
            var phys = new PlayerMoveComponent() { MaxMoveSpeed = 9, Acceleration = 0.5f };
            phys.AddAnchor(AnchorPoint.RightHand, new Position(new Vector2(0.4f, 0.21f), Vector2.Zero));
            phys.AddAnchor(AnchorPoint.LeftHand, new Position(new Vector2(-0.45f, -0.05f), Vector2.Zero, 0.6f));
            RegisterComponent(phys);
            RegisterComponent(new CharacterRenderComponent(Color.Gray, "Textures/character"));
            RegisterComponent(new PlayerActionComponent());
            RegisterComponent(new BonusComponent());
            RegisterComponent(new SkillComponent());
            RegisterComponent(new ManaComponent());
            RegisterComponent(new ExperienceComponent());
            RegisterComponent(new PlayerInventoryComponent() { Inventory = Objects.Inventory.Inventory.Create() });
            RegisterComponent(new EquipComponent() { EquipSlots = EquipSlotCollection.Create() });
            var damageable = new HitPointComponent(max_hitpoints) { InvFrames = inv_frames };
            RegisterComponent(damageable);
            RegisterComponent(new DestructibleComponent());
        }

        public static Player Create()
        {
            var player = new Player();
            player.InitializeComponents();
            var pos = (PositionComponent)player;
            pos.SetLocalHalfsize(new Vector2(20, 36));
            return player;
        }
    }
}
