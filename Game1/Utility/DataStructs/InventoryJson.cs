using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Objects.Items;

namespace Omniplatformer.Utility.DataStructs
{
    class InventoryJson
    {
        public static object ToJson(Player obj)
        {
            var items = obj.Inventory.slots
                .Select(x => x.Item)
                .Where(x => x != null)
                .Select(x => x.AsJson()).ToArray();
            return items;
        }

        public static IEnumerable<Item> FromJson(JObject input, Deserializer deserializer)
        {
            JArray items_data = (JArray)input["Inventory"];

            foreach(JObject obj in items_data)
            {
                yield return (Item)deserializer.decodeObject(obj);
            }
        }
    }
}
