using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Animations
{
    public abstract class Animation
    {
        public abstract AnimationType AnimationType { get; }
        public bool Active { get; private set; }
        protected float CurrentTime { get; set; }
        protected float Duration { get; set; }
        protected AnimatedRenderComponent Drawable { get; set; }

        public Animation(AnimatedRenderComponent drawable)
        {
            Drawable = drawable;
        }

        public virtual void Start(float duration)
        {
            Duration = duration;
            CurrentTime = 0;
            Active = true;
        }

        public virtual void End()
        {
            Active = false;
            Drawable.onAnimationEnd(AnimationType);
        }

        public virtual void Tick(float time_scale)
        {
            CurrentTime += time_scale;
            if (CurrentTime >= Duration)
                End();
        }
    }
}
