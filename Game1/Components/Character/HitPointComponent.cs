using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        [JsonProperty]
        float _currentHitPoints;
        [JsonIgnore]
        public float CurrentHitPoints { get => _currentHitPoints; set => _currentHitPoints = value.LimitToRange(0, MaxHitPoints); }
        public float MaxHitPoints { get; set; }
        bool is_dying;

        public HitPointComponent() { }
        public HitPointComponent(GameObject obj, float hit_points) : base(obj) { CurrentHitPoints = MaxHitPoints = hit_points; }

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
    }
}
