﻿using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using Omniplatformer.Objects;
using Omniplatformer.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Scenes
{
    public class PhysicsSystem : ISubsystem
    {
        /// <summary>
        /// The gravity constant
        /// </summary>
        public static float G => 1.1f;
        public static int TileSize => 16;
        // public List<GameObject> objects = new List<GameObject>();
        public List<PhysicsComponent> objects = new List<PhysicsComponent>();
        public List<DynamicPhysicsComponent> dynamics = new List<DynamicPhysicsComponent>();
        public TileMapPhysicsComponent TileMap { get; set; }

        public int GridWidth { get; set; }
        public int GridHeight { get; set; }

        public PhysicsSystem()
        {
            GridWidth = 5000;
            GridHeight = 5000;
        }

        public void RegisterObject(GameObject obj)
        {
            var physicable = (PhysicsComponent)obj;
            if (physicable != null)
                Register(physicable);

            var tilemap = obj.GetComponent<TileMapPhysicsComponent>();
            if (tilemap != null)
            {
                TileMap = tilemap;
            }
        }

        public void UnregisterObject(GameObject obj)
        {
            var physicable = (PhysicsComponent)obj;
            if (physicable != null)
                Unregister(physicable);
        }

        public void Register(PhysicsComponent physicable)
        {
            // TODO: refactor this
            if (physicable is TileMapPhysicsComponent)
                return;
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

        public (float kx, float ky) GetCollisionTime(DynamicPhysicsComponent obj, PhysicsComponent other)
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
                collision_direction = obj.Collides(other_obj.WorldPosition);

                if (collision_direction == Direction.None)
                    return false;

                /*
                if (obj.GameObject is Player)
                {
                    GameService.Instance.HUDState.status_messages.Add(other_obj.ToString() + " " + GetCollisionTime(obj, other_obj));
                }
                */

                // TODO: remove this workaround
                if (obj.Solid && other_obj.Solid)
                    ApplyCollisionResponse(obj, other_obj, other_obj.WorldPosition, collision_direction);
                return obj.ProcessCollision(collision_direction, other_obj);
            }

            bool processTilemapCollision()
            {
                // foreach(Position pos in TileMap.GetTilesFor(obj))
                foreach (var (i, j) in TileMap.GetTilesFor(obj))
                {
                    var pos = GetTileAtIndices(i, j);
                    if (TileMap.Grid[i, j] > 9)
                        continue;
                    collision_direction = obj.Collides(pos);
                    if (collision_direction != Direction.None)
                    {
                        ApplyCollisionResponse(obj, TileMap, pos, collision_direction);
                        if (obj.ProcessCollision(collision_direction, TileMap))
                            return true;
                    }
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

            return processTilemapCollision();
        }

        // Yeah, and air resistance
        protected void ApplyGravity(DynamicPhysicsComponent movable, float dt)
        {
            if (movable.InverseMass == 0)
                return;
            // constant "air resistance" force times inverse mass
            float a_air = 0.004f * movable.InverseMass * dt * 0;
            // float a_air = 0;
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
                // float friction = 1 - ground.Friction;
                float friction = ground.Friction;
                //if (movable.move_direction == Direction.None)
                //    movable.HorizontalSpeed *= 1 - friction;

                {
                    // TODO: implement "chassis"
                    var speed = (ground as DynamicPhysicsComponent)?.HorizontalSpeed;
                    movable.HorizontalSpeed += ((speed ?? 0) - movable.HorizontalSpeed) * friction * dt * 0.1f;
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

        protected void ApplyCollisionResponse(DynamicPhysicsComponent movable, PhysicsComponent target, Position target_pos, Direction dir)
        {
            // if (target.Solid)
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
                PinTo(movable, target_pos, dir);
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
        protected void PinTo(PhysicsComponent subject, Position target_position, Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    {
                        var new_x = target_position.Center.X - (target_position.halfsize.X + subject.WorldPosition.halfsize.X);
                        subject.SetWorldCenter(new Vector2(new_x, subject.WorldPosition.Center.Y));
                        break;
                    }
                case Direction.Left:
                    {
                        var new_x = target_position.Center.X + (target_position.halfsize.X + subject.WorldPosition.halfsize.X);
                        subject.SetWorldCenter(new Vector2(new_x, subject.WorldPosition.Center.Y));
                        break;
                    }
                case Direction.Up:
                    {
                        var new_y = target_position.Center.Y - (target_position.halfsize.Y + subject.WorldPosition.halfsize.Y);
                        subject.SetWorldCenter(new Vector2(subject.WorldPosition.Center.X, new_y));
                        break;
                    }
                case Direction.Down:
                    {
                        var new_y = target_position.Center.Y + (target_position.halfsize.Y + subject.WorldPosition.halfsize.Y);
                        subject.SetWorldCenter(new Vector2(subject.WorldPosition.Center.X, new_y));
                        break;
                    }
            }
        }

        public Position GetTileAtIndices(int i, int j)
        {
            return new Position(new Vector2(i * TileSize, j * TileSize), new Vector2(TileSize / 2));
        }

        public (int, int) GetTileIndices(Vector2 coords)
        {
            return ((int)coords.X / TileSize, (int)coords.Y / TileSize);
        }

        public short GetTileAtCoords(Vector2 coords)
        {
            var (i, j) = GetTileIndices(coords);
            return TileMap.Grid[i, j];
        }

        public void RemoveTileAtCoords(Vector2 coords)
        {
            var (i, j) = GetTileIndices(coords);
            TileMap.Grid[i, j] = 0;
        }

        public GameObject GetObjectAtCoords(Vector2 coords)
        {
            /*
            foreach (var platform in objects)
            {
                // if (!platform.Draggable)
                // {
                //     continue;
                // }
                var platform_pos = (PositionComponent)platform;
                if (platform_pos.Contains(coords))
                {
                    return platform;
                }
            }
            */

            foreach (var component in objects)
            {
                if (component.Contains(coords))
                {
                    return component.GameObject;
                }
            }
            return null;
        }

        public IEnumerable<GameObject> GetOverlappingObjects(PositionComponent physicable)
        {
            foreach (var component in objects)
            {
                if (physicable.Overlaps(component.WorldPosition))
                    yield return component.GameObject;
            }
        }

        public IEnumerable<GameObject> GetObjectsAroundPosition(Position position, int radius)
        {
            foreach (var component in objects)
            {
                var vector = component.WorldPosition.Center - position.Center;
                if (vector.Length() < radius)
                {
                    yield return component.GameObject;
                }
            }
        }
    }
}
