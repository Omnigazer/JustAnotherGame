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
        const float jump_accel = 4;
        const float max_jumpspeed = 24;
        const float soft_jump_cap = 0.3f * max_jumpspeed;

        // Movement dynamic caps
        public int max_jumps = 2;
        public int max_jump_time = 9; // Max jump time
        const int wall_jump_pin_ticks = 1; // Time required to be spent against the wall to walljump

        // Movement counters
        public int remaining_jumps;
        public float current_jump_time;
        public float current_pin_time; // represents time spent against a wall or a climbable object, such as a rope

        // public bool CanClimb { get; set; }
        public bool IsClimbing { get; set; }
        public bool IsJumping { get; set; }

        public PlayerMoveComponent(GameObject obj, Vector2 coords, Vector2 halfsize) : base(obj, coords, halfsize)
        {
            MaxMoveSpeed = 5;
            Acceleration = 0.8f;
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
            base.ResetCollisionFlags();
        }

        public override bool ProcessCollision(Direction direction, PhysicsComponent obj)
        {
            base.ProcessCollision(direction, obj);
            if (direction == Direction.Up && obj.Solid)
            {
                StopJumping();
            }
            if (direction == Direction.Down)
            {
                if (!IsJumping)
                    ResetJumps();
            }

            if (obj.Pickupable)
            {
                // TODO: definitely extract this into a component
                var player = (Player)GameObject;
                // player.GetBonus()
                // player.Pickup((Collectible)obj);
                player.Pickup(obj.GameObject);
                obj.Pickupable = false;
            }
            return false;
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
            switch (move_direction)
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
    }
}
