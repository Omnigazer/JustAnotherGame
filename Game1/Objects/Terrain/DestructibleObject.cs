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


        public override void InitializeCustomComponents()
        {
            RegisterComponent(new PhysicsComponent() { Solid = true, Hittable = true });
            RegisterComponent(new WallRenderComponent(Color.Yellow));
            var damageable = new HitPointComponent(10);
            RegisterComponent(damageable);
            damageable._onBeginDestroy += StartDeathAnimation;
        }

        public static DestructibleObject Create(Vector2 coords, Vector2 halfsize)
        {
            var quad = new DestructibleObject();
            quad.InitializeComponents();
            var pos = quad.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            pos.SetLocalHalfsize(halfsize);
            return quad;
        }
    }
}
