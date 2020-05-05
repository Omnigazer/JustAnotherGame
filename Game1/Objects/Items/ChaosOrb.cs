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

        public ChaosOrb()
        {
            Team = Team.Friend;
        }

        public static ChaosOrb Create()
        {
            var orb = new ChaosOrb();
            orb.InitComponents();
            return orb;
        }

        public void InitComponents()
        {
            string texture = "Textures/chaos_orb";
            var halfsize = new Vector2(25, 25);
            Components.Add(new PhysicsComponent(this, Vector2.Zero, halfsize, new Vector2(0.5f, 0.1f)));
            Components.Add(new GlowingRenderComponent(this, Color.White, texture) { Radius = 200 });
            Descriptors.Add(Descriptor.ChannelSlot);
        }

        public ChaosOrb(string texture = null)
        {

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
