using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Utility.EventHandlers;

namespace Omniplatformer.Components.Physics
{
    public class MoveComponent : Component
    {
        public Vector2 CurrentMovement { get; set; }
        public event EventHandler<MoveEventArgs> _onMove = delegate { };
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

        public MoveComponent() { }
        public MoveComponent(GameObject obj) : base(obj)
        {

        }

        /*
        protected void onMove(Vector2 displacement)
        {
            _onMove(this, new MoveEventArgs(displacement));
        }
        */

        // Check what kinds of objects are we colliding here
        // TODO: problematic method & overrides, refactor
        public virtual void ProcessCollisionInteractions(List<(Direction, GameObject)> collisions)
        {
            foreach (var (direction, obj) in collisions)
            {
                ProcessCollision(direction, obj);
            }
        }

        public virtual void ProcessCollision(Direction direction, GameObject obj)
        {

        }

        public virtual void ProcessMovement()
        {

        }

        public virtual void AdjustSpeed(Vector2 v)
        {
            CurrentMovement += v;
        }

        public virtual void Move(Vector2 displacement)
        {
            var pos = GetComponent<PositionComponent>();
            pos.AdjustPosition(displacement);
            // onMove(displacement);
        }
    }
}
