using Microsoft.Xna.Framework.Graphics;

namespace Omniplatformer.Graphics
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
