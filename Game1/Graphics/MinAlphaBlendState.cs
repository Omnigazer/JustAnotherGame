using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public class MinAlphaBlendState : BlendState
    {
        public MinAlphaBlendState() : base()
        {
            AlphaBlendFunction = BlendFunction.Min;
            ColorBlendFunction = BlendFunction.Max;
            AlphaSourceBlend = Blend.One;
            AlphaDestinationBlend = Blend.One;
        }
    }
}
