using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Omniplatformer
{
    public delegate void MoveEventHandler(object sender, MoveEventArgs args);

    public class MoveEventArgs : EventArgs
    {
        public MoveEventArgs(Vector2 v)
        {
            this.displacement = v;
        }
        public Vector2 displacement;
    }
}
