using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components;
using Omniplatformer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    class Chest : GameObject
    {
        public Inventory Inventory { get; set; }
        public Chest(Vector2 coords, Vector2 halfsize) : this(coords, halfsize, new List<WieldedItem>())
        {

        }

        public Chest(Vector2 coords, Vector2 halfsize, IEnumerable<WieldedItem> items)
        {
            Inventory = new Inventory();
            // foreach(var (item, i) in items.Select((x, i) => (x, i)))
            if (items.Count() > Inventory.slots.Count())
                throw new Exception("Items supplied to inventory exceed its capacity");
            for(int i = 0; i < items.Count(); i++)
            {
                Inventory.slots[i].Item = items.ElementAt(i);
            }
            Components.Add(new PhysicsComponent(this, coords, halfsize));
            Components.Add(new RenderComponent(this, Color.Firebrick));
        }

        public Chest(Vector2 coords, Vector2 halfsize, params WieldedItem[] items) : this(coords, halfsize, (IEnumerable<WieldedItem>)items)
        {

        }

        public override object AsJson()
        {
            return new {
                Id,
                type = GetType().AssemblyQualifiedName,
                Position = PositionJson.ToJson(this),
                Items = new List<object>()
                {
                    Inventory.slots[0].Item.AsJson()
                }
            };
        }

        /*
        public static GameObject FromJson(JObject data)
        {
            Guid id = Guid.Parse(data["Id"].ToString());
            Chest chest = (Chest)SerializeService.Instance.LocateObject(id);

            if (chest == null)
            {
                var (coords, halfsize, origin) = PositionJson.FromJson(data);
                JObject item_data = (JObject)((JArray)data["Items"])?[0];
                if (item_data != null)
                {
                    WieldedItem item = (WieldedItem)WieldedItem.FromJson(item_data);
                    chest = new Chest(coords, halfsize, item) { Id = id };
                }
                else
                    chest = new Chest(coords, halfsize) { Id = id };
                SerializeService.Instance.RegisterObject(chest);
            }
            return chest;
        }
        */

        public static GameObject FromJson(Deserializer deserializer)
        {
            var data = deserializer.getData();
            var (coords, halfsize, origin) = PositionJson.FromJson(data);
            JObject item_data = (JObject)((JArray)data["Items"])?[0];
            GameObject chest;
            if (item_data != null)
            {
                WieldedItem item = (WieldedItem)deserializer.decodeObject(item_data);
                chest = new Chest(coords, halfsize, item);
            }
            else
                chest = new Chest(coords, halfsize);
            return chest;
        }
    }
}
