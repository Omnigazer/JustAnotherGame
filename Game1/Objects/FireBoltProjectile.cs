using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using Omniplatformer.Objects;
using Omniplatformer.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public class FireBoltProjectile : Projectile
    {
        IEnumerator behavior;
        public FireBoltProjectile(Vector2 center, Vector2 halfsize, GameObject source = null): base(center, halfsize, source)
        {
            Team = source.Team;
            Components.Add(new ProjectileMoveComponent(this, center, halfsize));
            Components.Add(new GlowingRenderComponent(this));
            Components.Add(new DamageHitComponent(this, damage: 4));
            behavior = behaviorGen();
        }

        public override object AsJson()
        {
            return new { type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }

        public override void Tick(float time_scale)
        {
            current_time_scale = time_scale;
            behavior.MoveNext();
            base.Tick(time_scale);
        }

        public void GenerateSpark()
        {
            var movable = GetComponent<ProjectileMoveComponent>();
            int x = RandomGen.Next(-15, 15), y = RandomGen.Next(-15, 15);
            var spark = new Particle(movable.WorldPosition.Coords, new Vector2(1), new Vector2(x, y));
            Game.AddToMainScene(spark);
        }

        float current_time_scale;
        IEnumerator behaviorGen()
        {
            var movable = GetComponent<DynamicPhysicsComponent>();
            while (true)
            {
                float wait_time = RandomGen.NextFloat(2, 8);
                for (float t = 0; t < wait_time; t += current_time_scale)
                {
                    yield return null;
                }
                GenerateSpark();
            }
        }
    }
}
