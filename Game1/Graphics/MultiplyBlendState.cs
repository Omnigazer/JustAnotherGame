using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public class MultiplyBlendState : BlendState
    {
        public MultiplyBlendState() : base()
        {
            ColorBlendFunction = BlendFunction.Add;
            ColorSourceBlend = Blend.DestinationColor;
            ColorDestinationBlend = Blend.Zero;
            AlphaBlendFunction = BlendFunction.Min;
            // AlphaBlendFunction = BlendFunction.Max;
            AlphaSourceBlend = Blend.One;
            AlphaDestinationBlend = Blend.One;
        }
    }
}
