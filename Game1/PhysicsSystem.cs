using Microsoft.Xna.Framework;
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
        // public List<GameObject> objects = new List<GameObject>();
        public List<PhysicsComponent> objects = new List<PhysicsComponent>();
        public List<DynamicPhysicsComponent> dynamics = new List<DynamicPhysicsComponent>();

        public void Register(PhysicsComponent physicable)
        {
            objects.Add(physicable);
            if (physicable is DynamicPhysicsComponent)
                dynamics.Add((DynamicPhysicsComponent)physicable);
        }

        public void Unregister(PhysicsComponent physicable)
        {
            objects.Remove(physicable);
            if (physicable is DynamicPhysicsComponent)
                dynamics.Remove((DynamicPhysicsComponent)physicable);
        }

        public void Tick(float time_scale)
        {
            // should iterate through dynamic objects only
            for (int i = dynamics.Count - 1; i >= 0; i = Math.Min(i-1, dynamics.Count - 1))
            {
                var obj = dynamics[i];
                // reset flags?
                obj.ResetCollisionFlags();
                // apply external forces
                ApplyGravity(obj);
                // apply controls
                obj.ProcessMovement();
                ProcessCollisions(obj);
                // Perform the movement
                obj.Move(time_scale);
            }
        }

        protected void ProcessCollisions(DynamicPhysicsComponent obj)
        {
            Direction collision_direction;
            for (int j = objects.Count - 1; j >= 0; j--)
            {
                var other_obj = objects[j];
                if (obj == other_obj)
                    continue;
                collision_direction = obj.Collides(other_obj);
                if (collision_direction != Direction.None)
                {
                    ApplyResponse(obj, other_obj, collision_direction);
                    obj.ProcessCollision(collision_direction, other_obj);
                }
            }
        }

        protected void ApplyGravity(DynamicPhysicsComponent movable)
        {
            float gravity = 1f;
            movable.CurrentMovement += new Vector2(0, -gravity);
        }

        protected void ApplyResponse(DynamicPhysicsComponent movable, PhysicsComponent target, Direction dir)
        {
            if (target.Solid)
            {
                switch (dir)
                {
                    case Direction.Up:
                        {
                            movable.VerticalSpeed = Math.Min(0, movable.CurrentMovement.Y);
                            break;
                        }
                    case Direction.Left:
                        {
                            movable.HorizontalSpeed = Math.Max(0, movable.HorizontalSpeed);
                            break;
                        }
                    case Direction.Right:
                        {
                            movable.HorizontalSpeed = Math.Min(0, movable.HorizontalSpeed);
                            break;
                        }
                    case Direction.Down:
                        {
                            movable.VerticalSpeed = Math.Max(0, movable.CurrentMovement.Y);
                            if(movable.move_direction == Direction.None)
                                movable.HorizontalSpeed *= 1 - target.Friction;
                            break;
                        }
                }
                PinTo(movable, target, dir);
            }
            else if (target.Liquid)
            {
                // var pos = movable.GetComponent<PositionComponent>();
                // TODO: extract this
                var pos = target.GetComponent<PositionComponent>();
                float immersion = pos.GetImmersionShare(target.GameObject);
                movable.CurrentMovement *= 1 - target.Friction * immersion;
            }
        }

        // Adjusts the subject's position in case of "penetration"
        protected void PinTo(PhysicsComponent subject, PhysicsComponent target, Direction direction)
        {
            // var subject_pos = (PositionComponent)subject;
            // var their_pos = (PositionComponent)target;

            var subject_pos = subject.GetComponent<PositionComponent>();
            var their_pos = target.GetComponent<PositionComponent>();

            switch (direction)
            {
                case Direction.Right:
                    {
                        var new_x = their_pos.WorldPosition.Center.X - (their_pos.WorldPosition.halfsize.X + subject_pos.WorldPosition.halfsize.X);
                        subject_pos.SetLocalCenter(new Vector2(new_x, subject_pos.WorldPosition.Center.Y));
                        break;
                    }
                case Direction.Left:
                    {
                        var new_x = their_pos.WorldPosition.Center.X + (their_pos.WorldPosition.halfsize.X + subject_pos.WorldPosition.halfsize.X);
                        subject_pos.SetLocalCenter(new Vector2(new_x, subject_pos.WorldPosition.Center.Y));
                        break;
                    }
                case Direction.Up:
                    {
                        var new_y = their_pos.WorldPosition.Center.Y - (their_pos.WorldPosition.halfsize.Y + subject_pos.WorldPosition.halfsize.Y);
                        subject_pos.SetLocalCenter(new Vector2(subject_pos.WorldPosition.Center.X, new_y));
                        break;
                    }
                case Direction.Down:
                    {
                        var new_y = their_pos.WorldPosition.Center.Y + (their_pos.WorldPosition.halfsize.Y + subject_pos.WorldPosition.halfsize.Y);
                        subject_pos.SetLocalCenter(new Vector2(subject_pos.WorldPosition.Center.X, new_y));
                        break;
                    }
            }
        }
    }
}
