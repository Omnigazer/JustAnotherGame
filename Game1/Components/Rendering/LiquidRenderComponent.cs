using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components.Physics;
using Omniplatformer.Content;
using Omniplatformer.Objects;
using Omniplatformer.Services;

namespace Omniplatformer.Components.Rendering
{
    public class LiquidRenderComponent : RenderComponent
    {
        public Color GlowColor { get; set; } = Color.Orange;
        public int Radius { get; set; } = 100;
        public LiquidRenderComponent() { }
        public LiquidRenderComponent(Color color, string texture = null, int z_index = 0) : base(color, texture, z_index) { z_index = 100; }

        public override void Draw() { }

        public override void DrawToBackground()
        {
            base.Draw();
        }
    }
}
