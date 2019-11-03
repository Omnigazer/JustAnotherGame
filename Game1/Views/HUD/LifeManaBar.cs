using Microsoft.Xna.Framework;
using Omniplatformer.Enums;

namespace Omniplatformer.Views.HUD
{
    class LifeManaBar : ManaBar
    {
        protected override ManaType ManaType => ManaType.Life;
        protected override Color Color => Color.Azure;
    }
}
