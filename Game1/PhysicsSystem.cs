using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public class PhysicsSystem
    {
        public List<GameObject> objects = new List<GameObject>();

        public void Tick(float time_scale)
        {
            for (int j = objects.Count - 1; j >= 0; j--)
            {
                var collisions = new List<(Direction, GameObject)>();
                var obj = objects[j];
                var pos = (PositionComponent)obj;
                Direction collision_direction;
                foreach (var other_obj in objects)
                {
                    // TODO: look into how we access components
                    collision_direction = pos.Collides(other_obj);
                    if (collision_direction != Direction.None)
                        collisions.Add((collision_direction, other_obj));
                }
                var obj_movable = (MoveComponent)obj;
                obj_movable?.ProcessCollisionInteractions(collisions);
                obj_movable?.ProcessMovement(time_scale);
            }
        }

        public void ProcessCollision()
        {

        }
    }
}
