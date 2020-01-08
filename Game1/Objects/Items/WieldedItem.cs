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
        readonly Texture2D texture = GameContent.Instance.cursor;

        public WieldedItem(int damage)
        {
            Team = Team.Friend;
            Descriptors.Add(Descriptor.RightHandSlot);
            Components.Add(new PhysicsComponent(this, Vector2.Zero, halfsize, new Vector2(0.5f, 0.1f)));
            Components.Add(new RenderComponent(this, Color.White, texture));
            Components.Add(new MeleeDamageHitComponent(this, damage, knockback: knockback));
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

        public override object AsJson()
        {
            return new
            {
                Id,
                type = GetType().AssemblyQualifiedName,
                damage = GetComponent<MeleeDamageHitComponent>().Damage
            };
        }

        public static GameObject FromJson(Deserializer deserializer)
        {
            // var item = new WieldedItem((int)data["damage"]) { Id = id };
            var data = deserializer.getData();
            var item = new WieldedItem((int)data["damage"]);
            // SerializeService.Instance.RegisterObject(item);
            return item;
        }

        public void SetWielder(GameObject source)
        {
            Source = source;
        }
    }
}
