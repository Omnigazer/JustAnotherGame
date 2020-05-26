using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Rendering
{
    class ForegroundRenderComponent : RenderComponent
    {
        public ForegroundRenderComponent() { }
        public ForegroundRenderComponent(Color color, string texture = null, int z_index = 0) : base(color, texture, z_index) { }

        public override void Draw()
        {

        }

        public override void DrawToForeground()
        {
            base.Draw();
        }
    }
}