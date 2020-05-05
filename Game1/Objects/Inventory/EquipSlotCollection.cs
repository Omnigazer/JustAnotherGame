using System.Collections.Generic;
using System.Linq;

namespace Omniplatformer.Objects.Inventory
{
    public class EquipSlotCollection // : IEnumerable<EquipSlot>
    {
        public ChannelSlot ChannelSlot { get; set; }
        public RightHandSlot RightHandSlot { get; set; }
        public LeftHandSlot LeftHandSlot { get; set; }
        public List<MiscSlot> MiscSlots { get; set; }

        public EquipSlotCollection()
        {

        }

        public static EquipSlotCollection Create()
        {
            var collection = new EquipSlotCollection()
            {
                MiscSlots = new List<MiscSlot>(),
                LeftHandSlot = new LeftHandSlot(),
                RightHandSlot = new RightHandSlot(),
                ChannelSlot = new ChannelSlot()
            };
            for (int i = 0; i < 6; i++)
            {
                collection.MiscSlots.Add(new MiscSlot());
            }
            return collection;
        }

        public IEnumerable<Slot> GetSlots()
        {
            yield return ChannelSlot;
            yield return LeftHandSlot;
            yield return RightHandSlot;
            foreach(var slot in MiscSlots)
            {
                yield return slot;
            }
        }
    }
}
