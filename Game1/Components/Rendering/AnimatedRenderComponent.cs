using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Omniplatformer.Animations;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Utility.Extensions;

namespace Omniplatformer.Components.Rendering
{
    public class AnimatedRenderComponent : RenderComponent
    {
        [JsonProperty]
        private Dictionary<AnimationType, Animation> Animations { get; set; } = new Dictionary<AnimationType, Animation>();

        public AnimatedRenderComponent() { }
        public AnimatedRenderComponent(Color color, string texture = null, int z_index = 0) : base(color, texture, z_index) { }

        public void AddAnimation(Animation animation)
        {
            Animations.Add(animation.AnimationType, animation);
        }

        public override void Compile()
        {
            GameObject.OnLeaveScene.Subscribe((_) =>
            {
                onAnimationState.OnCompleted();
                onAnimationEnd.OnCompleted();
            });
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
        public Subject<(AnimationType type, AnimationState state)> onAnimationState = new Subject<(AnimationType, AnimationState)>();

        public Subject<AnimationType> onAnimationEnd = new Subject<AnimationType>();

        public override void Tick(float dt)
        {
            foreach (var (type, animation) in Animations.Where(kv => kv.Value.Active))
            {
                animation.Tick(dt);
            }
        }
    }
}
