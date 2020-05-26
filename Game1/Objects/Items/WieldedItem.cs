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
    public class WieldedItem : Item
    {
        readonly Vector2 knockback = new Vector2(4, 3);
        readonly Vector2 halfsize = new Vector2(3, 25);
        readonly string texture = "Textures/cursor2";

        public static WieldedItem Create(int damage)
        {
            var item = new WieldedItem();
            item.InitializeComponents();
            var c = item.GetComponent<DamageHitComponent>();
            c.Damage = damage;
            return item;
        }

        public override void InitializeCustomComponents()
        {
            Descriptors.Add(Descriptor.RightHandSlot);
            RegisterComponent(new PhysicsComponent(Vector2.Zero, halfsize, new Vector2(0.5f, 0.1f)));
            RegisterComponent(new RenderComponent(Color.White, texture));
            RegisterComponent(new MeleeDamageHitComponent(0, knockback: knockback));
        }

        public override void OnEquip(Character character)
        {
            SetWielder(character);
            // draw-related
            var item_pos = (PositionComponent)this;
            item_pos.SetParent(character, AnchorPoint.RightHand);
            // character.CurrentScene.RegisterObject(this);
        }

        public override void OnUnequip(Character character)
        {
            SetWielder(null);
            var item_pos = (PositionComponent)this;
            item_pos.ClearParent();
            // character.CurrentScene.UnregisterObject(this);
        }

        public void SetWielder(GameObject source)
        {
            Source = source;
        }
    }
}
