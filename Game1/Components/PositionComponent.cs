using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public struct Position
    {
        public Vector2 Center { get; set; }
        public Vector2 halfsize;

        // The rotation origin
        public Vector2 Origin { get; set; }
        public float RotationAngle { get; set; }
        public HorizontalDirection face_direction;

        // used for from-position object initializers
        public Position(Position position)
        {
            Center = position.Center;
            halfsize = position.halfsize;
            Origin = position.Origin;
            RotationAngle = position.RotationAngle;
            face_direction = position.face_direction;
        }

        public Position(Vector2 center, Vector2 halfsize, float angle = 0, HorizontalDirection dir = HorizontalDirection.Right)
        {
            Center = center;
            this.halfsize = halfsize;
            RotationAngle = angle;
            face_direction = dir;
            Origin = new Vector2(0.5f, 0.5f);
        }

        public Position(Vector2 center, Vector2 halfsize, float angle, Vector2 origin, HorizontalDirection dir = HorizontalDirection.Right)
        {
            Center = center;
            this.halfsize = halfsize;
            RotationAngle = angle;
            face_direction = dir;
            Origin = origin;
        }

        /*
        public static Vector2 operator *(Position a, Position b)
        {                
            var dir_multiplier = (b.face_direction == Direction.Left ? -1 : 1);
            var v = new Vector2(a.Center.X * dir_multiplier, a.Center.Y);                
            return Vector2.Transform(v, Matrix.CreateRotationZ(-b.RotationAngle) * Matrix.CreateTranslation(b.Center.X, b.Center.Y, 0));
        } 
        */
        public static Position operator *(Position a, Position b)
        {
            var dir_multiplier = (int)b.face_direction;
            var v = new Vector2(a.Center.X * dir_multiplier, a.Center.Y);
            return new Position(
                    Vector2.Transform(v, Matrix.CreateRotationZ(-b.RotationAngle) * Matrix.CreateTranslation(b.Center.X, b.Center.Y, 0)),
                    a.halfsize,
                    //a.RotationAngle + b.RotationAngle,
                    dir_multiplier * a.RotationAngle + b.RotationAngle,
                    a.Origin,
                    // a.face_direction                    
                    (HorizontalDirection)((int)a.face_direction * (int)b.face_direction)
                );
            // 
        }
    }

    public class PositionComponent : Component
    {        
        // TODO: TEST        
        public PositionComponent parent_pos;
        public AnchorPoint parent_anchor = AnchorPoint.Default;

        public Position local_position;        
        public Position WorldPosition => parent_pos != null ? local_position * parent_pos.GetAnchor(parent_anchor) : local_position;
        
        // public float RotationAngle { get; set; }
        // protected Vector2 _local_center;
        // public virtual Vector2 Center { get => _center; set => _center = value; }
        // public virtual Vector2 Center => parent_pos != null ? _local_center * parent_pos : _local_center;
        // public virtual Vector2 Center => parent_pos != null ? local_position * parent_pos.local_position : _local_center;
        // public Vector2 halfsize;

        public PositionComponent(GameObject obj, Vector2 center, Vector2 halfsize) : base(obj)
        {
            local_position = new Position(center, halfsize);            
            // _local_center = center;
            // this.halfsize = halfsize;
        }

        public PositionComponent(GameObject obj, Vector2 center, Vector2 halfsize, float angle) : base(obj)
        {
            local_position = new Position(center, halfsize, angle);            
            // _local_center = center;
            // this.halfsize = halfsize;
        }

        public PositionComponent(GameObject obj, Vector2 center, Vector2 halfsize, float angle, Vector2 origin) : base(obj)
        {
            local_position = new Position(center, halfsize, angle, origin);            
            // _local_center = center;
            // this.halfsize = halfsize;
        }

        // TODO: refactor this
        public Dictionary<AnchorPoint, Position> DefaultAnchors { get; set; } = new Dictionary<AnchorPoint, Position>() { { AnchorPoint.Default, new Position() }, { AnchorPoint.Hand, new Position(new Vector2(12, 30), Vector2.Zero) } };
        public Dictionary<AnchorPoint, Position> CurrentAnchors { get; set; } = new Dictionary<AnchorPoint, Position>() { { AnchorPoint.Default, new Position() }, { AnchorPoint.Hand, new Position(new Vector2(12, 30), Vector2.Zero) } };

        public Position GetAnchor(AnchorPoint anchor_name)
        {
            switch (anchor_name)
            {
                // TODO: test
                case AnchorPoint.Hand:
                    {
                        // Position x = new Position(new Vector2(12, 30), Vector2.Zero);
                        var x = CurrentAnchors[AnchorPoint.Hand];
                        return x * local_position;
                    }
                default:
                    {
                        return local_position;                        
                    }
            }
        }

        public void ResetAnchors()
        {
            foreach (var (name, el) in DefaultAnchors)
            {
                CurrentAnchors[name] = el;
            }
        }

        public void ResetAnchor(AnchorPoint anchor_name)
        {
            CurrentAnchors[anchor_name] = DefaultAnchors[anchor_name];
        }

        /*
        public static Vector2 operator *(Vector2 vector, PositionComponent position)
        {                           
            vector.X *= (position.face_direction == Direction.Left ? -1 : 1);
            return Vector2.Transform(vector, Matrix.CreateRotationZ(-position.RotationAngle) * Matrix.CreateTranslation(position.Center.X, position.Center.Y, 0));                        
        }
        */

        public void AdjustPosition(Vector2 displacement)
        {
            local_position.Center += displacement;            
        }

        /*
        public void SetLocalPosition(Vector2 position)
        {
            _local_center = position;
        }
        */

        public Rectangle GetRectangle()
        {
            var pt = WorldPosition.halfsize.ToPoint();
            pt.X *= 2;
            pt.Y *= 2;
            return new Rectangle((WorldPosition.Center - WorldPosition.halfsize).ToPoint(), pt);
        }        

        public bool Contains(Vector2 pt)
        {
            if ((pt.X >= WorldPosition.Center.X - WorldPosition.halfsize.X && pt.X <= WorldPosition.Center.X + WorldPosition.halfsize.X) && (pt.Y >= WorldPosition.Center.Y - WorldPosition.halfsize.Y && pt.Y <= WorldPosition.Center.Y + WorldPosition.halfsize.Y))
                return true;
            return false;
        }

        public bool Overlaps(GameObject obj)
        {
            PositionComponent pos = (PositionComponent)obj;
            if (pos == null)
                return false;
            
            if (Math.Abs(WorldPosition.Center.X - pos.WorldPosition.Center.X) > WorldPosition.halfsize.X + pos.WorldPosition.halfsize.X) return false;
            if (Math.Abs(WorldPosition.Center.Y - pos.WorldPosition.Center.Y) > WorldPosition.halfsize.Y + pos.WorldPosition.halfsize.Y) return false;
            return true;            
        }

        public Direction Collides(GameObject other)
        {
            PositionComponent their_pos = (PositionComponent)other;
            if (their_pos == null)
                return Direction.None;

            if (Overlaps(other))
            {
                var hd = Math.Abs(WorldPosition.Center.X - their_pos.WorldPosition.Center.X) - (WorldPosition.halfsize.X + their_pos.WorldPosition.halfsize.X);
                var vd = Math.Abs(WorldPosition.Center.Y - their_pos.WorldPosition.Center.Y) - (WorldPosition.halfsize.Y + their_pos.WorldPosition.halfsize.Y);

                // Now compare them to know the side of collision

                if (hd > vd)
                {
                    if (WorldPosition.Center.X < their_pos.WorldPosition.Center.X)
                        return Direction.Right;
                    else
                        return Direction.Left;
                }
                else if (vd > hd)
                {
                    if (WorldPosition.Center.Y < their_pos.WorldPosition.Center.Y)
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

        public void Rotate(float angle)
        {
            local_position.RotationAngle += angle;
        }

        public void SetLocalFace(HorizontalDirection direction)
        {
            local_position.face_direction = direction;
            // local_position = new Position(local_position.Center, local_position.halfsize, local_position.RotationAngle, local_position.Origin, direction);
        }

        public void SetLocalCenter(Vector2 center)
        {
            local_position.Center = center;
            // local_position = new Position(center, local_position.halfsize, local_position.RotationAngle, local_position.Origin, local_position.face_direction);            
        }
    }
}
