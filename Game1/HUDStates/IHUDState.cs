using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUDStates
{
    public interface IHUDState
    {
        void Draw();
        void HandleControls();
    }
}
