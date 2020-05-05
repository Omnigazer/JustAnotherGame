using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Terrain
{
    class ForegroundQuad : GameObject
    {
        public ForegroundQuad(Vector2 center, Vector2 halfsize, Vector2 origin)
        {
            Components.Add(new PhysicsComponent(this, center, halfsize, origin));
            Components.Add(new ForegroundRenderComponent(this, Color.Green));
        }
    }
}
