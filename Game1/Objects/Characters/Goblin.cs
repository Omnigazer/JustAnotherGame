using System;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using Omniplatformer.Components.Behavior;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Objects.Projectiles;
using Omniplatformer.Services;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Characters
{
    public class Goblin : Character
    {
        public Goblin(Vector2 coords)
        {
            Team = Team.Enemy;
            var halfsize = new Vector2(20, 26);
            // Components.Add(new PositionComponent(this, coords, halfsize));
            Components.Add(new GoblinBehaviorComponent(this));
            Components.Add(new ThrowAttackComponent(this));
            Components.Add(new CharMoveComponent(this, coords, halfsize, movespeed: 1.8f));
            Components.Add(new CharacterRenderComponent(this, Color.Green, GameContent.Instance.character));
            Components.Add(new DamageHitComponent(this, damage: 2, knockback: new Vector2(3, 2)));

            var damageable = new HitPointComponent(this, 8);
            damageable._onDamage += OnDamage;
            damageable._onBeginDestroy += (sender, e) => onDestroy();
            Components.Add(damageable);
        }

        public void OnDamage(object sender, EventArgs e)
        {
            Aggravate();
        }

        // Aggravate everyone in the 1000 radius
        public void Aggravate()
        {
            var pos = GetComponent<PositionComponent>();
            foreach (var obj in CurrentScene.PhysicsSystem.GetObjectsAroundPosition(pos.WorldPosition, 1000)
                                                          .Where(x => x.Team == Team.Enemy))
            {
                var behavior = obj.GetComponent<BehaviorComponent>();
                if (behavior != null)
                {
                    behavior.Aggressive = true;
                }
            }
        }

        public static GameObject FromJson(Deserializer deserializer)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(deserializer.getData());
            var goblin = new Goblin(coords);
            return goblin;
        }
    }
}
