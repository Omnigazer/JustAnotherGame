using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;

namespace Omniplatformer.Objects
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
