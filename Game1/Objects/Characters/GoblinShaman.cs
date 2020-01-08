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
        public GoblinShaman(Vector2 coords)
        {
            Team = Team.Enemy;
            var halfsize = new Vector2(15, 20);
            Components.Add(new GoblinBehaviorComponent(this));
            Components.Add(new CastAttackComponent(this));
            Components.Add(new CharMoveComponent(this, coords, halfsize, movespeed: 1.4f));
            Components.Add(new CharacterRenderComponent(this, Color.Orange, GameContent.Instance.character));
            Components.Add(new DamageHitComponent(this, damage: 3, knockback: new Vector2(3, 2)));

            var damageable = new HitPointComponent(this, 12);
            damageable._onDamage += OnDamage;
            damageable._onBeginDestroy += (sender, e) => onDestroy();
            Components.Add(damageable);
        }

        public void OnDamage(object sender, EventArgs e)
        {
            GetComponent<BehaviorComponent>().Aggressive = true;
        }

        public override void onDestroy()
        {
            // TODO: extract this into a drop component
            Item drop = new ChaosOrb();
            var pos = (PhysicsComponent)drop;
            pos.SetLocalCoords(GetComponent<PositionComponent>().WorldPosition.Coords);
            pos.Pickupable = true;
            CurrentScene.RegisterObject(drop);

            base.onDestroy();
        }

        public static GameObject FromJson(Deserializer deserializer)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(deserializer.getData());
            var shaman = new GoblinShaman(coords);
            // SerializeService.Instance.RegisterObject(zombie);
            return shaman;
        }
    }
}
