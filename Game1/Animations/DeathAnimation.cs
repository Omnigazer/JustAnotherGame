using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Animations
{
    public class DeathAnimation : Animation
    {
        public override AnimationType AnimationType => AnimationType.Death;

        public DeathAnimation(AnimatedRenderComponent drawable) : base(drawable)
        {

        }

        public override void End()
        {
            base.End();
            // TODO: just in case, watch this
            Drawable.Opacity = 1;
        }

        public override void Tick(float dt)
        {
            base.Tick(dt);
            float alpha = (float)(Duration - CurrentTime) / Duration;
            Drawable.Opacity = alpha;
        }
    }
}
