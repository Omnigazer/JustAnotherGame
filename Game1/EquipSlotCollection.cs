using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public class EquipSlotCollection : IEnumerable<EquipSlot>
    {
        public List<EquipSlot> miscSlots;

        public EquipSlotCollection()
        {
            miscSlots = new List<EquipSlot>();
            miscSlots.Add(new EquipSlot());
            miscSlots.Add(new EquipSlot());
        }

        public IEnumerator<EquipSlot> GetEnumerator()
        {
            return miscSlots.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return miscSlots.GetEnumerator();
        }

    }

    public class EquipSlot
    {
        public GameObject Item { get; set; }
    }
}
