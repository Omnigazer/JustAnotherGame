using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Omniplatformer.Animations;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Utility.Extensions;

namespace Omniplatformer.Components.Rendering
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
        [JsonProperty]
        private Dictionary<AnimationType, Animation> Animations { get; set; } = new Dictionary<AnimationType, Animation>();

        public AnimatedRenderComponent() { }
        public AnimatedRenderComponent(GameObject obj) : base(obj) { }
        public AnimatedRenderComponent(GameObject obj, Color color, string texture = null, int z_index = 0) : base(obj, color, texture, z_index) { }

        public void AddAnimation(Animation animation)
        {
            Animations.Add(animation.AnimationType, animation);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="length" description="length in ticks"></param>
        public void StartAnimation(AnimationType animation, float length)
        {
            Animations[animation].Start(length);
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
        }
    }
}
