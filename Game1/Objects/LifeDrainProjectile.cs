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
    public class LifeDrainProjectile : Projectile
    {
        public LifeDrainProjectile(Vector2 center, Vector2 halfsize, GameObject source = null): base(center, halfsize, source)
        {
            Team = source?.Team ?? Team.Friend;
            // Components.Add(new DynamicPhysicsComponent(this, center, halfsize));
            var c = new GlowingRenderComponent(this) { GlowColor = Color.Purple };
            Components.Add(c);
            // Components.Add(new GlowingRenderComponent(this));
            Components.Add(new ProjectileMoveComponent(this, center, halfsize));
            Components.Add(new LifeDrainHitComponent(this, damage: 3));
        }

        public override object AsJson()
        {
            return new { type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }
    }
}
