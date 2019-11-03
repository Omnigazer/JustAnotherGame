using Microsoft.Xna.Framework;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;

namespace Omniplatformer.Objects.Projectiles
{
    class Particle : Projectile
    {
        public Particle(Vector2 coords): base()
        {
            TTL = 15;
            var movable = new DynamicPhysicsComponent(this, coords, new Vector2(1)) { Solid = false, Hittable = false, InverseMass = 10 };
            Components.Add(movable);
            Components.Add(new GlowingRenderComponent(this) { Radius = 20 });
        }
    }
}
