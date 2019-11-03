using System;
using Microsoft.Xna.Framework;

namespace Omniplatformer.Utility.EventHandlers
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
