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
        protected Animation CurrentAnimation { get; set; }
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
            if (CurrentAnimation != animation || interrupt)
            {
                current_animation_ticks = 0;
                current_animation_length = length;
                CurrentAnimation = animation;
            }            
        }

        public void EndAnimation()
        {
            onAnimationEnd(CurrentAnimation);
            CurrentAnimation = Animation.Default;
        }

        public event EventHandler<AnimationEventArgs> _onAnimationEnd = delegate { };
        public void onAnimationEnd(Animation animation)
        {
            _onAnimationEnd(this, new AnimationEventArgs(animation));
        }

        public override void Tick()
        {
            if (CurrentAnimation != Animation.Default && ++current_animation_ticks >= current_animation_length)
            {
                EndAnimation();
            }            
        }

        // TODO: get a state for this
        public override void Draw()
        {
            base.Draw();            
        }
    }
}
