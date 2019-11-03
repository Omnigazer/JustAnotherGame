using Microsoft.Xna.Framework.Graphics;

namespace Omniplatformer.Graphics
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
