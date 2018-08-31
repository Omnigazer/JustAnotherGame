using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        }

        public WallRenderComponent(GameObject obj, Color color) : base(obj, color)
        {
        }

        public override void Draw()
        {
            if (CurrentAnimations.ContainsKey(Animation.Death))
            {
                var (ticks, length, current_step) = CurrentAnimations[Animation.Death];
                float alpha = (float)(length - ticks) / length;
                base.Draw(alpha);
            }
            else
            {
                base.Draw();
            }
        }
    }
}
