using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
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
        }
        // TODO: should be extracted to damageable component
        float hit_points = 10;

        public override void ApplyDamage(float damage)
        {
            hit_points -= damage;
            if (hit_points <= 0)
            {
                var drawable = GetComponent<AnimatedRenderComponent>();
                drawable._onAnimationEnd += onDeathAnimationEnd;
                drawable.StartAnimation(AnimationType.Death, 50);
            }
        }

        private void onDeathAnimationEnd(object sender, AnimationEventArgs e)
        {
            if (e.animation == AnimationType.Death)
            {
                onDestroy();
            }
        }

        public override object AsJson()
        {
            return new { type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }

        public static GameObject FromJson(JObject data)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(data);
            return new DestructibleObject(coords, halfsize);
        }
    }
}
