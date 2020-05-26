using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Utility;

namespace Omniplatformer.Objects.Items
{
    public class Shield : Item
    {
        public static Shield Create()
        {
            var shield = new Shield();
            shield.InitializeComponents();
            return shield;
        }

        public override void InitializeCustomComponents()
        {
            var halfsize = new Vector2(10, 32);
            var texture = "Textures/shield";
            Descriptors.Add(Descriptor.LeftHandSlot);
            RegisterComponent(new PhysicsComponent(Vector2.Zero, halfsize, new Vector2(0.5f, 0.1f)) { Hittable = true });
            RegisterComponent(new RenderComponent(Color.White, texture, 2));
        }

        public override void OnEquip(Character character)
        {
            SetWielder(character);
            // draw-related
            var item_pos = (PositionComponent)this;
            item_pos.SetParent(character, AnchorPoint.LeftHand);
            item_pos.SetLocalCenter(new Vector2(5, 5));
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
