using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;

namespace Omniplatformer.Objects
{
    class Particle : Projectile
    {
        public Particle(Vector2 coords, Vector2 direction): base()
        {
            TTL = 15;
            var movable = new DynamicPhysicsComponent(this, coords, new Vector2(1)) { Solid = false, Hittable = false, InverseMass = 10 };
            movable.CurrentMovement = direction;
            Components.Add(movable);
            Components.Add(new GlowingRenderComponent(this) { Radius = 20 });
        }
    }
}
