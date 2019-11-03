using Microsoft.Xna.Framework;
using Omniplatformer.Enums;

namespace Omniplatformer.Views.HUD
{
    class ChaosManaBar : ManaBar
    {
        protected override ManaType ManaType => ManaType.Chaos;
        protected override Color Color => Color.FromNonPremultiplied(255, 80, 20, 255);
    }
}
