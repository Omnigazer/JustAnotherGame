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

        }

        public override void SetupNode()
        {
            var (width, height) = GameService.Instance.RenderSystem.GetResolution();
            Width = width;
            Height = height;
            Node.Padding = 20;
            Node.FlexDirection = Facebook.Yoga.YogaFlexDirection.Row;
            Node.AlignItems = Facebook.Yoga.YogaAlign.FlexStart;
        }

        public override void DrawSelf()
        {
            if (Node.IsDirty)
                Node.CalculateLayout();
        }
    }
}
