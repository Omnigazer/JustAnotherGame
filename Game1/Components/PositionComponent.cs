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
        public static Vector2 DefaultOrigin => new Vector2(0.5f, 0.5f);
        public Vector2 Coords { get; set; }
        public Vector2 Center => Coords + halfsize - 2 * halfsize * Origin;
        // public Vector2 Center => Coords;
        public Vector2 halfsize;

        // The rotation origin
        public Vector2 Origin { get; set; }
        public float RotationAngle { get; set; }
        public HorizontalDirection face_direction;

        // used for from-position object initializers
        public Position(Position position)
        {
            Coords = position.Coords;
            halfsize = position.halfsize;
            Origin = position.Origin;
            RotationAngle = position.RotationAngle;
            face_direction = position.face_direction;
        }

        public Position(Vector2 coords, Vector2 halfsize, float angle = 0, HorizontalDirection dir = HorizontalDirection.Right)
        {
            Coords = coords;
            this.halfsize = halfsize;
            RotationAngle = angle;
            face_direction = dir;
            Origin = Position.DefaultOrigin;
            // Origin = new Vector2(GameService.Instance.origin, GameService.Instance.origin);
            // Origin = Vector2.Zero;
        }

        public Position(Vector2 coords, Vector2 halfsize, float angle, Vector2 origin, HorizontalDirection dir = HorizontalDirection.Right)
        {
            Coords = coords;
            this.halfsize = halfsize;
            RotationAngle = angle;
            face_direction = dir;
            Origin = origin;
        }

        public static Position operator *(Position a, Position b)
        {
            var dir_multiplier = (int)b.face_direction;
            var v = new Vector2(a.Coords.X * dir_multiplier, a.Coords.Y);
            return new Position(
                    Vector2.Transform(v, Matrix.CreateRotationZ(-b.RotationAngle) * Matrix.CreateTranslation(b.Coords.X, b.Coords.Y, 0)),
                    a.halfsize,
                    dir_multiplier * a.RotationAngle + b.RotationAngle,
                    a.Origin,
                    (HorizontalDirection)((int)a.face_direction * (int)b.face_direction)
                );
            //
        }

        public override string ToString()
        {
            return $"x:{Coords.X} y:{Coords.Y} halfsize:{halfsize.X} {halfsize.Y}";
        }
    }

    public abstract class PositionComponent : Component
    {
        // TODO: TEST
        public PositionComponent parent_pos;
        public AnchorPoint parent_anchor = AnchorPoint.Default;

        public Position local_position;
        public Position WorldPosition => parent_pos != null ? local_position * parent_pos.GetAnchor(parent_anchor) : local_position;

        public Dictionary<AnchorPoint, Position> DefaultAnchors { get; set; } = new Dictionary<AnchorPoint, Position>();
        public Dictionary<AnchorPoint, Position> CurrentAnchors { get; set; } = new Dictionary<AnchorPoint, Position>();

        public PositionComponent(GameObject obj, Vector2 coords, Vector2 halfsize) : base(obj)
        {
            local_position = new Position(coords, halfsize);
        }

        public PositionComponent(GameObject obj, Vector2 coords, Vector2 halfsize, float angle) : base(obj)
        {
            local_position = new Position(coords, halfsize, angle);
        }

        public PositionComponent(GameObject obj, Vector2 coords, Vector2 halfsize, float angle, Vector2 origin) : base(obj)
        {
            local_position = new Position(coords, halfsize, angle, origin);
        }

        public void AddAnchor(AnchorPoint anchor_name, Position position)
        {
            DefaultAnchors.Add(anchor_name, position);
            CurrentAnchors.Add(anchor_name, position);
        }

        public Position GetAnchor(AnchorPoint anchor_name)
        {
            switch (anchor_name)
            {
                // TODO: test
                case AnchorPoint.Hand:
                    {
                        var clamped_position = CurrentAnchors[AnchorPoint.Hand];
                        var real_position = new Position(clamped_position) { Coords = new Vector2(2 * clamped_position.Center.X * local_position.halfsize.X, 2 * clamped_position.Center.Y * WorldPosition.halfsize.Y) };
                        return real_position * WorldPosition;
                        // return real_position;
                    }
                default:
                    {
                        return WorldPosition;
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

        public void AdjustPosition(Vector2 displacement)
        {
            local_position.Coords += displacement;
        }

        public Rectangle GetRectangle()
        {
            var halfsize = WorldPosition.halfsize;
            var zero = WorldPosition.Center - WorldPosition.halfsize;
            // zero = new Vector2((int)Math.Round(zero.X, 0, MidpointRounding.ToEven), (int)Math.Round(zero.Y, 0, MidpointRounding.ToEven));
            return new Rectangle((zero).ToPoint(), new Point((int)halfsize.X * 2, (int)halfsize.Y * 2));
            // return new Rectangle((zero + new Vector2(WorldPosition.Origin.X * pt.X, WorldPosition.Origin.Y * pt.Y)).ToPoint(), pt);
        }

        public IEnumerable<Vector2> GetRectPoints()
        {
            return new List<Vector2>() {
                WorldPosition.Center - WorldPosition.halfsize,
                WorldPosition.Center + WorldPosition.halfsize,
                WorldPosition.Center - new Vector2(WorldPosition.halfsize.X, -WorldPosition.halfsize.Y),
                WorldPosition.Center + new Vector2(WorldPosition.halfsize.X, -WorldPosition.halfsize.Y)
            };
        }

        public bool Contains(Vector2 pt)
        {
            if ((pt.X >= WorldPosition.Center.X - WorldPosition.halfsize.X && pt.X <= WorldPosition.Center.X + WorldPosition.halfsize.X) && (pt.Y >= WorldPosition.Center.Y - WorldPosition.halfsize.Y && pt.Y <= WorldPosition.Center.Y + WorldPosition.halfsize.Y))
                return true;
            return false;
        }

        public bool Overlaps(PositionComponent pos)
        {
            // PositionComponent pos = (PositionComponent)pos;
            if (pos == null)
                return false;

            if (Math.Abs(WorldPosition.Center.X - pos.WorldPosition.Center.X) > WorldPosition.halfsize.X + pos.WorldPosition.halfsize.X) return false;
            if (Math.Abs(WorldPosition.Center.Y - pos.WorldPosition.Center.Y) > WorldPosition.halfsize.Y + pos.WorldPosition.halfsize.Y) return false;
            return true;
        }

        public Direction Collides(PositionComponent other)
        {
            if (other == null)
                return Direction.None;

            if (Overlaps(other))
            {
                var hd = Math.Abs(WorldPosition.Center.X - other.WorldPosition.Center.X) - (WorldPosition.halfsize.X + other.WorldPosition.halfsize.X);
                var vd = Math.Abs(WorldPosition.Center.Y - other.WorldPosition.Center.Y) - (WorldPosition.halfsize.Y + other.WorldPosition.halfsize.Y);

                // Now compare them to know the side of collision
                if (hd > vd && (Math.Abs(vd) > WorldPosition.halfsize.Y || WorldPosition.Center.Y < other.WorldPosition.Center.Y))
                {
                    if (WorldPosition.Center.X < other.WorldPosition.Center.X)
                        return Direction.Right;
                    else
                    {
                        return Direction.Left;
                    }
                }
                else if (vd > hd)
                {
                    if (WorldPosition.Center.Y < other.WorldPosition.Center.Y)
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

        // TODO: properly implement this
        public GameObject GetClosestObject(Vector2 direction, Func<PhysicsComponent, bool> predicate = null)
        {
            // TODO: count the range from the anchor
            // TODO: this doesn't work properly with angled directions, should be raycast or something

            // rect from vector
            Rectangle rect = new Rectangle(WorldPosition.Center.ToPoint(), new Vector2(Math.Abs(direction.X), Math.Abs(direction.Y)).ToPoint());
            rect.Offset(Math.Min(direction.X, 0), Math.Min(direction.Y, 0));

            // IEnumerable<GameObject> list = GameService.Characters.Union(GameService.Objects).Where(x => x.Hittable && rect.Intersects(((PositionComponent)x).GetRectangle()));

            IEnumerable<PhysicsComponent> list = GameService.Instance.PhysicsSystem.objects.Where(x => x.Hittable && rect.Intersects(x.GetRectangle()));
            if (predicate != null)
                list = list.Where(predicate);
            return list.FirstOrDefault()?.GameObject;
        }

        public void Rotate(float angle)
        {
            local_position.RotationAngle += angle;
        }

        public void SetLocalFace(HorizontalDirection direction)
        {
            local_position.face_direction = direction;
        }

        public void SetLocalCenter(Vector2 center)
        {
            var diff = local_position.Center - local_position.Coords;
            SetLocalCoords(center - diff);
        }

        public void SetLocalCoords(Vector2 coords)
        {
            local_position.Coords = coords;
        }

        public void SetLocalHalfsize(Vector2 halfsize)
        {
            local_position.halfsize = halfsize;
        }

        public void SetParent(GameObject obj, AnchorPoint anchor = AnchorPoint.Default)
        {
            parent_pos = (PositionComponent)obj;
            parent_anchor = anchor;
            SetLocalCoords(Vector2.Zero);
        }

        public void ClearParent()
        {
            parent_pos = null;
        }

        public override string ToString()
        {
            return WorldPosition.ToString();
        }
    }
}
