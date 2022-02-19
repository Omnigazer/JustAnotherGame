using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Omniplatformer.Components.Physics;
using Omniplatformer.Enums;
using Omniplatformer.Objects;

namespace Omniplatformer.Scenes.Subsystems
{
    public class PhysicsSystem : ISubsystem
    {
        /// <summary>
        /// The gravity constant
        /// </summary>
        public static float G => 1.1f;

        /// <summary>
        /// The volume of the character
        /// </summary>
        public static float CharV => 3f;

        public static int TileSize => 16;

        // public List<GameObject> objects = new List<GameObject>();
        public List<PhysicsComponent> objects = new List<PhysicsComponent>();

        public List<DynamicPhysicsComponent> dynamics = new List<DynamicPhysicsComponent>();
        public TileMapPhysicsComponent TileMap { get; set; }

        public int GridWidth { get; set; }
        public int GridHeight { get; set; }

        // TODO: using max resolution, refactor this
        private int chunk_width => 2560 / TileSize;

        private int chunk_height => 1440 / TileSize;
        public List<PhysicsComponent>[,] Chunks;

        public PhysicsSystem()
        {
            GridWidth = 5000;
            GridHeight = 5000;
            Chunks = new List<PhysicsComponent>[GridWidth / chunk_width, GridHeight / chunk_height];
            for (int i = 0; i < Chunks.GetLength(0); i++)
                for (int j = 0; j < Chunks.GetLength(1); j++)
                    Chunks[i, j] = new List<PhysicsComponent>();
        }

        public IEnumerable<PhysicsComponent> GetEligiblesForCollisions(PhysicsComponent phys)
        {
            var (x, y) = GetChunk(phys);

            for (int i = x - 1; i <= x + 1; i++)
                for (int j = y - 1; j <= y + 1; j++)
                    foreach (var obj in Chunks[i, j])
                        if (obj != phys)
                            yield return obj;
        }

        public void ProcessChunks()
        {
            for (int i = 0; i < Chunks.GetLength(0); i++)
                for (int j = 0; j < Chunks.GetLength(1); j++)
                    Chunks[i, j].Clear();

            foreach (var obj in objects)
            {
                var (x, y) = GetChunk(obj);
                Chunks[x, y].Add(obj);
            }
        }

        public (int, int) GetChunk(PhysicsComponent phys)
        {
            var pos = phys.WorldPosition.Center;
            return ((int)(pos.X / 2560), (int)(pos.Y / 1440));
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
            ProcessChunks();
            // divide dt by this number
            int N = 4;

            for (int j = 0; j < N; j++)
            {
                // for (int i = dynamics.Count - 1; i >= 0; i = Math.Min(i - 1, dynamics.Count - 1))
                foreach (var obj in dynamics)
                {
                    // var obj = dynamics[i];
                    float local_dt = dt / N;
                    // apply external forces
                    ApplyGravity(obj, local_dt);
                    // apply controls
                    obj.ProcessMovement(local_dt);
                    ApplyFriction(obj, local_dt);
                    // reset flags?
                    obj.ResetCollisionFlags();
                    ProcessCollisions(obj);

                    // Perform the movement
                    obj.Move(local_dt);
                }
            }
        }

        protected void ProcessCollisions(DynamicPhysicsComponent obj)
        {
            Direction collision_direction;
            void processCollision(PhysicsComponent other_obj)
            {
                collision_direction = obj.Collides(other_obj.WorldPosition);

                if (collision_direction == Direction.None)
                    return;

                // TODO: remove this workaround
                if (obj.ShouldImpactObject(other_obj, collision_direction))
                    ApplyCollisionResponse(obj, other_obj, other_obj.WorldPosition, collision_direction);
                obj.ProcessCollision(collision_direction, other_obj);
            }

            void processTilemapCollision()
            {
                // foreach(Position pos in TileMap.GetTilesFor(obj))
                foreach (var (i, j) in TileMap.GetTilesFor(obj))
                {
                    var pos = GetTileAtIndices(i, j);
                    collision_direction = obj.Collides(pos);
                    if (collision_direction != Direction.None)
                    {
                        ApplyCollisionResponse(obj, TileMap, pos, collision_direction);
                        obj.ProcessCollision(collision_direction, TileMap);
                    }
                }
            }

            foreach (var other in GetEligiblesForCollisions(obj))
            {
                processCollision(other);
            }
            processTilemapCollision();
        }

        protected void ApplyGravity(DynamicPhysicsComponent movable, float dt)
        {
            if (movable.InverseMass == 0)
                return;
            movable.ApplyImpulse(new Vector2(0, -G * dt), true);
        }

        protected void ApplyFriction(DynamicPhysicsComponent movable, float dt)
        {
            // get current "platform"
            var ground = movable.CurrentGround;

            // air friction
            if (ground == null)
            {
                var chassis_speed = (movable.GetComponent<PlayerMoveComponent>())?.ChassisSpeed ?? 0;
                float air_control = 150f;
                float v_air = 0.004f * movable.InverseMass;
                float h_air = 0.004f * movable.InverseMass;
                if (movable.MoveDirection != Direction.None)
                    h_air *= air_control;
                movable.HorizontalSpeed += (chassis_speed - movable.HorizontalSpeed) * h_air * dt;
                movable.VerticalSpeed += (-movable.HorizontalSpeed) * v_air * dt;
            }
            else if (ground.Solid)
            {
                float friction = ground.Friction;
                {
                    var chassis_speed = (movable.GetComponent<PlayerMoveComponent>())?.ChassisSpeed ?? 0;
                    var ground_speed = (ground as DynamicPhysicsComponent)?.HorizontalSpeed;
                    var delta = ((ground_speed ?? 0) - movable.HorizontalSpeed + chassis_speed) * friction * dt;
                    movable.HorizontalSpeed += delta;
                }
            }
        }

        protected void ApplyCollisionResponse(DynamicPhysicsComponent movable, PhysicsComponent target, Position target_pos, Direction dir)
        {
            if (target.Solid)
            {
                switch (dir)
                {
                    case Direction.Up:
                        {
                            movable.VerticalSpeed = Math.Min(0, movable.VerticalSpeed);
                            break;
                        }
                    case Direction.Left:
                        {
                            movable.HorizontalSpeed = Math.Max(0, movable.HorizontalSpeed);
                            if (movable is PlayerMoveComponent)
                                movable.GetComponent<PlayerMoveComponent>().ChassisSpeed = Math.Max(0,
                                    movable.GetComponent<PlayerMoveComponent>().ChassisSpeed);
                            break;
                        }
                    case Direction.Right:
                        {
                            movable.HorizontalSpeed = Math.Min(0, movable.HorizontalSpeed);
                            if (movable is PlayerMoveComponent)
                                movable.GetComponent<PlayerMoveComponent>().ChassisSpeed = Math.Min(0,
                                    movable.GetComponent<PlayerMoveComponent>().ChassisSpeed);
                            break;
                        }
                    case Direction.Down:
                        {
                            movable.VerticalSpeed = Math.Max(0, movable.VerticalSpeed);
                            movable.CurrentGround = target;
                            break;
                        }
                }

                PinTo(movable, target_pos, dir);
            }
            // TODO: reimplement this
            else if (target.Liquid)
            {
                float immersion = movable.GetImmersionShare(target);
                movable.ApplyImpulse(new Vector2(0, G * CharV * immersion));
                movable.ApplyResistance(target.Friction * immersion);
            }
        }

        // Adjusts the subject's position in case of "penetration"
        protected void PinTo(PhysicsComponent subject, Position target_position, Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    {
                        var new_x = target_position.Center.X - (target_position.Halfsize.X + subject.WorldPosition.Halfsize.X);
                        subject.SetWorldCenter(new Vector2(new_x, subject.WorldPosition.Center.Y));
                        break;
                    }
                case Direction.Left:
                    {
                        var new_x = target_position.Center.X + (target_position.Halfsize.X + subject.WorldPosition.Halfsize.X);
                        subject.SetWorldCenter(new Vector2(new_x, subject.WorldPosition.Center.Y));
                        break;
                    }
                case Direction.Up:
                    {
                        var new_y = target_position.Center.Y - (target_position.Halfsize.Y + subject.WorldPosition.Halfsize.Y);
                        subject.SetWorldCenter(new Vector2(subject.WorldPosition.Center.X, new_y));
                        break;
                    }
                case Direction.Down:
                    {
                        var new_y = target_position.Center.Y + (target_position.Halfsize.Y + subject.WorldPosition.Halfsize.Y);
                        subject.SetWorldCenter(new Vector2(subject.WorldPosition.Center.X, new_y));
                        break;
                    }
            }
        }

        public Position GetTileAtIndices(int i, int j)
        {
            return new Position(new Vector2(i * TileSize, j * TileSize), new Vector2(TileSize / 2), 0, Vector2.Zero);
        }

        public (int, int) GetTileIndices(Vector2 coords)
        {
            return ((int)coords.X / TileSize, (int)coords.Y / TileSize);
        }

        public short GetTileAtCoords(Vector2 coords)
        {
            var (i, j) = GetTileIndices(coords);
            return TileMap.Grid[i, j].Item1;
        }

        public void RemoveTileAtCoords(Vector2 coords)
        {
            var (i, j) = GetTileIndices(coords);
            TileMap.Grid[i, j] = (0, 0);
        }

        public GameObject GetObjectAtCoords(Vector2 coords)
        {
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
