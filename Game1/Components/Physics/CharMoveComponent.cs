using System;
using Microsoft.Xna.Framework;
using Omniplatformer.Enums;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Physics
{
    // public class CharMoveComponent : MoveComponent
    public class CharMoveComponent : DynamicPhysicsComponent
    {
        // Movement constants
        protected const float max_fall_speed = 20;

        // Movement dynamic caps
        public float MaxMoveSpeed { get; set; }
        public float ClimbSpeed => 3;
        public float Acceleration { get; set; } = 0.5f;

        // Movement counters and flags
        // public Direction move_direction;

        public CharMoveComponent(GameObject obj, Vector2 coords, Vector2 halfsize) : base(obj, coords, halfsize)
        {
            MaxMoveSpeed = 5;
            Hittable = true;
            Solid = true;
        }

        public CharMoveComponent(GameObject obj, Vector2 coords, Vector2 halfsize, float movespeed) : base(obj, coords, halfsize)
        {
            MaxMoveSpeed = movespeed;
            Hittable = true;
            Solid = true;
        }

        // Check what kinds of objects are we colliding here
        // TODO: problematic method & overrides, refactor
        public override void ResetCollisionFlags()
        {
            // default all interactions to false
            IsInLiquid = false;
            LiquidImmersion = 0f;
            IsNextToCeiling = false;
            IsNextToLeftWall = false;
            IsNextToRightWall = false;
            IsNextToRope = false;

            base.ResetCollisionFlags();
        }

        public override bool ProcessCollision(Direction direction, PhysicsComponent obj)
        {
            if (obj.Solid)
            {
                // if (direction == Direction.Down) { IsOnGround = true; }
                if (direction == Direction.Up) { IsNextToCeiling = true; }
                else if (direction == Direction.Left)
                    IsNextToLeftWall = true;
                else if (direction == Direction.Right)
                    IsNextToRightWall = true;
            }
            else if (obj.Climbable)
            {
                IsNextToRope = true;
            }

            // TODO: refactor this
            if (obj.Hittable)
            {
                var hittable = GetComponent<HitComponent>();
                hittable?.Hit(obj.GameObject);
            }

            base.ProcessCollision(direction, obj);
            return false;
        }

        public override void ProcessMovement(float dt)
        {
            ProcessWalking(dt);
            TrimSpeed();
            CapMovement();
        }

        public virtual void ProcessWalking(float dt)
        {
            var pos = GetComponent<PositionComponent>();
            switch (MoveDirection)
            {
                case Direction.Left:
                    {
                        pos.SetLocalFace(HorizontalDirection.Left);
                        // CurrentMovement += new Vector2(-move_speed, 0);
                        CurrentMovement += new Vector2(-Acceleration * dt, 0);
                        break;
                    }
                case Direction.Right:
                    {
                        pos.SetLocalFace(HorizontalDirection.Right);
                        // CurrentMovement += new Vector2(move_speed, 0);
                        CurrentMovement += new Vector2(Acceleration * dt, 0);
                        break;
                    }
            }
        }

        /*
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


        public virtual void ProcessLiquid()
        {
            // Apply "Archimedes"
            VerticalSpeed += 0.7f * 1f * LiquidImmersion;

            // Apply water friction
            CurrentMovement += new Vector2(-CurrentMovement.X * water_friction, -CurrentMovement.Y * water_friction);
        }
        */

        public virtual void CapMovement()
        {
            float fall_cap = GetDownSpeedCap();
            float capped_y = Math.Max(CurrentMovement.Y, fall_cap);
            capped_y = Math.Min(capped_y, GetUpSpeedCap());
            if (Math.Abs(HorizontalSpeed) > GetHorizontalSpeedCap())
            {
                // HorizontalSpeed -= 2 * Acceleration * Math.Sign(HorizontalSpeed) * (Math.Abs(HorizontalSpeed) - GetHorizontalSpeedCap());
                HorizontalSpeed = GetHorizontalSpeedCap() * Math.Sign(HorizontalSpeed);
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



        /*
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
        */
    }
}
