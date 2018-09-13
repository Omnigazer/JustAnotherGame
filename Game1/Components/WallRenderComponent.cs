using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    class WallRenderComponent : AnimatedRenderComponent
    {
        public WallRenderComponent(GameObject obj) : base(obj)
        {
            Animations.Add(AnimationType.Death, new DeathAnimation(this));
        }

        public WallRenderComponent(GameObject obj, Color color) : base(obj, color)
        {
        }
    }
}
