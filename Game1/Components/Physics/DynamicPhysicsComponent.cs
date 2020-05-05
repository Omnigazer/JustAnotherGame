using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Omniplatformer.Enums;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Physics
{
    public class DynamicPhysicsComponent : PhysicsComponent
    {
        const float epsilon = 0.01f;
        // Collision interaction flags
        public PhysicsComponent CurrentGround { get; set; }
        public bool IsInLiquid { get; set; }
        public float LiquidImmersion { get; set; }
        public bool IsNextToCeiling { get; set; }
        public bool IsNextToLeftWall { get; set; }
        public bool IsNextToRightWall { get; set; }
        public bool IsNextToWall => IsNextToLeftWall || IsNextToRightWall;
        public bool IsNextToRope { get; set; }
        public bool CanClimb { get; set; }

        [JsonProperty]
        protected Vector2 CurrentMovement { get; set; }

        // Movement counters and flags
        public Direction MoveDirection { get; set; }

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

        public DynamicPhysicsComponent() { }

        public DynamicPhysicsComponent(GameObject obj, Vector2 coords, Vector2 halfsize): base(obj, coords, halfsize)
        {

        }

        public virtual void Move(float dt)
        {
            AdjustPosition(CurrentMovement * dt);
        }

        /// <summary>
        /// Applies the specified impulse to the body
        /// </summary>
        /// <param name="impulse">The value for the impulse</param>
        /// <param name="ignore_mass">If set to true, assumes a mass of 1 to set the speed directly.</param>
        public void ApplyImpulse(Vector2 impulse, bool ignore_mass = false)
        {
            CurrentMovement += impulse * (ignore_mass ? 1 : InverseMass);
        }

        public virtual void ProcessMovement(float dt)
        {

        }

        public virtual bool ProcessCollision(Direction dir, PhysicsComponent obj)
        {
            return false;
        }

        public virtual void ResetCollisionFlags()
        {
            CurrentGround = null;
        }

        // Set speed < e to zero to prevent shaking
        public void TrimSpeed()
        {
            if (Math.Abs(VerticalSpeed) < epsilon)
            {
                VerticalSpeed = 0;
            }
            if (Math.Abs(HorizontalSpeed) < epsilon)
            {
                HorizontalSpeed = 0;
            }
        }
    }
}
