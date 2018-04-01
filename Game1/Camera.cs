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
            Zoom = 1.1f;
        }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; private set; }

        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }

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
            Zoom = Math.Max(0.25f, Zoom);
            Zoom = Math.Min(2f, Zoom);
        }
    }
}
