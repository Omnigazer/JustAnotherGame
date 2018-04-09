﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public class CharMoveComponent : MoveComponent
    {
        // Movement counters and flags
        // public bool CanClimb { get; set; }

        // Collision interaction flags
        public bool IsOnGround { get; set; }
        public bool IsInLiquid { get; set; }
        public float LiquidImmersion { get; set; }
        public bool IsNextToCeiling { get; set; }
        public bool IsNextToLeftWall { get; set; }
        public bool IsNextToRightWall { get; set; }
        public bool IsNextToWall => IsNextToLeftWall || IsNextToRightWall;
        public bool IsNextToRope { get; set; }

        // Movement constants
        protected const float gravity = 1f;
        protected const float max_fall_speed = 20;
        protected const float water_friction = 0.1f;

        // Movement dynamic caps
        public float MaxMoveSpeed { get; set; }
        public float ClimbSpeed => 3;
        public float Acceleration => 0.5f;

        // Movement counters and flags
        public Direction move_direction;

        public GameObject CurrentPlatform { get; set; } // Platform the character is currently standing on

        public CharMoveComponent(GameObject obj) : base(obj)
        {
            MaxMoveSpeed = 5;
        }

        public CharMoveComponent(GameObject obj, float movespeed) : base(obj)
        {
            MaxMoveSpeed = movespeed;
        }

        // Check what kinds of objects are we colliding here
        // TODO: problematic method & overrides, refactor
        public override void ProcessCollisionInteractions(List<(Direction, GameObject)> collisions)
        {
            // default all interactions to false
            IsOnGround = false;
            IsInLiquid = false;
            LiquidImmersion = 0f;
            IsNextToCeiling = false;
            IsNextToLeftWall = false;
            IsNextToRightWall = false;
            IsNextToRope = false;

            base.ProcessCollisionInteractions(collisions);

            if (!IsOnGround)
            {
                ClearCurrentPlatform();
            }
        }

        protected override void ProcessCollision(Direction direction, GameObject obj)
        {
            // TODO: make the component acquisition less costly
            var pos = GetComponent<PositionComponent>();
            var movable = GetComponent<MoveComponent>();
            if (obj.Solid)
            {
                var new_direction = pos.Collides(obj);
                PinTo(obj, new_direction);
                if (new_direction == Direction.Down) { IsOnGround = true; }
                else if (new_direction == Direction.Up) { IsNextToCeiling = true; }
                else if (new_direction == Direction.Left)
                    IsNextToLeftWall = true;
                else if (new_direction == Direction.Right)
                    IsNextToRightWall = true;
            }
            else if (obj.Liquid)
            {
                IsInLiquid = true;
                LiquidImmersion = pos.GetImmersionShare(obj);
            }
            else if (obj.Climbable)
            {
                IsNextToRope = true;
            }

            // TODO: refactor this
            if (obj.Hittable)
            {
                var hittable = GetComponent<HitComponent>();
                hittable?.Hit(obj);
            }

            base.ProcessCollision(direction, obj);
        }

        public override Vector2 GetMoveVector()
        {
            var movable = GetComponent<MoveComponent>();
            ProcessWalking();
            ApplyGravity();

            if (IsInLiquid)
            {
                ProcessLiquid();
            }

            movable.TrimSpeed();
            RestrictMovement();
            CapMovement();

            return movable.CurrentMovement;
        }

        public void ProcessWalking()
        {
            var pos = GetComponent<PositionComponent>();
            switch (move_direction)
            {
                case Direction.Left:
                    {
                        pos.SetLocalFace(HorizontalDirection.Left);
                        // CurrentMovement += new Vector2(-move_speed, 0);
                        CurrentMovement += new Vector2(-Acceleration, 0);
                        break;
                    }
                case Direction.Right:
                    {
                        pos.SetLocalFace(HorizontalDirection.Right);
                        // CurrentMovement += new Vector2(move_speed, 0);
                        CurrentMovement += new Vector2(Acceleration, 0);
                        break;
                    }
                default:
                    {
                        // CurrentMovement = new Vector2(0, CurrentMovement.Y);
                        if (CurrentPlatform != null)
                        {
                            // Math.Sign()
                            CurrentMovement += new Vector2(GetHorizontalFriction(), 0);
                        }
                        break;
                    }
            }
        }

        public void PinTo(GameObject target, Direction direction)
        {
            PositionComponent pos = GetComponent<PositionComponent>();
            PositionComponent their_pos = target.GetComponent<PositionComponent>();

            switch (direction)
            {
                // TODO: Refactor this to move, although this should never happen to objects that can't move
                case Direction.Right:
                    {
                        var new_x = their_pos.WorldPosition.Center.X - (their_pos.WorldPosition.halfsize.X + pos.WorldPosition.halfsize.X);
                        pos.SetLocalCenter(new Vector2(new_x, pos.WorldPosition.Center.Y));
                        break;
                    }
                case Direction.Left:
                    {
                        var new_x = their_pos.WorldPosition.Center.X + (their_pos.WorldPosition.halfsize.X + pos.WorldPosition.halfsize.X);
                        pos.SetLocalCenter(new Vector2(new_x, pos.WorldPosition.Center.Y));
                        break;
                    }
                case Direction.Up:
                    {
                        var new_y = their_pos.WorldPosition.Center.Y - (their_pos.WorldPosition.halfsize.Y + pos.WorldPosition.halfsize.Y);
                        pos.SetLocalCenter(new Vector2(pos.WorldPosition.Center.X, new_y));
                        break;
                    }
                case Direction.Down:
                    {
                        var new_y = their_pos.WorldPosition.Center.Y + (their_pos.WorldPosition.halfsize.Y + pos.WorldPosition.halfsize.Y);
                        pos.SetLocalCenter(new Vector2(pos.WorldPosition.Center.X, new_y));

                        if (CurrentPlatform == null)
                        {
                            var movable = target.GetComponent<MoveComponent>();
                            if (movable != null)
                            {
                                movable._onMove += Target_onMove;
                            }
                            CurrentPlatform = target;
                        }
                        break;
                    }
            }
        }

        public void ClearCurrentPlatform()
        {
            if (CurrentPlatform != null)
            {
                var movable = CurrentPlatform.GetComponent<MoveComponent>();
                if (movable != null)
                    movable._onMove -= Target_onMove;
            }
            CurrentPlatform = null;
        }

        private void Target_onMove(object sender, MoveEventArgs e)
        {
            Move(e.displacement);
        }

        // TODO: maybe extract this into another component
        public virtual void ProcessLiquid()
        {
            var movable = GetComponent<MoveComponent>();
            // Apply "Archimedes"
            movable.VerticalSpeed += 0.7f * gravity * LiquidImmersion;

            // Apply water friction
            movable.CurrentMovement += new Vector2(-movable.CurrentMovement.X * water_friction, -movable.CurrentMovement.Y * water_friction);
        }

        // TODO: maybe extract this into another component
        public virtual void ApplyGravity()
        {
            var movable = GetComponent<MoveComponent>();
            movable.CurrentMovement += new Vector2(0, -gravity);
        }

        public virtual void RestrictMovement()
        {
            if (IsNextToCeiling)
            {
                if (CurrentMovement.Y > 0)
                    CurrentMovement = new Vector2(CurrentMovement.X, 0);
            }

            if (IsOnGround)
            {
                if (CurrentMovement.Y < 0)
                    CurrentMovement = new Vector2(CurrentMovement.X, 0);
            }
        }

        public virtual void CapMovement()
        {
            var movable = GetComponent<MoveComponent>();

            float fall_cap = GetDownSpeedCap();
            float capped_y = Math.Max(movable.CurrentMovement.Y, fall_cap);
            capped_y = Math.Min(capped_y, GetUpSpeedCap());
            if (Math.Abs(HorizontalSpeed) > GetHorizontalSpeedCap())
            {
                HorizontalSpeed -= 2 * Acceleration * Math.Sign(HorizontalSpeed);
            }
            VerticalSpeed = capped_y;
        }

        /// <summary>
        /// Maximum horizontal speed
        /// </summary>
        /// <returns></returns>
        public virtual float GetHorizontalSpeedCap()
        {
            return MaxMoveSpeed;
        }

        /// <summary>
        /// Maximum "up" speed
        /// </summary>
        /// <returns></returns>
        public virtual float GetUpSpeedCap()
        {
            return max_fall_speed;
        }

        /// <summary>
        /// Maximum "down" speed
        /// </summary>
        /// <returns></returns>
        public virtual float GetDownSpeedCap()
        {
            return -max_fall_speed;
        }

        public float GetHorizontalFriction()
        {
            if (Math.Abs(CurrentPlatform.Friction) <= Math.Abs(CurrentMovement.X))
            {
                return -CurrentPlatform.Friction * Math.Sign(CurrentMovement.X);
            }
            else
            {
                return -CurrentMovement.X;
            }
        }
    }
}
