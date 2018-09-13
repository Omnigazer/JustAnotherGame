using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Omniplatformer.Components
{
    class AnimatedRenderComponent : RenderComponent
    {
        // protected Animation CurrentAnimation { get; set; }
        /// <summary>
        /// Maps animation type to the tuple (current ticks, duration, current step)
        /// </summary>
        protected Dictionary<Animation, (float, float, int)> CurrentAnimations { get; set; } = new Dictionary<Animation, (float, float, int)>();

        public class AnimationEventArgs : EventArgs
        {
            public Animation animation;
            public AnimationEventArgs(Animation animation)
            {
                this.animation = animation;
            }
        }

        public AnimatedRenderComponent(GameObject obj) : base(obj)
        {

        }

        public AnimatedRenderComponent(GameObject obj, Color color) : base(obj, color)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="length" description="length in ticks"></param>
        public void StartAnimation(Animation animation, float length, bool interrupt = false)
        {
            // TODO: implement interruption logic here
            if (CurrentAnimations.ContainsKey(animation))
            {
                if (interrupt)
                    CurrentAnimations[animation] = (0, length, 0);
            }
            else
                CurrentAnimations.Add(animation, (0, length, 0));
        }

        public void EndAnimation(Animation animation)
        {
            CurrentAnimations.Remove(animation);
            PositionComponent pos = GetComponent<PositionComponent>();
            pos.ResetAnchors();
            onAnimationEnd(animation);
            // CurrentAnimation = Animation.Default;
        }

        // TODO: extract this
        public event EventHandler<AnimationEventArgs> _onAnimationEnd = delegate { };
        public void onAnimationEnd(Animation animation)
        {
            _onAnimationEnd(this, new AnimationEventArgs(animation));
        }

        public event EventHandler<AnimationEventArgs> _onAnimationHit = delegate { };
        public void onAnimationHit(Animation animation)
        {
            _onAnimationHit(this, new AnimationEventArgs(animation));
        }

        public override void Tick(float time_scale)
        {
            foreach (var (animation, (ticks, length, current_step)) in CurrentAnimations.ToList())
            {
                CurrentAnimations[animation] = (ticks + time_scale, length, current_step);
                if (ticks + time_scale >= length)
                {
                    EndAnimation(animation);
                }
            }
            /*
            if (CurrentAnimation != Animation.Default && ++current_animation_ticks >= current_animation_length)
            {
                EndAnimation();
            }
            */
        }

        // TODO: get a state for this
        public override void Draw()
        {
            base.Draw();
        }
    }
}
