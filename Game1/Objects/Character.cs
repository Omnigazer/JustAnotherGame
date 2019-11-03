using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Services;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects
{
    public abstract class Character : GameObject
    {
        float _currentHitPoints;
        public float CurrentHitPoints { get => _currentHitPoints; set => _currentHitPoints = value.LimitToRange(0, MaxHitPoints); }
        public float MaxHitPoints { get; set; }
        public bool Vulnerable { get; set; }
        public bool Aggressive { get; set; }
        public virtual int ExpReward => 300;

        public Character()
        {
            CurrentHitPoints = MaxHitPoints = 50;
            Vulnerable = true;
        }

        // Generic characters currently have no need for mana
        public virtual bool SpendMana(ManaType type, float amount) => true;

        public override void onDestroy()
        {
            // TODO: find another approach for earning exp
            GameService.Player.EarnExperience(ExpReward);
            base.onDestroy();
        }

        public override void ApplyDamage(float damage)
        {
            CurrentHitPoints -= damage;
            var drawable = GetComponent<CharacterRenderComponent>();
            drawable.StartAnimation(AnimationType.Hit, 15);
            Cooldowns.SetOrAdd("Stun", 8);
            if (CurrentHitPoints <= 0)
            {
                onDestroy();
            }
        }

        public override object AsJson()
        {
            return new { Id, type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }
    }
}
