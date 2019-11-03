using Microsoft.Xna.Framework;
using Omniplatformer.Enums;

namespace Omniplatformer.Views.HUD
{
    class DeathManaBar : ManaBar
    {
        protected override ManaType ManaType => ManaType.Death;
        protected override Color Color => Color.DarkViolet;
    }
}
