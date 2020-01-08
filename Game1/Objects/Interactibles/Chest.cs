﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Objects.Items;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Interactibles
{
    class Chest : GameObject
    {
        public Inventory.Inventory Inventory { get; set; }

        public Chest(Vector2 coords, Vector2 halfsize, IEnumerable<WieldedItem> items)
        {
            Inventory = new Inventory.Inventory();
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
