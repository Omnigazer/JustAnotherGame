using Microsoft.Xna.Framework;
using Omniplatformer.Animations;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Rendering
{
    class WallRenderComponent : AnimatedRenderComponent
    {
        public WallRenderComponent() { }

        public WallRenderComponent(Color color) : base(color)
        {
            AddAnimation(new DeathAnimation(this));
        }
    }
}
