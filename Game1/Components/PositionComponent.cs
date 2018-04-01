using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public class PositionComponent : Component
    {
        // TODO: TEST
        public PositionComponent parent_pos;
        public float RotationAngle { get; set; }
        protected Vector2 _local_center;
        // public virtual Vector2 Center { get => _center; set => _center = value; }
        public virtual Vector2 Center => parent_pos != null ? _local_center * parent_pos : _local_center;
        public Vector2 halfsize;

        public PositionComponent(GameObject obj, Vector2 center, Vector2 halfsize) : base(obj)
        {
            _local_center = center;
            this.halfsize = halfsize;
        }        
        
        public static Vector2 operator *(Vector2 vector, PositionComponent position)
        {            
            return position.Center + vector;            
        }

        public void AdjustPosition(Vector2 displacement)
        {
            _local_center += displacement;
        }

        public void SetLocalPosition(Vector2 position)
        {
            _local_center = position;
        }

        public Rectangle GetRectangle()
        {
            var pt = halfsize.ToPoint();
            pt.X *= 2;
            pt.Y *= 2;
            return new Rectangle((Center - halfsize).ToPoint(), pt);
        }        

        public bool Contains(Vector2 pt)
        {
            if ((pt.X >= Center.X - halfsize.X && pt.X <= Center.X + halfsize.X) && (pt.Y >= Center.Y - halfsize.Y && pt.Y <= Center.Y + halfsize.Y))
                return true;
            return false;
        }

        public bool Overlaps(GameObject obj)
        {
            PositionComponent pos = (PositionComponent)obj;
            if (pos == null)
                return false;
            
            if (Math.Abs(Center.X - pos.Center.X) > halfsize.X + pos.halfsize.X) return false;
            if (Math.Abs(Center.Y - pos.Center.Y) > halfsize.Y + pos.halfsize.Y) return false;
            return true;            
        }

        public Direction Collides(GameObject other)
        {
            PositionComponent their_pos = (PositionComponent)other;
            if (their_pos == null)
                return Direction.None;

            if (Overlaps(other))
            {
                var hd = Math.Abs(Center.X - their_pos.Center.X) - (halfsize.X + their_pos.halfsize.X);
                var vd = Math.Abs(Center.Y - their_pos.Center.Y) - (halfsize.Y + their_pos.halfsize.Y);

                // Now compare them to know the side of collision

                if (hd > vd)
                {
                    if (Center.X < their_pos.Center.X)
                        return Direction.Right;
                    else
                        return Direction.Left;
                }
                else if (vd > hd)
                {
                    if (Center.Y < their_pos.Center.Y)
                        return Direction.Up;
                    else
                        return Direction.Down;
                }
            }
            return Direction.None;
        }

        public float GetIntersectionArea(GameObject obj)
        {
            PositionComponent their_pos = (PositionComponent)obj;
            if (their_pos == null)
                return 0;

            var my_rect = GetRectangle();
            var their_rect = their_pos.GetRectangle();
            float intersection_width = Math.Min(my_rect.Right, their_rect.Right) - Math.Max(my_rect.Left, their_rect.Left);
            float intersection_height = Math.Min(my_rect.Bottom, their_rect.Bottom) - Math.Max(my_rect.Top, their_rect.Top);
            return intersection_width * intersection_height;
        }

        public float GetImmersionShare(GameObject obj)
        {
            var my_rect = GetRectangle();
            float my_area = (float)(my_rect.Width * my_rect.Height);
            return GetIntersectionArea(obj) / my_area;
        }            
    }
}
