using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;

namespace Omniplatformer.Animations
{
    public class HitAnimation : Animation
    {
        public override AnimationType AnimationType => AnimationType.Hit;

        public HitAnimation(AnimatedRenderComponent drawable) : base(drawable)
        {

        }

        public override void End()
        {
            Drawable.Color = Drawable.DefaultColor;
            base.End();
        }

        public override void Tick(float dt)
        {
            Drawable.Color = Color.Red;
            base.Tick(dt);
        }
    }
}
