using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Scenes.Subsystems;

namespace Omniplatformer.Scenes
{
    public class Scene
    {
        public List<ISubsystem> Subsystems { get; set; } = new List<ISubsystem>();
        public RenderSystem RenderSystem { get; set; }
        public PhysicsSystem PhysicsSystem { get; set; }
        public UpdateSystem UpdateSystem { get; set; }
        public Player Player { get; set; }

        public Dictionary<string, List<GameObject>> Groups { get; set; } = new Dictionary<string, List<GameObject>>() { { "default", new List<GameObject>() } };

        public Scene()
        {
            UpdateSystem = new UpdateSystem();
        }

        public void RegisterObject(GameObject obj)
        {
            foreach (var system in Subsystems)
            {
                system.RegisterObject(obj);
            }
            obj.CurrentScene = this;
            obj._onDestroy += Obj__onDestroy;
        }

        private void Obj__onDestroy(object sender, EventArgs e)
        {
            var obj = (GameObject)sender;
            UnregisterObject(obj);
            obj._onDestroy -= Obj__onDestroy;
        }

        public void UnregisterObject(GameObject obj)
        {
            foreach (var system in Subsystems)
            {
                system.UnregisterObject(obj);
            }

            obj.CurrentScene = null;
        }

        public void ProcessSubsystems(float dt)
        {
            foreach (var system in Subsystems)
            {
                system.Tick(dt);
            }
            UpdateSystem.Tick(dt);
        }
    }
}
