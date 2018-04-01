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
            if (CurrentAnimation == Animation.Death)
            {
                float alpha = (float)(current_animation_length - current_animation_ticks) / current_animation_length;
                base.Draw(alpha);
            }
            else
            {
                base.Draw();
            }            
        }
    }
}
