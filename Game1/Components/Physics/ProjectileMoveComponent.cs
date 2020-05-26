using Microsoft.Xna.Framework;
using Omniplatformer.Enums;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Physics
{
    public class ProjectileMoveComponent : DynamicPhysicsComponent
    {
        public Vector2 Direction { get; set; }

        bool done = false;

        public override bool ProcessCollision(Direction direction, PhysicsComponent obj)
        {
            if (done)
            {
                return false;
            }
            base.ProcessCollision(direction, obj);

            if (direction != Enums.Direction.None && obj.GameObject != GameObject.Source && (obj.Solid || obj.Hittable) && obj.GameObject.Team != GameObject.Team)
            {
                var hittable = GetComponent<HitComponent>();
                hittable?.Hit(obj.GameObject);
                // TODO: might have to extract this
                // GameObject.onDestroy();
                CurrentMovement = Vector2.Zero;
                GameObject.onDestroy();
                done = true;
                return true;
                // Hit(obj);
            }
            return false;
        }

        public override void ProcessMovement(float dt)
        {
            // CurrentMovement = Direction;
        }
    }
}
