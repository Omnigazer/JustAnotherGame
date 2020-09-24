using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Interactibles;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Objects.Items;
using Omniplatformer.Utility;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Interactibles
{
    class Bush : GameObject
    {
        public InventoryNS.Inventory Inventory => GetComponent<InventoryComponent>().Inventory;

        public override void InitializeCustomComponents()
        {
            RegisterComponent(new PhysicsComponent());
            RegisterComponent(new RenderComponent(Color.White, "Textures/bush"));
            RegisterComponent(new InventoryComponent() { Inventory = Objects.InventoryNS.Inventory.Create() });
            RegisterComponent(new InteractibleInventoryComponent());
        }

        public static Bush Create(Vector2 coords, Vector2 halfsize)
        {
            var bush = new Bush();
            bush.InitializeComponents();
            var pos = bush.GetComponent<PositionComponent>();
            pos.SetLocalCoords(coords);
            pos.SetLocalHalfsize(halfsize);
            Item[] items = new Item[] {
                WoodenStick.Create(2),
                Redberry.Create(5),
            };
            if (items.Count() > bush.Inventory.slots.Count())
                throw new Exception("Items supplied to inventory exceed its capacity");
            for (int i = 0; i < items.Count(); i++)
            {
                bush.Inventory.slots[i].Item = items.ElementAt(i);
            }

            return bush;
        }
    }
}
