using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    class LifeManaBar : ManaBar
    {
        protected override ManaType ManaType => ManaType.Life;
        protected override Color Color => Color.Azure;
    }
}
