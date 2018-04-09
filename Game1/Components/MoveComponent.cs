using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
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

        public MoveComponent(GameObject obj) : base(obj)
        {

        }

        protected void onMove(Vector2 displacement)
        {
            _onMove(this, new MoveEventArgs(displacement));
        }

        // Check what kinds of objects are we colliding here
        // TODO: problematic method & overrides, refactor
        public virtual void ProcessCollisionInteractions(List<(Direction, GameObject)> collisions)
        {
            foreach (var (direction, obj) in collisions)
            {
                ProcessCollision(direction, obj);
            }
        }

        protected virtual void ProcessCollision(Direction direction, GameObject obj)
        {

        }

        public virtual Vector2 GetMoveVector()
        {
            return CurrentMovement;
        }

        public virtual void Move()
        {
            var displacement = GetMoveVector();
            Move(displacement);
        }

        public virtual void AdjustSpeed(Vector2 v)
        {
            CurrentMovement += v;
        }

        public virtual void Move(Vector2 displacement)
        {
            var pos = GetComponent<PositionComponent>();
            pos.AdjustPosition(displacement);
            onMove(displacement);
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
