using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Utility.Extensions;

namespace Omniplatformer.Components.Physics
{
    public abstract class PositionComponent : Component
    {
        // TODO: TEST
        [JsonProperty]
        protected PositionComponent parent_pos;
        [JsonProperty]
        protected AnchorPoint parent_anchor = AnchorPoint.Default;
        [JsonProperty]
        protected Position local_position;

        [JsonIgnore]
        public Position WorldPosition => parent_pos != null ? local_position * parent_pos.GetAnchor(parent_anchor) : local_position;
        public Dictionary<AnchorPoint, Position> DefaultAnchors { get; set; } = new Dictionary<AnchorPoint, Position>();
        public Dictionary<AnchorPoint, Position> CurrentAnchors { get; set; } = new Dictionary<AnchorPoint, Position>();

        public PositionComponent() {
            local_position = new Position { Origin = Position.DefaultOrigin };
        }

        protected PositionComponent(Vector2 coords, Vector2 halfsize, float angle = 0, Vector2? origin = null)
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
            if (CurrentAnchors.ContainsKey(anchor_name))
            {
                var clamped_position = CurrentAnchors[anchor_name];
                var real_position = new Position(clamped_position)
                {
                    Coords = new Vector2(
                        2 * clamped_position.Center.X * local_position.Halfsize.X,
                        2 * clamped_position.Center.Y * local_position.Halfsize.Y
                        )
                };
                return real_position * WorldPosition;
            }

            return WorldPosition;
        }

        public void ResetAnchors()
        {
            foreach (var (name, _) in DefaultAnchors)
            {
                ResetAnchor(name);
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
            var halfsize = WorldPosition.Halfsize;
            var zero = WorldPosition.Center - WorldPosition.Halfsize;
            return new Rectangle((zero).ToPoint(), new Point((int)halfsize.X * 2, (int)halfsize.Y * 2));
        }

        public IEnumerable<Vector2> GetRectPoints()
        {
            return new List<Vector2>() {
                WorldPosition.Center - WorldPosition.Halfsize,
                WorldPosition.Center + WorldPosition.Halfsize,
                WorldPosition.Center - new Vector2(WorldPosition.Halfsize.X, -WorldPosition.Halfsize.Y),
                WorldPosition.Center + new Vector2(WorldPosition.Halfsize.X, -WorldPosition.Halfsize.Y)
            };
        }

        public bool Contains(Vector2 pt)
        {
            if ((pt.X >= WorldPosition.Center.X - WorldPosition.Halfsize.X && pt.X <= WorldPosition.Center.X + WorldPosition.Halfsize.X) && (pt.Y >= WorldPosition.Center.Y - WorldPosition.Halfsize.Y && pt.Y <= WorldPosition.Center.Y + WorldPosition.Halfsize.Y))
                return true;
            return false;
        }

        public bool Overlaps(Position pos)
        {
            if (Math.Abs(WorldPosition.Center.X - pos.Center.X) > WorldPosition.Halfsize.X + pos.Halfsize.X) return false;
            if (Math.Abs(WorldPosition.Center.Y - pos.Center.Y) > WorldPosition.Halfsize.Y + pos.Halfsize.Y) return false;
            return true;
        }

        public Direction Collides(Position other)
        {
            var hd = Math.Abs(WorldPosition.Center.X - other.Center.X) - (WorldPosition.Halfsize.X + other.Halfsize.X);
            var vd = Math.Abs(WorldPosition.Center.Y - other.Center.Y) - (WorldPosition.Halfsize.Y + other.Halfsize.Y);

            if (hd > 0 || vd > 0)
                return Direction.None;

            // Now compare them to know the side of collision
            if (hd > vd && (Math.Abs(vd) > WorldPosition.Halfsize.Y || WorldPosition.Center.Y < other.Center.Y))
            {
                if (WorldPosition.Center.X < other.Center.X)
                    return Direction.Right;
                else
                    return Direction.Left;
            }
            else if (vd > hd)
            {
                if (WorldPosition.Center.Y < other.Center.Y)
                    return Direction.Up;
                else
                    return Direction.Down;
            }

            return Direction.None;
        }

        public (float x, float y) GetIntersection(PositionComponent their_pos)
        {
            // PositionComponent their_pos = (PositionComponent)obj;
            if (their_pos == null)
                return (0, 0);

            var my_rect = GetRectangle();
            var their_rect = their_pos.GetRectangle();
            float intersection_width = Math.Min(my_rect.Right, their_rect.Right) - Math.Max(my_rect.Left, their_rect.Left);
            float intersection_height = Math.Min(my_rect.Bottom, their_rect.Bottom) - Math.Max(my_rect.Top, their_rect.Top);
            return (intersection_width, intersection_height);
        }

        public float GetIntersectionArea(PositionComponent their_pos)
        {
            var (x, y) = GetIntersection(their_pos);
            return x * y;
        }

        public float GetImmersionShare(PositionComponent pos)
        {
            var my_rect = GetRectangle();
            float my_area = (float)(my_rect.Width * my_rect.Height);
            return GetIntersectionArea(pos) / my_area;
        }

        // TODO: properly implement this
        public GameObject GetClosestObject(Vector2 direction, Func<PhysicsComponent, bool> predicate = null)
        {
            // TODO: count the range from the anchor
            // TODO: this doesn't work properly with angled directions, should be raycast or something

            // rect from vector
            Rectangle rect = new Rectangle(WorldPosition.Center.ToPoint() - new Point(0, (int)WorldPosition.Halfsize.Y), new Vector2(Math.Abs(direction.X), Math.Abs(direction.Y)).ToPoint());
            rect.Offset(Math.Min(direction.X, 0), Math.Min(direction.Y, 0));

            // IEnumerable<GameObject> list = GameService.Characters.Union(GameService.Objects).Where(x => x.Hittable && rect.Intersects(((PositionComponent)x).GetRectangle()));

            IEnumerable<PhysicsComponent> list = Scene.PhysicsSystem.objects.Where(x => x.Hittable && rect.Intersects(x.GetRectangle()));
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
            local_position.FaceDirection = direction;
        }

        public void SetLocalCenter(Vector2 center)
        {
            var diff = local_position.Center - local_position.Coords;
            SetLocalCoords(center - diff);
        }

        public void SetWorldCenter(Vector2 center)
        {
            var diff = local_position.Center - local_position.Coords + (parent_pos?.WorldPosition.Coords ?? Vector2.Zero);
            SetLocalCoords(center - diff);
        }

        public void SetLocalCoords(Vector2 coords)
        {
            local_position.Coords = coords;
        }

        public void SetLocalHalfsize(Vector2 halfsize)
        {
            local_position.Halfsize = halfsize;
        }

        public void SetLocalOrigin(Vector2 origin)
        {
            local_position.Origin = origin;
        }

        public void SetParent(GameObject obj, AnchorPoint anchor = AnchorPoint.Default)
        {
            parent_pos = (PositionComponent)obj;
            parent_anchor = anchor;
            // SetLocalCoords(Vector2.Zero);
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
