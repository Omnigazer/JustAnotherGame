using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Utility;
using Omniplatformer.Items;
using Newtonsoft.Json.Linq;

namespace Omniplatformer
{
    public class WieldedItem : Item
    {
        // public int Damage { get; set; }
        // public Vector2 Knockback { get; set; }
        public override GameObject Source => _source?.Source ?? this;
        private GameObject _source;

        public WieldedItem(int damage, Texture2D texture = null)
        {
            if (texture == null)
                texture = GameContent.Instance.cursor;
            Team = Team.Friend;
            // Damage = damage;
            var Knockback = new Vector2(4, 3);
            var halfsize = new Vector2(3, 25);
            Descriptors.Add(Descriptor.HandSlot);
            Components.Add(new PhysicsComponent(this, Vector2.Zero, halfsize, new Vector2(0.5f, 0.1f)));
            Components.Add(new RenderComponent(this, Color.White, texture));
            Components.Add(new MeleeDamageHitComponent(this, damage, Knockback));
        }

        public override void OnEquip(Character character)
        {
            SetWielder(character);
            // draw-related
            var item_pos = (PositionComponent)this;
            item_pos.SetParent(character, AnchorPoint.RightHand);
            Game.AddToMainScene(this);
        }

        public override void OnUnequip(Character character)
        {
            SetWielder(null);
            var item_pos = (PositionComponent)this;
            item_pos.ClearParent();
            Game.RemoveFromMainScene(this);
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
            _source = source;
        }
    }
}
