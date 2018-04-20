using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUDStates
{
    public abstract class HUDState
    {
        // tracks key press/release
        public static Dictionary<Keys, bool> release_map = new Dictionary<Keys, bool>();
        public static bool lmb_pressed;
        public static bool rmb_pressed;
        public static Point click_pos;
        public virtual void Draw() { }
        public virtual void HandleControls() { }

    }
}
