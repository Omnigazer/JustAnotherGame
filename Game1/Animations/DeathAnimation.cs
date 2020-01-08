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
    public class DeathAnimation : Animation
    {
        public override AnimationType AnimationType => AnimationType.Death;

        public DeathAnimation(AnimatedRenderComponent drawable) : base(drawable)
        {

        }

        public override void Tick(float dt)
        {
            base.Tick(dt);
            float alpha = (float)(Duration - CurrentTime) / Duration;
            Drawable.Opacity = alpha;
        }
    }
}
