using Microsoft.Xna.Framework;
using Omniplatformer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components.Physics
{
    public struct Position
    {
        public static Vector2 DefaultOrigin => new Vector2(0.5f, 0.5f);

        // public Vector2 Coords { get; set; }
        // public Vector2 Center => Coords + Halfsize - 2 * Halfsize * Origin;
        public Vector2 Coords => Center - Halfsize + 2 * Halfsize * Origin;

        public Vector2 Center { get; set; }
        public Vector2 Halfsize { get; set; }

        // The rotation origin
        public Vector2 Origin { get; set; }

        public float RotationAngle { get; set; }
        public HorizontalDirection FaceDirection { get; set; }

        // used for from-position object initializers
        public Position(Position position)
        {
            // Coords = position.Coords;
            Center = position.Center;
            Halfsize = position.Halfsize;
            Origin = position.Origin;
            RotationAngle = position.RotationAngle;
            FaceDirection = position.FaceDirection;
        }

        public Position(Vector2 coords, Vector2 halfsize, float angle = 0, Vector2? origin = null, HorizontalDirection dir = HorizontalDirection.Right)
        {
            Origin = origin ?? Position.DefaultOrigin;
            Center = coords + halfsize - 2 * halfsize * Origin;
            this.Halfsize = halfsize;
            RotationAngle = angle;
            FaceDirection = dir;
        }

        public static Position operator *(Position a, Position b)
        {
            var dir_multiplier = (int)b.FaceDirection;
            var v = new Vector2(a.Coords.X * dir_multiplier, a.Coords.Y);
            return new Position(
                    Vector2.Transform(v, Matrix.CreateRotationZ(-b.RotationAngle) * Matrix.CreateTranslation(b.Coords.X, b.Coords.Y, 0)),
                    a.Halfsize,
                    dir_multiplier * a.RotationAngle + b.RotationAngle,
                    a.Origin,
                    (HorizontalDirection)((int)a.FaceDirection * (int)b.FaceDirection)
                );
            //
        }

        public override string ToString()
        {
            return $"x:{Center.X} y:{Center.Y} halfsize:{Halfsize.X} {Halfsize.Y}";
        }
    }
}
