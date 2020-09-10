using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Interactibles;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Objects.Items;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Interactibles
{
    class Chest : GameObject
    {
        [JsonIgnore]
        public Inventory.Inventory Inventory => GetComponent<InventoryComponent>().Inventory;

        public override void InitializeCustomComponents()
        {
            RegisterComponent(new PhysicsComponent());
            RegisterComponent(new RenderComponent(Color.Firebrick));
            RegisterComponent(new InventoryComponent() { Inventory = Objects.Inventory.Inventory.Create() });
            RegisterComponent(new InteractibleInventoryComponent());
        }

        public static Chest Create(Vector2 coords, Vector2 halfsize, IEnumerable<WieldedItem> items)
        {
            var chest = new Chest();
            chest.InitializeComponents();
            var pos = chest.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            pos.SetLocalHalfsize(halfsize);
            if (items.Count() > chest.Inventory.slots.Count())
                throw new Exception("Items supplied to inventory exceed its capacity");
            for (int i = 0; i < items.Count(); i++)
            {
                chest.Inventory.slots[i].Item = items.ElementAt(i);
            }

            return chest;
        }

        public static Chest Create(Vector2 coords, Vector2 halfsize, params WieldedItem[] items)
        {
            return Create(coords, halfsize, (IEnumerable<WieldedItem>)items);
        }
    }
}
