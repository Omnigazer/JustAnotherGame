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
        /// 
        /// </summary>
        protected Dictionary<Animation, (int, int)> CurrentAnimations { get; set; } = new Dictionary<Animation, (int, int)>();

        protected int current_animation_ticks = 0;
        protected int current_animation_length = 10;

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
        public void StartAnimation(Animation animation, int length, bool interrupt = false)
        {
            /*
            if (CurrentAnimation != animation || interrupt)
            {
                current_animation_ticks = 0;
                current_animation_length = length;
                CurrentAnimation = animation;
            } 
            */

            // TODO: implement interruption logic here
            if (CurrentAnimations.ContainsKey(animation))
            {
                if (interrupt)
                    CurrentAnimations[animation] = (0, length);
            }
            else
                CurrentAnimations.Add(animation, (0, length));
        }

        public void EndAnimation(Animation animation)
        {
            CurrentAnimations.Remove(animation);
            PositionComponent pos = GetComponent<PositionComponent>();
            pos.ResetAnchors();
            onAnimationEnd(animation);
            // CurrentAnimation = Animation.Default;
        }

        public event EventHandler<AnimationEventArgs> _onAnimationEnd = delegate { };
        public void onAnimationEnd(Animation animation)
        {
            _onAnimationEnd(this, new AnimationEventArgs(animation));
        }

        public override void Tick()
        {            
            foreach (var (animation, (ticks, length)) in CurrentAnimations.ToList())            
            {
                CurrentAnimations[animation] = (ticks + 1, length);
                if (ticks + 1 >= length)
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
