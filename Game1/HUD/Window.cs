using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    // Placeholder class for root control
    public class Root : ViewControl
    {
        public Root()
        {
            var (width, height) = GameService.Instance.RenderSystem.GetResolution();
            Width = width;
            Height = height;
        }
    }
}
