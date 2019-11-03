using Microsoft.Xna.Framework;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Physics
{
    public class PhysicsComponent : PositionComponent
    {
        public float InverseMass { get; set; } = 1;
        public float Friction { get; set; }
        public bool Tile { get; set; }
        public bool Solid { get; set; }
        public bool Liquid { get; set; }
        public virtual bool Climbable { get; set; }
        public virtual bool Pickupable { get; set; }
        public virtual bool Hittable { get; set; }

        public PhysicsComponent(GameObject obj, Vector2 coords, Vector2 halfsize): base(obj, coords, halfsize)
        {

        }

        public PhysicsComponent(GameObject obj, Vector2 coords, Vector2 halfsize, Vector2 origin) : base(obj, coords, halfsize, 0, origin)
        {

        }
    }
}
