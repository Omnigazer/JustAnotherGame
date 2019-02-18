using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Scenes
{
    public interface ISubsystem
    {
        void Tick(float dt);
        void RegisterObject(GameObject obj);
        void UnregisterObject(GameObject obj);
    }
}
