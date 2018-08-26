using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Items;

namespace Omniplatformer
{
    public class EquipSlotCollection // : IEnumerable<EquipSlot>
    {
        public EquipSlot HandSlot { get; set; }
        public List<EquipSlot> MiscSlots { get; set; }

        public EquipSlotCollection()
        {
            MiscSlots = new List<EquipSlot>();
            for(int i =0;i<6;i++)
            {
                MiscSlots.Add(new EquipSlot());
            }
            HandSlot = new EquipSlot();
        }

        /*
        public IEnumerator<EquipSlot> GetEnumerator()
        {
            return miscSlots.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return miscSlots.GetEnumerator();
        }
        */
    }
}
