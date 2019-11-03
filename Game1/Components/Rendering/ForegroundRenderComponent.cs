using Microsoft.Xna.Framework;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Rendering
{
    class ForegroundRenderComponent : RenderComponent
    {
        public ForegroundRenderComponent(GameObject obj) : base(obj)
        {
        }

        public ForegroundRenderComponent(GameObject obj, Color color) : base(obj, color)
        {
        }

        public ForegroundRenderComponent(GameObject obj, Color color, int z_index) : base(obj, color, z_index)
        {
        }

        public override void Draw()
        {

        }

        public override void DrawToForeground()
        {
            base.Draw();
        }
    }
}