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
        }

        public override void OnCompile()
        {
            var damageable = GetComponent<HitPointComponent>();
            damageable._onDamage += OnDamage;
            damageable._onBeginDestroy += (sender, e) => onDestroy();
        }

        public void OnDamage(object sender, EventArgs e)
        {
            GetComponent<BehaviorComponent>().Aggressive = true;
        }

        public override void onDestroy()
        {
            // TODO: extract this into a drop component
            Item drop = ChaosOrb.Create();
            this.DropItem(drop);
            base.onDestroy();
        }
    }
}
