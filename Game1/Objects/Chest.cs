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
                Inventory.slots[i].item = items.ElementAt(i);
            }
            Pickupable = false;
            Solid = false;
            Components.Add(new PositionComponent(this, coords, halfsize));
            Components.Add(new RenderComponent(this, Color.Firebrick));
        }

        public Chest(Vector2 coords, Vector2 halfsize, params WieldedItem[] items) : this(coords, halfsize, (IEnumerable<WieldedItem>)items)
        {

        }

        public override object AsJson()
        {
            return PositionJson.ToJson(this);
        }

        public static GameObject FromJson(JObject data)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(data);
            return new Chest(coords, halfsize);
        }
    }
}
