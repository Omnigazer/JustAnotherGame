using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Newtonsoft.Json;
using Omniplatformer.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Omniplatformer.Utility.JsonConverters;

namespace Omniplatformer.Animations
{
    public class SpritesheetAnimation : Animation
    {
        public int MaxFrames { get; set; } = 0;
        public int CurrentFrame { get; set; } = 0;
        public int Columns { get; set; } = 1;
        public int Rows => (int)Math.Ceiling(((decimal)MaxFrames / Columns));

        public SpritesheetAnimation(AnimatedRenderComponent drawable) : base(drawable)
        {
        }

        public Rectangle GetRect()
        {
            var frame_rect = Texture.Bounds;
            frame_rect.Location = new Point(
                frame_rect.Width / Columns * (CurrentFrame % Columns),
                frame_rect.Height / Rows * ((int)Math.Ceiling((float)(CurrentFrame / Columns)))
            );
            frame_rect.Size = new Point(
                frame_rect.Width / Columns,
                frame_rect.Height / Rows
            );
            return frame_rect;
        }

        protected override void ProcessFrames(float dt)
        {
            base.ProcessFrames(dt);
            CurrentFrame = (int)((CurrentTime / Duration) * MaxFrames);
            if (Active)
            {
                Drawable.Texture = Texture;
                Drawable.Rect = GetRect();
            }
        }
    }
}
