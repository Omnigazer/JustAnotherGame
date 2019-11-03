using Omniplatformer.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;

namespace Omniplatformer.Components
{
    public abstract class Component
    {
        public GameObject GameObject { get; set; }
        public Scene Scene => GameObject.CurrentScene;
        public string Tag { get; set; }
        public Component(GameObject obj)
        {
            GameObject = obj;
        }

        public T GetComponent<T>() where T : Component
        {
            return GameObject.GetComponent<T>();
        }

        public virtual void Tick(float dt) { }
    }
}
