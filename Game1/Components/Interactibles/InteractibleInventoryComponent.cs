using Omniplatformer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;
using Omniplatformer.Services;
using Omniplatformer.Components.Character;

namespace Omniplatformer.Components.Interactibles
{
    public class InteractibleInventoryComponent : InteractibleComponent
    {
        public override void Interact()
        {
            var inventory = GetComponent<InventoryComponent>().Inventory;
            GameService.Instance.OpenTargetInventory(inventory);
        }
    }
}
