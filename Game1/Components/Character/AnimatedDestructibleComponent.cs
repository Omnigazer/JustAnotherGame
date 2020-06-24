using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components.Character
{
    class AnimatedDestructibleComponent : DestructibleComponent
    {
        public int AnimationLength { get; set; }

        public override void Destroy()
        {
            var drawable = GetComponent<AnimatedRenderComponent>();
            if (drawable != null)
            {
                drawable.onAnimationEnd.Where((animation_type) => animation_type == AnimationType.Death)
                                        .Take(1).Subscribe((_) => GameObject.LeaveScene());
                drawable.StartAnimation(AnimationType.Death, AnimationLength);
            }
        }
    }
}
