using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components.Character
{
    class InvFramesComponent : Component
    {
        public int InvFrames { get; set; } = 0;
        HitPointComponent Damageable => GetComponent<HitPointComponent>();

        public override void Compile()
        {
            Damageable.OnDamage.Subscribe((damage) => OnDamage(damage));
        }

        public void OnDamage(float damage)
        {
            var drawable = GetComponent<CharacterRenderComponent>();
            if (damage >= 0)
            {
                drawable.StartAnimation(AnimationType.Hit, InvFrames);
                Damageable.Vulnerable = false;
                drawable.onAnimationEnd.Where((animation_type) => animation_type == AnimationType.Hit)
                                        .FirstAsync().Subscribe((_) => Damageable.Vulnerable = true);
            }
        }
    }
}
