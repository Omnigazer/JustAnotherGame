using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Projectiles
{
    public class FireBoltProjectile : Projectile
    {
        IEnumerator behavior;
        public FireBoltProjectile(Vector2 center, Vector2 direction, GameObject source = null): base(source)
        {
            Team = source.Team;

            var proj_movable = new ProjectileMoveComponent(this, center, new Vector2(5, 5)) { InverseMass = 0 };
            direction.Normalize();
            float speed = 20;
            proj_movable.Rotate(-(float)Math.Atan2(direction.Y, direction.X));
            proj_movable.ApplyImpulse(speed * direction, true);

            Components.Add(proj_movable);
            Components.Add(new GlowingRenderComponent(this));
            Components.Add(new DamageHitComponent(this, damage: 4));
            behavior = behaviorGen();
        }

        public override object AsJson()
        {
            return new { type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }

        public override void Tick(float dt)
        {
            current_dt = dt;
            behavior.MoveNext();
            base.Tick(dt);
        }

        public void GenerateSpark()
        {
            var movable = GetComponent<ProjectileMoveComponent>();
            float x = RandomGen.NextFloat(-1.5f, 1.5f), y = RandomGen.NextFloat(-1.5f, 1.5f);
            var spark = new Particle(movable.WorldPosition.Coords);
            var s_movable = (DynamicPhysicsComponent)spark;
            s_movable.ApplyImpulse(new Vector2(x, y));
            CurrentScene.RegisterObject(spark);
        }

        float current_dt;
        IEnumerator behaviorGen()
        {
            var movable = GetComponent<DynamicPhysicsComponent>();
            while (true)
            {
                float wait_time = RandomGen.NextFloat(2, 8);
                for (float t = 0; t < wait_time; t += current_dt)
                {
                    yield return null;
                }
                GenerateSpark();
            }
        }
    }
}
