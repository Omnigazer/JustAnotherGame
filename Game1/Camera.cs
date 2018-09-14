using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public class Camera
    {
        public Camera()
        {
            Zoom = 1.0f;
        }

        Game1 Game => GameService.Instance;

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; private set; }

        public Rectangle GetRectangle()
        {
            return new Rectangle(Position.ToPoint() - new Point(ViewportWidth / 2, ViewportHeight / 2), new Point(ViewportWidth, ViewportHeight));
        }

        public int ViewportWidth => Game.graphics.PreferredBackBufferWidth;
        public int ViewportHeight => Game.graphics.PreferredBackBufferHeight;

        public Vector2 ViewportCenter => new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);

        public Matrix TranslationMatrix => Matrix.CreateTranslation(-(int)Position.X,
                                            (int)Position.Y, 0) *
                                            Matrix.CreateRotationZ(Rotation) *
                                            Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                            Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));

        public Matrix NonTranslationMatrix => Matrix.CreateRotationZ(Rotation) *
                                              Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));

        public void AdjustZoom(float value)
        {
            Zoom += value;
            Zoom = Math.Max(0.125f, Zoom);
            Zoom = Math.Min(2f, Zoom);
        }

        public void ResetZoom()
        {
            Zoom = 1.0f;
        }
    }
}
