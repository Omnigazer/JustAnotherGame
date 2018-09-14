using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Animations
{
    /// <summary>
    /// Currently a dud
    /// </summary>
    public class CastAnimation : Animation
    {
        public override AnimationType AnimationType => AnimationType.Cast;

        public CastAnimation(AnimatedRenderComponent drawable) : base(drawable)
        {

        }
    }
}
