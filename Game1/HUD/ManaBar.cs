using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    class ManaBar
    {
        // public Player Player { get; set; }
        public Player Player => GameService.Player;
        public Point Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Thickness { get; set; }

        protected int bar_loop = 0;
        protected int loop_period = 8;
        protected float distort_loop = 0;
        protected int distort_amp = 200; // merely a technical value
        protected float distort_speed = 0.25f;

        public ManaBar(Point position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
            Thickness = 5;
            distort_loop = new Random().Next(distort_amp);
        }

        void ContinueLoop()
        {
            bar_loop = (bar_loop + 1) % (400 * loop_period);
            distort_loop = (distort_loop + distort_speed) % distort_amp;
        }

        public virtual void Draw()
        {
            ContinueLoop();
        }

        public void ApplyDistort()
        {
            var distortEffect = GameContent.Instance.DistortEffect;
            distortEffect.Parameters["OffsetPower"].SetValue(0.05f);
            // float lower_angle = (float)(-Math.PI);
            float lower_angle = 0;
            float upper_angle = 2 * (float)(Math.PI);
            float amp = upper_angle - lower_angle;
            float angle = (float)(lower_angle + amp * ((float)distort_loop / distort_amp));
            distortEffect.Parameters["angle"].SetValue(angle);
            distortEffect.CurrentTechnique.Passes[0].Apply();
        }
    }
}
