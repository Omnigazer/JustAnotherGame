using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Utility;

namespace Omniplatformer.Objects.Items
{
    public class ChaosOrb : Item
    {
        int ChaosBonus { get; set; } = 1;
        float ManaRegenBonus { get; set; } = 0.05f / 60;

        public ChaosOrb(Texture2D texture = null)
        {
            if (texture == null)
                texture = GameContent.Instance.chaos_orb;
            Team = Team.Friend;
            // Damage = damage;
            var halfsize = new Vector2(25, 25);
            Descriptors.Add(Descriptor.ChannelSlot);
            Components.Add(new PhysicsComponent(this, Vector2.Zero, halfsize, new Vector2(0.5f, 0.1f)));
            Components.Add(new GlowingRenderComponent(this, Color.White, texture) { Radius = 200 });
        }

        public override void OnEquip(Character character)
        {
            var bonusable = (BonusComponent)character;
            // bonusable.SkillBonuses[Skill.Chaos].Add(1);
            bonusable.ManaRegenBonuses[ManaType.Chaos].Add(ManaRegenBonus);
            base.OnEquip(character);
        }

        public override void OnUnequip(Character character)
        {
            var bonusable = (BonusComponent)character;
            // bonusable.SkillBonuses[Skill.Chaos].Remove(1);
            bonusable.ManaRegenBonuses[ManaType.Chaos].Remove(ManaRegenBonus);
            base.OnUnequip(character);
        }

        public override object AsJson()
        {
            return new
            {
                Id,
                type = GetType().AssemblyQualifiedName
            };
        }

        public static GameObject FromJson(Deserializer deserializer)
        {
            return new ChaosOrb();
        }
    }
}
