using System.Collections.Generic;

namespace Omniplatformer.Scenes.Subsystems
{
    /// <summary>
    /// The system responsible for simply processing the objects' states
    /// </summary>
    public class UpdateSystem
    {
        // TODO: extract this to a separate component?
        public List<IUpdatable> Objects { get; set; } = new List<IUpdatable>();

        public UpdateSystem() { }

        public void RegisterObject(IUpdatable obj)
        {
            Objects.Add(obj);
        }

        public void UnregisterObject(IUpdatable obj)
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

    public interface IUpdatable
    {
        void Tick(float dt);
    }
}
