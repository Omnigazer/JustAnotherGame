using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using Omniplatformer.Components.Behavior;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Objects.Items;
using Omniplatformer.Services;
using Omniplatformer.Spells;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;
using Omniplatformer.Utility.Extensions;

namespace Omniplatformer.Objects.Characters
{
    public class GoblinShaman : Character
    {
        public GoblinShaman()
        {
            Team = Team.Enemy;
        }

        public static GoblinShaman Create(Vector2 coords)
        {
            var shaman = new GoblinShaman();
            shaman.InitializeComponents();
            var pos = shaman.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            pos.SetLocalHalfsize(new Vector2(15, 20));
            return shaman;
        }

        public override void InitializeCustomComponents()
        {
            RegisterComponent(new GoblinBehaviorComponent());
            RegisterComponent(new CastAttackComponent(FireBolt.Create().GetComponent<SpellComponent>()));
            RegisterComponent(new SpellCasterComponent());
            RegisterComponent(new CharMoveComponent() { MaxMoveSpeed = 1.4f });
            RegisterComponent(new CharacterRenderComponent(Color.Orange, "Textures/character"));
            RegisterComponent(new DamageHitComponent(damage: 3, knockback: new Vector2(3, 2)));
            RegisterComponent(new HitPointComponent(12));
            RegisterComponent(new DestructibleComponent());
            RegisterComponent(new YieldExperienceComponent() { Value = 250 });
            RegisterComponent(new DropItemComponent());
        }

        public override void OnCompile()
        {
            var damageable = GetComponent<HitPointComponent>();
            damageable.OnDamage.Subscribe((damage) => OnDamage());
        }

        public void OnDamage()
        {
            GetComponent<BehaviorComponent>().Aggressive = true;
        }
    }
}
