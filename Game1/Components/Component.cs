using Omniplatformer.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;
using Newtonsoft.Json;

namespace Omniplatformer.Components
{
    [Serializable]
    public abstract class Component
    {
        public GameObject GameObject { get; set; }

        [JsonIgnore]
        public Scene Scene => GameObject.CurrentScene;

        public T GetComponent<T>() where T : Component
        {
            return GameObject.GetComponent<T>();
        }

        public virtual void Tick(float dt) { }
        public virtual void Compile() { }
    }
}
