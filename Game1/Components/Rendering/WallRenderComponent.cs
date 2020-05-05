using Microsoft.Xna.Framework;
using Omniplatformer.Animations;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Rendering
{
    class WallRenderComponent : AnimatedRenderComponent
    {
        public WallRenderComponent() { }

        public WallRenderComponent(GameObject obj) : base(obj)
        {
            AddAnimation(new DeathAnimation(this));
        }

        public WallRenderComponent(GameObject obj, Color color) : base(obj, color)
        {
            AddAnimation(new DeathAnimation(this));
        }
    }
}
