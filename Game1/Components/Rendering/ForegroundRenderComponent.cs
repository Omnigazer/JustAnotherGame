using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Rendering
{
    class ForegroundRenderComponent : RenderComponent
    {
        public ForegroundRenderComponent(GameObject obj) : base(obj) { }
        public ForegroundRenderComponent(GameObject obj, Color color, Texture2D texture = null, int z_index = 0) : base(obj, color, texture, z_index) { }

        public override void Draw()
        {

        }

        public override void DrawToForeground()
        {
            base.Draw();
        }
    }
}