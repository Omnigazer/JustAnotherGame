using System.Collections.Generic;
using Omniplatformer.Objects;

namespace Omniplatformer.Scenes.Subsystems
{
    /// <summary>
    /// The system responsible for simply processing the objects' states
    /// </summary>
    class SimulationSystem : ISubsystem
    {
        // TODO: extract this to a separate component?
        public List<GameObject> Objects { get; set; } = new List<GameObject>();

        public SimulationSystem()
        {

        }

        public void RegisterObject(GameObject obj)
        {
            if (obj.Tickable)
                Objects.Add(obj);
        }

        public void UnregisterObject(GameObject obj)
        {
            Objects.Remove(obj);
        }

        public void Tick(float dt)
        {
            for (int j = Objects.Count - 1; j >= 0; j--)
            {
                var obj = Objects[j];
                obj.Tick(dt);
            }
        }
    }
}
