﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    class NatureManaBar : ManaBar
    {
        protected override ManaType ManaType => ManaType.Nature;
        protected override Color Color => Color.OliveDrab;
    }
}
