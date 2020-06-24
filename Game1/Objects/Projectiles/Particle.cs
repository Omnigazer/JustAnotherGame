using Microsoft.Xna.Framework;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;

namespace Omniplatformer.Objects.Projectiles
{
    class Particle : Projectile
    {
        public override void InitializeCustomComponents()
        {
            TTL = 15;
            var movable = new DynamicPhysicsComponent() { Solid = false, Hittable = false, InverseMass = 10 };
            RegisterComponent(movable);
            RegisterComponent(new GlowingRenderComponent() { Radius = 20 });
            RegisterComponent(new DestructibleComponent());
        }

        public static Particle Create(Vector2 coords)
        {
            var particle = new Particle();
            particle.InitializeComponents();
            var pos = particle.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            pos.SetLocalHalfsize(new Vector2(1, 1));
            return particle;
        }
    }
}
