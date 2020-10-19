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
    public class Animation
    {
        public AnimationType AnimationType { get; set; }
        public bool Active { get; private set; }
        public LoopMode Mode { get; set; } = LoopMode.Once;

        [JsonProperty]
        protected float CurrentTime { get; set; }

        [JsonProperty]
        protected float Duration { get; set; }

        [JsonProperty]
        protected AnimatedRenderComponent Drawable { get; set; }

        [JsonProperty, JsonConverter(typeof(TextureConverter))]
        public Texture2D Texture { get; set; }

        public Animation(AnimatedRenderComponent drawable)
        {
            Drawable = drawable;
        }

        public virtual void Start(float duration)
        {
            Duration = duration;
            CurrentTime = 0;
            Active = true;
            loop_direction = 1;
        }

        public virtual void End()
        {
            Active = false;
            Drawable.onAnimationEnd.OnNext(AnimationType);
        }

        int loop_direction = 1;

        protected virtual void ProcessFrames(float dt)
        {
            CurrentTime += dt * loop_direction;
            // CurrentFrame = CurrentFrame + loop_direction;
            if (CurrentTime >= Duration)
                switch (Mode)
                {
                    case LoopMode.Once:
                        {
                            // CurrentFrame = 0;
                            CurrentTime = 0;
                            End();
                            break;
                        }
                    case LoopMode.ClampForever:
                        {
                            // CurrentFrame--;
                            CurrentTime -= dt;
                            End();
                            break;
                        }
                    case LoopMode.Loop:
                        {
                            CurrentTime = 0;
                            // CurrentFrame = 0;
                            break;
                        }
                    case LoopMode.PingPong:
                    case LoopMode.PingPongOnce:
                        {
                            CurrentTime -= dt;
                            // CurrentFrame = MaxFrames - 1;
                            loop_direction = -1;
                            break;
                        }
                }
            if (CurrentTime <= 0)
            {
                if (Mode == LoopMode.PingPong)
                    loop_direction = 1;
                if (Mode == LoopMode.PingPongOnce)
                    End();
            }
        }

        public virtual void Tick(float dt)
        {
            ProcessFrames(dt);
        }
    }
}
