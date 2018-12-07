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
        /// <summary>
        /// The gravity constant
        /// </summary>
        public static float G => 1.1f;
        public static int TileSize => 16;
        // public List<GameObject> objects = new List<GameObject>();
        public List<PhysicsComponent> objects = new List<PhysicsComponent>();
        public List<DynamicPhysicsComponent> dynamics = new List<DynamicPhysicsComponent>();
        public PhysicsComponent[,] tiles = new PhysicsComponent[5000, 5000];

        public void Register(PhysicsComponent physicable)
        {
            if (physicable.Tile)
            {
                int i = (int)physicable.WorldPosition.Coords.X / TileSize;
                int j = (int)physicable.WorldPosition.Coords.Y / TileSize;
                tiles[i + 2500, j + 2500] = physicable;
            }
            else
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

        public void Tick(float dt)
        {
            // divide dt by this number
            int N = 4;

            for (int i = dynamics.Count - 1; i >= 0; i = Math.Min(i-1, dynamics.Count - 1))
            {
                var obj = dynamics[i];
                for (int j = 0; j < N; j++)
                {
                    float local_dt = dt / N;
                    // apply external forces
                    ApplyGravity(obj, local_dt);
                    ApplyFriction(obj, local_dt);
                    // apply controls
                    obj.ProcessMovement(local_dt);
                    // reset flags?
                    obj.ResetCollisionFlags();
                    if (ProcessCollisions(obj))
                        break;

                    // Perform the movement
                    obj.Move(local_dt);
                }
            }
        }

        public (float kx, float ky) GetCollisionTime(DynamicPhysicsComponent obj, PhysicsComponent other, float dt)
        {
            float px = Math.Abs(obj.WorldPosition.Center.X - other.WorldPosition.Center.X) - (obj.WorldPosition.halfsize.X + other.WorldPosition.halfsize.X);
            float py = Math.Abs(obj.WorldPosition.Center.Y - other.WorldPosition.Center.Y) - (obj.WorldPosition.halfsize.Y + other.WorldPosition.halfsize.Y);

            return (px, py);
        }

        List<(float, PhysicsComponent)> collisionDistanceMap = new List<(float, PhysicsComponent)>();

        protected bool ProcessCollisions(DynamicPhysicsComponent obj)
        {
            Direction collision_direction;
            bool processCollision(PhysicsComponent other_obj)
            {
                collision_direction = obj.Collides(other_obj);
                if (collision_direction != Direction.None)
                {
                    if (obj.GameObject is Player)
                    {
                        GameService.Instance.HUDState.status_messages.Add(other_obj.ToString() + " " + GetCollisionTime(obj, other_obj));
                    }
                    ApplyCollisionResponse(obj, other_obj, collision_direction);
                    return obj.ProcessCollision(collision_direction, other_obj);
                }
                return false;
            }

            collisionDistanceMap.Clear();

            for (int j = objects.Count - 1; j >= 0; j--)
            {
                var other_obj = objects[j];
                if (obj == other_obj)
                    continue;
                var (px, py) = GetCollisionTime(obj, other_obj);
                collisionDistanceMap.Add((px + py, other_obj));
                // processCollision(other_obj);
            }

            collisionDistanceMap.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            foreach (var (distance, other) in collisionDistanceMap)
            {
                if (processCollision(other))
                    return true;
            }

            foreach (var tile in GetTilesFor(obj))
            {
                if (processCollision(tile))
                    return true;
            }
            return false;
        }

        // Yeah, and air resistance
        protected void ApplyGravity(DynamicPhysicsComponent movable, float dt)
        {
            if (movable.InverseMass == 0)
                return;
            // constant "air resistance" force times inverse mass
            float a_air = 0.004f * movable.InverseMass * dt * 0;
            /*
            float a = -(gravity - a_air);
            int direction_sign = Math.Sign(movable.HorizontalSpeed);
            movable.CurrentMovement += new Vector2(-a_air * direction_sign, a);
            */
            movable.CurrentMovement += new Vector2(0, -G * dt);
            movable.CurrentMovement -= movable.CurrentMovement * a_air;
        }

        protected void ApplyFriction(DynamicPhysicsComponent movable, float dt)
        {
            // get current "platform"
            var ground = movable.CurrentGround;
            if (ground == null)
                return;

            if (ground.Solid) {
                float friction = 1 - ground.Friction;
                //if (movable.move_direction == Direction.None)
                //    movable.HorizontalSpeed *= 1 - friction;

                {
                    var speed = (ground as DynamicPhysicsComponent)?.HorizontalSpeed;
                    //if (speed >= 0 ? movable.HorizontalSpeed < speed : movable.HorizontalSpeed > speed)
                    movable.HorizontalSpeed += (speed ?? 0 - movable.HorizontalSpeed) * 0.1f * friction * dt;
                }
            }
            else if (ground.Liquid)
            {
                // var pos = movable.GetComponent<PositionComponent>();
                // TODO: extract this
                float immersion = movable.GetImmersionShare(ground);
                movable.CurrentMovement -= ground.Friction * immersion * movable.CurrentMovement * dt;
            }
        }

        public IEnumerable<PhysicsComponent> GetTilesFor(PhysicsComponent body)
        {
            var rect = body.GetRectangle();

            int left_index = rect.Left / TileSize - 1 + 2500;
            int width = rect.Width / TileSize + 2;
            int top_index = rect.Top / TileSize - 1 + 2500;
            int height = rect.Height / TileSize + 2;
            for (int i = left_index; i <= left_index + width; i++)
                for(int j = top_index; j <= top_index + height; j++)
                {
                    if(tiles[i,j] != null)
                      yield return tiles[i,j];
                }
        }

        protected void ApplyCollisionResponse(DynamicPhysicsComponent movable, PhysicsComponent target, Direction dir)
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
                            movable.CurrentGround = target;
                            /*
                            float friction = 1 - target.Friction;
                            if (movable.move_direction != Direction.None)
                                friction -= 1f;
                            //if (movable.move_direction == Direction.None)
                            //    movable.HorizontalSpeed *= 1 - friction;
                            if (target is DynamicPhysicsComponent)
                            {
                                var speed = (target as DynamicPhysicsComponent).HorizontalSpeed;
                                //if (speed >= 0 ? movable.HorizontalSpeed < speed : movable.HorizontalSpeed > speed)
                                movable.HorizontalSpeed += (speed - movable.HorizontalSpeed) * friction;
                            }
                            */
                            break;
                        }
                }
                PinTo(movable, target, dir);
            }
            /*
            else if (target.Liquid)
            {
                // var pos = movable.GetComponent<PositionComponent>();
                // TODO: extract this
                float immersion = movable.GetImmersionShare(target.GameObject);
                movable.CurrentMovement *= 1 - target.Friction * immersion;
            }
            */
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
