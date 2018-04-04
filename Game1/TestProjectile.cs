using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public class TestProjectile : Projectile
    {    
        public TestProjectile(Vector2 center, Vector2 halfsize): base(center, halfsize)
        {
            Team = Team.Friend;
            Components.Add(new PositionComponent(this, center, halfsize));
            Components.Add(new GlowingRenderComponent(this));
            Components.Add(new ProjectileMoveComponent(this));
            Components.Add(new DamageHitComponent(this, damage: 1));
        }                
    }
}
