﻿using Microsoft.Xna.Framework;
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
        public static int TileSize => 8;
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

        public void Tick(float time_scale)
        {
            // should iterate through dynamic objects only
            for (int i = dynamics.Count - 1; i >= 0; i = Math.Min(i-1, dynamics.Count - 1))
            {
                var obj = dynamics[i];
                // apply external forces
                ApplyGravity(obj);
                ApplyFriction(obj);
                // apply controls
                obj.ProcessMovement();
                // reset flags?
                obj.ResetCollisionFlags();
                ProcessCollisions(obj);

                // Perform the movement
                obj.Move(time_scale);
            }
        }

        protected void ProcessCollisions(DynamicPhysicsComponent obj)
        {
            Direction collision_direction;
            void processCollision(PhysicsComponent other_obj)
            {
                collision_direction = obj.Collides(other_obj);
                if (collision_direction != Direction.None)
                {
                    ApplyCollisionResponse(obj, other_obj, collision_direction);
                    obj.ProcessCollision(collision_direction, other_obj);
                }
            }

            for (int j = objects.Count - 1; j >= 0; j--)
            {
                var other_obj = objects[j];
                if (obj == other_obj)
                    continue;
                processCollision(other_obj);
            }
            foreach (var tile in GetTilesFor(obj))
            {
                processCollision(tile);
            }
        }

        protected void ApplyGravity(DynamicPhysicsComponent movable)
        {
            float gravity = 1f;
            movable.CurrentMovement += new Vector2(0, -gravity);
        }

        protected void ApplyFriction(DynamicPhysicsComponent movable)
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
                    movable.HorizontalSpeed += (speed ?? 0 - movable.HorizontalSpeed) * friction;
                }
            }
            else if (ground.Liquid)
            {
                // var pos = movable.GetComponent<PositionComponent>();
                // TODO: extract this
                float immersion = movable.GetImmersionShare(ground.GameObject);
                movable.CurrentMovement *= 1 - ground.Friction * immersion;
            }
        }

        public IEnumerable<PhysicsComponent> GetTilesFor(PhysicsComponent body)
        {
            if (body.GameObject is Player && body.WorldPosition.Center.X > 430)
            {

            }
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