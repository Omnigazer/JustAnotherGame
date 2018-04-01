using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;

namespace Omniplatformer.Components
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