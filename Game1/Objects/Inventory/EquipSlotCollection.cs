using System.Collections.Generic;

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
            MiscSlots = new List<MiscSlot>();
            for (int i = 0; i < 6; i++)
            {
                MiscSlots.Add(new MiscSlot());
            }
            LeftHandSlot = new LeftHandSlot();
            RightHandSlot = new RightHandSlot();
            ChannelSlot = new ChannelSlot();
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

        /*
        IEnumerator IEnumerable.GetEnumerator()
        {
            return miscSlots.GetEnumerator();
        }
        */
    }
}
