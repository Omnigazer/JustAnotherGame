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

        public static ChaosOrb Create()
        {
            var orb = new ChaosOrb();
            orb.InitializeComponents();
            return orb;
        }

        public override void InitializeCustomComponents()
        {
            string texture = "Textures/chaos_orb";
            var halfsize = new Vector2(25, 25);
            RegisterComponent(new PhysicsComponent(Vector2.Zero, halfsize, new Vector2(0.5f, 0.1f)));
            RegisterComponent(new GlowingRenderComponent(Color.White, texture) { Radius = 200 });
            Descriptors.Add(Descriptor.ChannelSlot);            
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
    }
}
