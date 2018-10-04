using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omniplatformer.Animations;

namespace Omniplatformer.Components
{
    public class AnimationEventArgs : EventArgs
    {
        public AnimationType animation;
        public AnimationEventArgs(AnimationType animation)
        {
            this.animation = animation;
        }
    }

    public class AnimatedRenderComponent : RenderComponent
    {
        // protected Animation CurrentAnimation { get; set; }
        /// <summary>
        /// Maps animation type to the tuple (current ticks, duration, current step)
        /// </summary>
        // protected Dictionary<AnimationType, (float, float, int)> CurrentAnimations { get; set; } = new Dictionary<AnimationType, (float, float, int)>();
        private Dictionary<AnimationType, Animation> Animations { get; set; } = new Dictionary<AnimationType, Animation>();

        public AnimatedRenderComponent(GameObject obj) : base(obj)
        {

        }

        public AnimatedRenderComponent(GameObject obj, Color color) : base(obj, color)
        {
        }

        public void AddAnimation(Animation animation)
        {
            Animations.Add(animation.AnimationType, animation);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="length" description="length in ticks"></param>
        public void StartAnimation(AnimationType animation, float length, bool interrupt = false)
        {
            /*
            // TODO: implement interruption logic here
            if (CurrentAnimations.ContainsKey(animation))
            {
                if (interrupt)
                    CurrentAnimations[animation] = (0, length, 0);
            }
            else
                CurrentAnimations.Add(animation, (0, length, 0));
            */
            Animations[animation].Start(length);
        }

        public void EndAnimation(AnimationType animation)
        {
            Animations[animation].End();
            onAnimationEnd(animation);
            /*
            CurrentAnimations.Remove(animation);
            PositionComponent pos = GetComponent<PositionComponent>();
            pos.ResetAnchors();
            onAnimationEnd(animation);
            // CurrentAnimation = Animation.Default;
            */
        }

        // TODO: extract this
        public event EventHandler<AnimationEventArgs> _onAnimationEnd = delegate { };
        public void onAnimationEnd(AnimationType animation)
        {
            _onAnimationEnd(this, new AnimationEventArgs(animation));
        }

        public event EventHandler<AnimationEventArgs> _onAnimationHit = delegate { };
        public void onAnimationHit(AnimationType animation)
        {
            _onAnimationHit(this, new AnimationEventArgs(animation));
        }

        public override void Tick(float dt)
        {
            foreach (var (type, animation) in Animations.Where(kv => kv.Value.Active))
            {
                animation.Tick(dt);
            }
            /*
            foreach (var (animation, (ticks, length, current_step)) in CurrentAnimations.ToList())
            {
                CurrentAnimations[animation] = (ticks + dt, length, current_step);
                if (ticks + dt >= length)
                {
                    EndAnimation(animation);
                }
            }
            */
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
