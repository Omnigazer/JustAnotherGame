using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    class DeathManaBar : ManaBar
    {
        protected override ManaType ManaType => ManaType.Death;
        protected override Color Color => Color.DarkViolet;
    }
}
