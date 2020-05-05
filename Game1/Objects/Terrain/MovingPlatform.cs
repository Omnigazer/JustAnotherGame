using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Terrain
{
    class MovingPlatform : GameObject
    {
        public MovingPlatform(Vector2 coords, Vector2 halfsize)
        {
            Components.Add(new PlatformMoveComponent(this, coords, halfsize) { Solid = true, Friction = 0.08f });
            Components.Add(new RenderComponent(this));
        }
    }
}
