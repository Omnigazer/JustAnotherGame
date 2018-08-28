using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public class ProjectileMoveComponent : MoveComponent
    {
        public Vector2 Direction { get; set; }

        public ProjectileMoveComponent(GameObject obj) : base(obj)
        {
        }

        // Check what kinds of objects are we colliding here
        // TODO: problematic method & overrides, refactor
        public override void ProcessCollisionInteractions(List<(Direction, GameObject)> collisions)
        {
            base.ProcessCollisionInteractions(collisions);
        }

        protected override void ProcessCollision(Direction direction, GameObject obj)
        {
            base.ProcessCollision(direction, obj);
            if (direction != Omniplatformer.Direction.None && obj != GameObject.Source && (obj.Solid || obj.Hittable) && obj.Team != GameObject.Team)
            {
                var hittable = GetComponent<HitComponent>();
                hittable?.Hit(obj);
                // TODO: might have to extract this
                GameObject.onDestroy();
                // Hit(obj);
            }
        }

        public override Vector2 GetMoveVector()
        {
            CurrentMovement = Direction;
            return CurrentMovement;
        }
    }
}
