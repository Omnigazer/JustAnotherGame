using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Components.Physics;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Interactibles;
using Omniplatformer.Objects.Inventory;
using Omniplatformer.Objects.Items;

namespace Omniplatformer.Components.Character
{
    public class PlayerInventoryComponent : InventoryComponent
    {
        public override void Compile()
        {
            var movable = GetComponent<DynamicPhysicsComponent>();
            movable.OnCollision += (sender, e) =>
            {
                // TODO: extract this into a component as well
                if (e.Target.GameObject is Item item && e.Target.Pickupable)
                {
                    PickupItem(item);
                    e.Target.Pickupable = false;
                }
                else if (e.Target.GameObject is Collectible collectible)
                {
                    GetBonus(collectible.Bonus);
                    collectible.onDestroy();
                }
            };
        }

        public void PickupItem(Item item)
        {
            Inventory.AddItem(item);
            item.CurrentScene.UnregisterObject(item);
        }

        public void GetBonus(Bonus bonus)
        {
            switch (bonus)
            {
                case Bonus.Jump:
                    {
                        var movable = GetComponent<PlayerMoveComponent>();
                        movable.max_jumps++;
                        break;
                    }
            }
        }
    }
}
