using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    class ChaosManaBar : ManaBar
    {
        protected override ManaType ManaType => ManaType.Chaos;
        protected override Color Color => Color.FromNonPremultiplied(255, 80, 20, 255);
    }
}
