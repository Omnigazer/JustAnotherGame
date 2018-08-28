using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public abstract class Component
    {
        protected GameObject GameObject { get; set; }
        public string Tag { get; set; }
        public Component(GameObject obj)
        {
            GameObject = obj;
        }

        public T GetComponent<T>() where T : Component
        {
            return GameObject.GetComponent<T>();
        }

        public virtual void Tick(float time_scale) { }
    }
}
