using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public class PlayerMoveComponent : CharMoveComponent
    {
        // Movement constants
        const float jump_accel = 5;
        const float max_jumpspeed = 30;
        const float soft_jump_cap = 0.3f * max_jumpspeed;

        // Movement dynamic caps
        public int max_jumps = 2;
        public int max_jump_ticks = 15; // Max jump time
        const int wall_jump_pin_ticks = 1; // Time required to be spent against the wall to walljump

        // Movement counters
        public int remaining_jumps;
        public float current_jump_time;
        public float current_pin_time; // represents time spent against a wall or a climbable object, such as a rope

        public bool CanClimb { get; set; }
        public bool IsClimbing { get; set; }
        public bool IsJumping { get; set; }

        public PlayerMoveComponent(GameObject obj) : base(obj)
        {
        }

        public override void Tick(float time_scale)
        {
            current_jump_time -= time_scale;
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
                IncreasePinTime(time_scale);
            }
            else
            {
                ResetPin();
            }

            base.Tick(time_scale);
        }

        public void IncreasePinTime(float time_scale)
        {
            current_pin_time += time_scale;
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
                current_jump_time = max_jump_ticks;
                // ClearCurrentPlatform();
                remaining_jumps--;
            }
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

        public override void ApplyGravity()
        {
            var movable = GetComponent<MoveComponent>();
            if (!IsJumping)
                movable.CurrentMovement += new Vector2(0, -gravity);
        }

        // TODO: problematic method & overrides, refactor
        public override void ProcessCollisionInteractions(List<(Direction, GameObject)> collisions)
        {
            CanClimb = false;
            base.ProcessCollisionInteractions(collisions);
            /*
            if (IsInLiquid && !IsJumping)
            {
                ResetJumps();
            }
            if (IsNextToWall || IsNextToRope)
            {
                IncreasePinTicks();
            }
            else
            {
                ResetPin();
            }
            */
        }

        protected override void ProcessCollision(Direction direction, GameObject obj)
        {
            base.ProcessCollision(direction, obj);
            if (obj.Pickupable)
            {
                // TODO: definitely extract this into a component
                var player = (Player)GameObject;
                // player.GetBonus()
                // player.Pickup((Collectible)obj);
                player.Pickup(obj);
            }
        }

        public override void RestrictMovement()
        {
            base.RestrictMovement();
            if (IsNextToCeiling)
            {
                StopJumping();
            }

            if (IsOnGround)
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
            var movable = GetComponent<MoveComponent>();
            switch (move_direction)
            {
                case Direction.Up:
                    {
                        movable.CurrentMovement = new Vector2(0, ClimbSpeed);
                        break;
                    }
                case Direction.Down:
                    {
                        movable.CurrentMovement = new Vector2(0, -ClimbSpeed);
                        break;
                    }
                default:
                    {
                        movable.CurrentMovement = Vector2.Zero;
                        break;
                    }
            }
        }

        // TODO: refactor this
        public override Vector2 GetMoveVector()
        {
            if (IsClimbing)
            {
                ProcessClimbing();
                RestrictMovement();
                return CurrentMovement;
            }
            else
            {
                ProcessWalking();
                ApplyGravity();

                if (IsJumping)
                {
                    CurrentMovement += new Vector2(0, jump_accel);
                }

                if (IsInLiquid)
                {
                    ProcessLiquid();
                }

                TrimSpeed();
                RestrictMovement();
                CapMovement();

                return CurrentMovement;
            }
        }
    }
}
