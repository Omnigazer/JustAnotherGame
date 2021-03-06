﻿using System;
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
        public EquipSlot RightHandSlot { get; set; }
        public EquipSlot LeftHandSlot { get; set; }
        public List<MiscSlot> MiscSlots { get; set; }

        public EquipSlotCollection()
        {
            MiscSlots = new List<MiscSlot>();
            for(int i =0;i<6;i++)
            {
                MiscSlots.Add(new MiscSlot());
            }
            LeftHandSlot = new EquipSlot();
            RightHandSlot = new EquipSlot();
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
