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
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

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
            shaman.InitComponents();
            var pos = shaman.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            return shaman;
        }

        public void InitComponents()
        {
            var halfsize = new Vector2(15, 20);
            Components.Add(new GoblinBehaviorComponent(this));
            Components.Add(new CastAttackComponent(this));
            Components.Add(new CharMoveComponent(this, Vector2.Zero, halfsize, movespeed: 1.4f));
            Components.Add(new CharacterRenderComponent(this, Color.Orange, "Textures/character"));
            Components.Add(new DamageHitComponent(this, damage: 3, knockback: new Vector2(3, 2)));
            Components.Add(new HitPointComponent(this, 12));
            Compile();
        }

        public override void Compile()
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
            var pos = (PhysicsComponent)drop;
            pos.SetLocalCoords(GetComponent<PositionComponent>().WorldPosition.Coords);
            pos.Pickupable = true;
            CurrentScene.RegisterObject(drop);

            base.onDestroy();
        }
    }
}
