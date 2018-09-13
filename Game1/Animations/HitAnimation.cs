using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Animations
{
    public class HitAnimation : Animation
    {
        public HitAnimation(AnimatedRenderComponent drawable) : base(drawable)
        {

        }

        public override void End()
        {
            Drawable.Color = Color.White;
            base.End();
        }

        public override void Tick(float time_scale)
        {
            base.Tick(time_scale);
            Drawable.Color = Color.Red;
        }
    }
}
