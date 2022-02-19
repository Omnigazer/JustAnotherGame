using System;
using Microsoft.Xna.Framework;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Characters;

namespace Omniplatformer.Components.Physics
{
    public class PlayerMoveComponent : CharMoveComponent
    {
        /* Movement constants */
        const float jump_accel = 4;
        const float max_jumpspeed = 24;
        const float soft_jump_cap = 0.3f * max_jumpspeed;

        /* Movement dynamic caps */
        public int max_jumps = 2;
        public int max_jump_time = 9; // Max jump time
        const int wall_jump_pin_ticks = 1; // Time required to be spent against the wall to walljump

        /* Movement counters */
        public int remaining_jumps;
        public float current_jump_time;
        public float current_pin_time; // represents time spent against a wall or a climbable object, such as a rope
        int drop_transparency_counter; // checks how many frames player has been inside a drop-platform

        /* Movement flags */
        public bool IsClimbing { get; set; }
        public bool IsJumping { get; set; }
        public bool IsCrouching { get; set; }
        public bool IsDropping { get; set; }

        public float ChassisSpeed { get; set; }

        public PlayerMoveComponent()
        {
            Solid = true;
            MaxMoveSpeed = 5;
            Acceleration = 0.8f;
            InverseMass = 0.1f;
        }

        public override void Tick(float dt)
        {
            current_jump_time -= dt;

            if (IsJumping && current_jump_time <= 0)
            {
                StopJumping();
            }

            if (IsInLiquid && !IsJumping)
            {
                ResetJumps();
            }

            if (IsNextToWall || IsNextToRope)
            {
                IncreasePinTime(dt);
            }
            else
            {
                ResetPin();
            }

            base.Tick(dt);
        }

        public void IncreasePinTime(float dt)
        {
            current_pin_time += dt;
            if (current_pin_time >= wall_jump_pin_ticks)
                CanClimb = true;
        }

        // Signifies sufficient time spent against a wall (or a rope)
        public bool IsPinnedToWall()
        {
            return current_pin_time >= wall_jump_pin_ticks;
        }

        // Stop treating the character as pinned/adjacent to the wall
        public void ResetPin()
        {
            current_pin_time = 0;
            CanClimb = false;
            StopClimbing();
        }

        public void StartClimbing()
        {
            if (CanClimb && !IsClimbing)
            {
                IsClimbing = true;
            }
        }

        public void StopClimbing()
        {
            IsClimbing = false;
        }

        public void StartDropping()
        {
            IsDropping = true;
        }

        public void StopDropping()
        {
            IsDropping = false;
        }

        public void Jump()
        {
            if (CanJump() && !IsJumping)
            {
                IsJumping = true;
                if (IsPinnedToWall())
                {
                    if (IsNextToLeftWall)
                        CurrentMovement += new Vector2(6, 40);
                    else if (IsNextToRightWall)
                        CurrentMovement += new Vector2(-6, 40);
                    ResetJumps();
                }
                ResetPin();
                current_jump_time = max_jump_time;
                // ClearCurrentPlatform();
                remaining_jumps--;
            }
        }

        public void Crouch()
        {
            if (IsCrouching)
                return;

            IsCrouching = true;
            SetLocalHalfsize(WorldPosition.Halfsize / 2);
        }

        public void Stand()
        {
            if (!IsCrouching)
                return;

            IsCrouching = false;
            SetLocalHalfsize(WorldPosition.Halfsize * 2);
        }

        public void StopJumping()
        {
            IsJumping = false;
        }

        public bool CanJump()
        {
            return ((remaining_jumps > 0) || (IsPinnedToWall()));
        }

        public void ResetJumps()
        {
            remaining_jumps = max_jumps;
        }

        // TODO: problematic method & overrides, refactor
        public override void ResetCollisionFlags()
        {
            CanClimb = false;
            if (drop_transparency_counter > 0)
                drop_transparency_counter--;
            base.ResetCollisionFlags();
        }

        public override bool ShouldImpactObject(PhysicsComponent target, Direction dir)
        {
            if (target.DropPlatform)
            {
                if (IsDropping || dir != Direction.Down || drop_transparency_counter > 0)
                {
                    drop_transparency_counter = 2;
                    return false;
                }
            }

            return target.Solid || target.Liquid;
        }

        public override void ProcessCollision(Direction direction, PhysicsComponent target)
        {
            base.ProcessCollision(direction, target);
            // TODO: refactor this
            if (ShouldImpactObject(target, direction))
                if (target.Solid)
                {
                    if (direction == Direction.Up)
                    {
                        StopJumping();
                    }
                    if (direction == Direction.Down)
                    {
                        if (!IsJumping)
                            ResetJumps();
                    }
                }
                else if (target.Liquid)
                {
                    if (!IsJumping)
                        ResetJumps();
                }
        }

        public override float GetUpSpeedCap()
        {
            if (IsJumping)
            {
                return soft_jump_cap;
            }
            else
            {
                return max_jumpspeed;
            }
        }

        public void ProcessClimbing()
        {
            switch (MoveDirection)
            {
                case Direction.Up:
                    {
                        CurrentMovement = new Vector2(0, ClimbSpeed);
                        break;
                    }
                case Direction.Down:
                    {
                        CurrentMovement = new Vector2(0, -ClimbSpeed);
                        break;
                    }
                default:
                    {
                        CurrentMovement = Vector2.Zero;
                        break;
                    }
            }
        }

        // TODO: refactor this
        public override void ProcessMovement(float dt)
        {
            if (IsClimbing)
            {
                ProcessClimbing();
            }
            else
            {
                ProcessWalking(dt);
                if (IsJumping)
                {
                    CurrentMovement += new Vector2(0, jump_accel);
                }

                TrimSpeed();
                CapMovement();
            }
        }

        public override void ProcessWalking(float dt)
        {
            var pos = GetComponent<PositionComponent>();
            switch (MoveDirection)
            {
                case Direction.Left:
                    {
                        pos.SetLocalFace(HorizontalDirection.Left);
                        // CurrentMovement += new Vector2(-move_speed, 0);
                        ChassisSpeed += -Acceleration * dt;
                        break;
                    }
                case Direction.Right:
                    {
                        pos.SetLocalFace(HorizontalDirection.Right);
                        // CurrentMovement += new Vector2(move_speed, 0);
                        ChassisSpeed += Acceleration * dt;
                        break;
                    }
                case Direction.None:
                    {
                        ChassisSpeed = 0;
                        break;
                    }
            }
        }

        public override void CapMovement()
        {
            float fall_cap = GetDownSpeedCap();
            float capped_y = Math.Max(CurrentMovement.Y, fall_cap);
            capped_y = Math.Min(capped_y, GetUpSpeedCap());
            if (Math.Abs(ChassisSpeed) > GetHorizontalSpeedCap())
            {
                ChassisSpeed = GetHorizontalSpeedCap() * Math.Sign(ChassisSpeed);
            }
            VerticalSpeed = capped_y;
        }
    }
}
