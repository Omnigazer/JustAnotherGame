using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Terrain
{
    class DestructibleObject : GameObject
    {
        public DestructibleObject(Vector2 center, Vector2 halfsize)
        {
            // TODO: supply colors / textures to the component
            Components.Add(new PhysicsComponent(this, center, halfsize) { Solid = true, Hittable = true });
            Components.Add(new WallRenderComponent(this, Color.Yellow));
            var damageable = new HitPointComponent(this, 10);
            Components.Add(damageable);
            damageable._onBeginDestroy += StartDeathAnimation;
        }

        private void StartDeathAnimation(object sender, System.EventArgs e)
        {
            var drawable = GetComponent<AnimatedRenderComponent>();
            drawable._onAnimationEnd += onDeathAnimationEnd;
            drawable.StartAnimation(AnimationType.Death, 50);
        }

        private void onDeathAnimationEnd(object sender, AnimationEventArgs e)
        {
            if (e.animation == AnimationType.Death)
            {
                onDestroy();
            }
        }
    }
}
