using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Items;
using Omniplatformer.Services;
using Omniplatformer.Utility.Extensions;

namespace Omniplatformer.Components.Character
{
    public class DropItemComponent : Component
    {
        public int Value { get; set; }

        public override void Compile()
        {
            GameObject.GetComponent<DestructibleComponent>().OnDestroy.Subscribe((obj) => DropItem());
        }

        public void DropItem()
        {
            // TODO: extract this into a drop component
            Item drop = ChaosOrb.Create();
            GameObject.DropItem(drop);
        }
    }
}
