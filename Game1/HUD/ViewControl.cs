using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    public abstract class ViewControl
    {
        public Point Position { get; set; }
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract void Draw();
    }
}
