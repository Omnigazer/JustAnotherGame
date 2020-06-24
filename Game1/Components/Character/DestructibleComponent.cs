using Omniplatformer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components.Character
{
    class DestructibleComponent : Component
    {
        public Subject<GameObject> OnDestroy = new Subject<GameObject>();

        public virtual void Destroy()
        {
            OnDestroy.OnNext(GameObject);
            OnDestroy.OnCompleted();
            GameObject.LeaveScene();
        }
    }
}
