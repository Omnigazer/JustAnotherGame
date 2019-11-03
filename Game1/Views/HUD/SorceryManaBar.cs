using Microsoft.Xna.Framework;
using Omniplatformer.Enums;

namespace Omniplatformer.Views.HUD
{
    class SorceryManaBar : ManaBar
    {
        protected override ManaType ManaType => ManaType.Sorcery;
        protected override Color Color => Color.Aqua;
        protected override bool HasCaustics => true;
    }
}
