using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Utility;

namespace Omniplatformer.Components.Character
{
    public class DamageEventArgs : EventArgs
    {
        public float Damage { get; set; }
        public DamageEventArgs(float damage)
        {
            Damage = damage;
        }
    }
    public class HitPointComponent : Component
    {
        public bool Vulnerable { get; set; } = true;
        /// <summary>
        /// Default invulnerability frames on taking damage
        /// </summary>
        public int InvFrames { get; set; } = 0;
        [JsonProperty]
        float _currentHitPoints;
        [JsonIgnore]
        public float CurrentHitPoints { get => _currentHitPoints; set => _currentHitPoints = value.LimitToRange(0, MaxHitPoints); }
        public float MaxHitPoints { get; set; }
        bool is_dying;

        public HitPointComponent() { }
        public HitPointComponent(float hit_points) { CurrentHitPoints = MaxHitPoints = hit_points; }

        public event EventHandler<DamageEventArgs> _onBeginDestroy = delegate { };
        public event EventHandler<DamageEventArgs> _onDamage = delegate { };

        public void ApplyDamage(float damage)
        {
            if (is_dying || !Vulnerable)
                return;

            CurrentHitPoints -= damage;
            _onDamage(this, new DamageEventArgs(damage));
            if (CurrentHitPoints <= 0)
            {
                is_dying = true;
                _onBeginDestroy(this, new DamageEventArgs(damage));
            }
        }

        public override void Compile()
        {
            var damageable = GetComponent<HitPointComponent>();
            damageable._onDamage += OnDamage;
            damageable._onBeginDestroy += (sender, e) => GameObject.onDestroy();
        }

        public void OnDamage(object sender, DamageEventArgs e)
        {
            if (InvFrames > 0)
                return;

            var hit_points = GetComponent<HitPointComponent>();
            var drawable = GetComponent<CharacterRenderComponent>();
            if (e.Damage >= 0)
            {
                drawable.StartAnimation(AnimationType.Hit, InvFrames);
                hit_points.Vulnerable = false;
            }
            drawable._onAnimationEnd += Drawable__onAnimationEnd;
        }

        private void Drawable__onAnimationEnd(object sender, AnimationEventArgs e)
        {
            if (e.animation == AnimationType.Hit)
            {
                var drawable = (CharacterRenderComponent)sender;
                var hit_points = GetComponent<HitPointComponent>();
                drawable._onAnimationEnd -= Drawable__onAnimationEnd;
                hit_points.Vulnerable = true;
            }
        }
    }
}
