using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Utility;

namespace Omniplatformer.Components.Character
{
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

        public Subject<float> OnDamage = new Subject<float>();

        public override void Compile()
        {
            GameObject.OnLeaveScene.Subscribe((_) => OnDamage.OnCompleted());
        }

        public void ApplyDamage(float damage)
        {
            if (is_dying || !Vulnerable)
                return;

            CurrentHitPoints -= damage;
            OnDamage.OnNext(damage);
            if (CurrentHitPoints <= 0)
            {
                is_dying = true;
                OnDamage.OnCompleted();
                GetComponent<DestructibleComponent>().Destroy();
            }
        }

        public void ApplyStun(float duration)
        {
            var cooldownable = GetComponent<CooldownComponent>();
            cooldownable.Cooldowns.SetOrAdd("Stun", duration);
        }
    }
}
