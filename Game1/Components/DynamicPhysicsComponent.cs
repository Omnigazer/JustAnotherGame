﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public class DynamicPhysicsComponent : PhysicsComponent
    {
        // Collision interaction flags
        public bool IsOnGround { get; set; }
        public bool IsInLiquid { get; set; }
        public float LiquidImmersion { get; set; }
        public bool IsNextToCeiling { get; set; }
        public bool IsNextToLeftWall { get; set; }
        public bool IsNextToRightWall { get; set; }
        public bool IsNextToWall => IsNextToLeftWall || IsNextToRightWall;
        public bool IsNextToRope { get; set; }
        public bool CanClimb { get; set; }

        public Vector2 CurrentMovement { get; set; }

        // Movement counters and flags
        public Direction move_direction;

        public float VerticalSpeed
        {
            get => CurrentMovement.Y;
            set => CurrentMovement = new Vector2(CurrentMovement.X, value);
        }

        public float HorizontalSpeed
        {
            get => CurrentMovement.X;
            set => CurrentMovement = new Vector2(value, CurrentMovement.Y);
        }

        public DynamicPhysicsComponent(GameObject obj, Vector2 coords, Vector2 halfsize): base(obj, coords, halfsize)
        {

        }

        public virtual void Move(float time_scale)
        {
            AdjustPosition(CurrentMovement * time_scale);
        }

        public virtual void ProcessMovement()
        {

        }

        public virtual void ProcessCollision(Direction dir, PhysicsComponent obj)
        {

        }

        public virtual void ResetCollisionFlags()
        {
            /*
            foreach (var (direction, obj) in collisions)
            {
                ProcessCollision(direction, obj);
            }
            */
        }

        // Set speed < e to zero to prevent shaking
        public void TrimSpeed()
        {
            float e = 0.1f;
            if (Math.Abs(VerticalSpeed) < e)
            {
                VerticalSpeed = 0;
            }
            if (Math.Abs(HorizontalSpeed) < e)
            {
                HorizontalSpeed = 0;
            }
        }
    }
}
