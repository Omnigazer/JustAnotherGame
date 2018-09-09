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
        public ChannelSlot ChannelSlot { get; set; }
        public EquipSlot HandSlot { get; set; }
        public List<MiscSlot> MiscSlots { get; set; }

        public EquipSlotCollection()
        {
            MiscSlots = new List<MiscSlot>();
            for(int i =0;i<6;i++)
            {
                MiscSlots.Add(new MiscSlot());
            }
            HandSlot = new EquipSlot();
            ChannelSlot = new ChannelSlot();
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
