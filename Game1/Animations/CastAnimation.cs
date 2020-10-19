using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;

namespace Omniplatformer.Animations
{
    /// <summary>
    /// Currently a dud
    /// </summary>
    public class CastAnimation : Animation
    {
        public CastAnimation(AnimatedRenderComponent drawable) : base(drawable)
        {
            AnimationType = AnimationType.Cast;
        }
    }
}
