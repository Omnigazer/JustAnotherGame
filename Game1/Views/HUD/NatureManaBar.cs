using Microsoft.Xna.Framework;
using Omniplatformer.Enums;

namespace Omniplatformer.Views.HUD
{
    class NatureManaBar : ManaBar
    {
        protected override ManaType ManaType => ManaType.Nature;
        protected override Color Color => Color.OliveDrab;
    }
}
