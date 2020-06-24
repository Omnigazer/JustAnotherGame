using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

        public List<GameObject> Garbage { get; set; } = new List<GameObject>();

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
        }

        public void UnregisterObject(GameObject obj)
        {
            Garbage.Add(obj);
        }

        public void ProcessSubsystems(float dt)
        {
            foreach (var system in Subsystems)
            {
                system.Tick(dt);
            }
            UpdateSystem.Tick(dt);
        }

        public void ProcessRemovals()
        {
            foreach(var obj in Garbage)
            {
                foreach (var system in Subsystems)
                {
                    system.UnregisterObject(obj);
                }

                obj.CurrentScene = null;
            }
            Garbage.Clear();
        }
    }
}
