using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using Omniplatformer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public class FireBoltProjectile : Projectile
    {
        public FireBoltProjectile(Vector2 center, Vector2 halfsize, GameObject source = null): base(center, halfsize, source)
        {
            Team = source.Team;
            Components.Add(new PositionComponent(this, center, halfsize));
            Components.Add(new GlowingRenderComponent(this));
            Components.Add(new ProjectileMoveComponent(this));
            Components.Add(new DamageHitComponent(this, damage: 1));
        }

        public override object AsJson()
        {
            return new { type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }
    }
}
