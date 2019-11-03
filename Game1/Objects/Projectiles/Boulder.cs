using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;

namespace Omniplatformer.Objects.Projectiles
{
    class Boulder : Projectile
    {
        public static float InverseMass => 0.8f;
        public Boulder(Vector2 coords): base()
        {
            TTL = 50000;
            var movable = new ProjectileMoveComponent(this, coords, new Vector2(6, 6)) { Solid = false, Hittable = false, InverseMass = InverseMass };
            Components.Add(movable);
            var c = new AnimatedRenderComponent(this) { Texture = GameContent.Instance.boulder };
            c.AddAnimation(new Animations.DeathAnimation(c));
            Components.Add(c);
            Components.Add(new DamageHitComponent(this, damage: 3));
        }

        public override void onDestroy()
        {
            var drawable = GetComponent<AnimatedRenderComponent>();
            if (drawable != null)
            {
                drawable._onAnimationEnd += (sender, e) => {
                    if (e.animation == AnimationType.Death)
                        base.onDestroy();
                };
                drawable.StartAnimation(AnimationType.Death, 50);
            }
        }
    }
}
