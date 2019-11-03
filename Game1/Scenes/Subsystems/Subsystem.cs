using Omniplatformer.Objects;

namespace Omniplatformer.Scenes.Subsystems
{
    public interface ISubsystem
    {
        void Tick(float dt);
        void RegisterObject(GameObject obj);
        void UnregisterObject(GameObject obj);
    }
}
