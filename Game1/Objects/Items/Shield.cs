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
        public Shield(Texture2D texture = null)
        {
            if (texture == null)
                texture = GameContent.Instance.shield;
            Team = Team.Friend;
            // Damage = damage;
            var halfsize = new Vector2(10, 32);
            Descriptors.Add(Descriptor.LeftHandSlot);
            Components.Add(new PhysicsComponent(this, Vector2.Zero, halfsize, new Vector2(0.5f, 0.1f)) { Hittable = true });
            Components.Add(new RenderComponent(this, Color.White, texture, 2));
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
            // var item = new WieldedItem((int)data["damage"]) { Id = id };
            var data = deserializer.getData();
            var item = new Shield();
            return item;
        }

        public void SetWielder(GameObject source)
        {
            Source = source;
        }
    }
}
