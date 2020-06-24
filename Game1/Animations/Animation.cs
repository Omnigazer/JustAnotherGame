using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Newtonsoft.Json;

namespace Omniplatformer.Animations
{
    public abstract class Animation
    {
        public abstract AnimationType AnimationType { get; }
        public bool Active { get; private set; }
        [JsonProperty]
        protected float CurrentTime { get; set; }
        [JsonProperty]
        protected float Duration { get; set; }
        [JsonProperty]
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
            Drawable.onAnimationEnd.OnNext(AnimationType);
        }

        public virtual void Tick(float dt)
        {
            CurrentTime += dt;
            if (CurrentTime >= Duration)
                End();
        }
    }
}
