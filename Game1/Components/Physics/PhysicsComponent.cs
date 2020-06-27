using Microsoft.Xna.Framework;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Physics
{
    public class PhysicsComponent : PositionComponent
    {
        public bool Disabled { get; set; }
        public float InverseMass { get; set; } = 1;
        public float Friction { get; set; }
        public bool Solid { get; set; }
        public bool Liquid { get; set; }
        public virtual bool Climbable { get; set; }
        public virtual bool Pickupable { get; set; }
        public bool Hittable { get; set; }

        public PhysicsComponent() { }

        public PhysicsComponent(Vector2 coords, Vector2 halfsize) : base(coords, halfsize)
        {
        }

        public PhysicsComponent(Vector2 coords, Vector2 halfsize, Vector2 origin) : base(coords, halfsize, 0, origin)
        {
        }
    }
}
