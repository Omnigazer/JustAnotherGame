using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    // public class CharMoveComponent : MoveComponent
    public class CharMoveComponent : DynamicPhysicsComponent
    {
        // Movement constants
        protected const float max_fall_speed = 20;
        protected const float water_friction = 0.35f;

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
        }

        public CharMoveComponent(GameObject obj, Vector2 coords, Vector2 halfsize, float movespeed) : base(obj, coords, halfsize)
        {
            MaxMoveSpeed = movespeed;
            Hittable = true;
        }

        // Check what kinds of objects are we colliding here
        // TODO: problematic method & overrides, refactor
        public override void ResetCollisionFlags()
        {
            // default all interactions to false
            IsOnGround = false;
            IsInLiquid = false;
            LiquidImmersion = 0f;
            IsNextToCeiling = false;
            IsNextToLeftWall = false;
            IsNextToRightWall = false;
            IsNextToRope = false;

            base.ResetCollisionFlags();
        }

        public override void ProcessCollision(Direction direction, PhysicsComponent obj)
        {
            // TODO: make the component acquisition less costly
            var pos = GetComponent<PositionComponent>();
            if (obj.Solid)
            {
                if (direction == Direction.Down) { IsOnGround = true; }
                else if (direction == Direction.Up) { IsNextToCeiling = true; }
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
        }

        public override void ProcessMovement()
        {
            ProcessWalking();
            TrimSpeed();
            CapMovement();
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
                        // if (CurrentPlatform != null)
                        if (IsOnGround)
                        {
                            // Math.Sign()
                            // CurrentMovement += new Vector2(GetHorizontalFriction(), 0);
                        }
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
