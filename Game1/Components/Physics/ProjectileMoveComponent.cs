using Microsoft.Xna.Framework;
using Omniplatformer.Components.Character;
using Omniplatformer.Enums;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Physics
{
    public class ProjectileMoveComponent : DynamicPhysicsComponent
    {
        public Vector2 Direction { get; set; }

        public override void ProcessCollision(Direction direction, PhysicsComponent obj)
        {
            if (Disabled)
            {
                return;
            }
            base.ProcessCollision(direction, obj);

            if (direction != Enums.Direction.None && obj.GameObject != GameObject.Source && (obj.Solid || obj.Hittable) && obj.GameObject.Team != GameObject.Team)
            {
                var hittable = GetComponent<HitComponent>();
                hittable?.Hit(obj.GameObject);
                // TODO: might have to extract this
                // GameObject.onDestroy();
                CurrentMovement = Vector2.Zero;
                GetComponent<DestructibleComponent>().Destroy();
                Disabled = true;
            }
        }
    }
}
