using Omniplatformer.Components.Physics;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Utility.Extensions
{
    public static class GameObjectExtensions
    {
        public static void DropItem(this GameObject obj, Item item)
        {
            var pos = (PhysicsComponent)item;
            pos.SetLocalCoords(obj.GetComponent<PositionComponent>().WorldPosition.Coords);
            pos.Pickupable = true;
            obj.CurrentScene.RegisterObject(item);
        }
    }
}
