using System;
using System.Collections.Generic;
using System.Linq;
using Omniplatformer.Enums;
using Omniplatformer.Objects.Items;
using Omniplatformer.Services;

namespace Omniplatformer.Objects.InventoryNS
{
    public class Inventory
    {
        public int Rows => 4;
        public int Cols => 8;
        public List<ItemSlot> slots = new List<ItemSlot>();

        public static Inventory Create()
        {
            var inventory = new Inventory();
            for (int i = 0; i < inventory.Rows; i++)
                for (int j = 0; j < inventory.Cols; j++)
                {
                    inventory.slots.Add(new ItemSlot());
                }
            return inventory;
        }

        public void AddItem(Item item)
        {
            slots.Find(slot => slot.Item == null).Item = item;
        }

        public ItemSlot GetSlotForItem(string item_id, int count)
        {
            // TODO: improve this
            foreach (var slot in slots)
            {
                if (slot.Item != null && slot.Item.ItemId == item_id && slot.Item.Count >= count)
                    return slot;
            }
            return null;
        }

        public bool HasItem(string item_id, int count)
        {
            return GetSlotForItem(item_id, count) != null;
        }

        public ItemSlot GetEmptySlot()
        {
            foreach (var slot in slots)
            {
                if (slot.Item == null)
                    return slot;
            }
            return null;
        }

    }

    public class ItemSlot
    {
        public Item Item { get; set; }
        protected virtual IEnumerable<Descriptor> AcceptedDescriptors() { yield return Descriptor.Item; }
        public bool AcceptsItem(Item item) { return AcceptedDescriptors().Intersect(item.Descriptors).Any(); }
        public virtual void OnItemAdd(Item item) { }
        public virtual void OnItemRemove(Item item) { }

        /// <summary>
        /// Destroys the specified number of items in the inventory
        /// </summary>
        /// <param name="count"></param>
        public void DrainItem(int count)
        {
            Item.Count -= count;
            if (Item.Count == 0)
                Item = null;
        }

        public void TakeItem(ref Item item)
        {
            item = Item;
            Item = null;
            if (item != null)
                OnItemRemove(item);
        }

        public void PutItem(ref Item item)
        {
            Item = item;
            OnItemAdd(Item);
            item = null;
        }

        public void MergeStack(ref Item item)
        {
            int val = Math.Min(Item.MaxCount - Item.Count, item.Count);
            Item.Count += val;
            item.Count -= val;
            if (item.Count <= 0)
                item = null;
        }

        public void SplitStack(ref Item item)
        {
            item = (Item)Item.Clone();
            var count = Item.Count / 2;
            if (count == 0)
                count++;
            item.Count = count;
            Item.Count -= item.Count;
            if (Item.Count <= 0)
            {
                Item = null;
                OnItemRemove(Item);
            }
        }

        public void PutFirstItem(ref Item item)
        {
            Item = (Item)item.Clone();
            Item.Count = 1;
            item.Count--;
            if (item.Count <= 0)
                item = null;
            OnItemAdd(Item);
        }

        public void MergeFirstItem(ref Item item)
        {
            int val = Math.Min(Item.MaxCount - Item.Count, 1);
            Item.Count += val;
            item.Count -= val;
            if (item.Count <= 0)
                item = null;
        }

        public void SwapItem(ref Item item)
        {
            var tmp = item;
            item = Item;
            Item = tmp;
            OnItemRemove(item);
            OnItemAdd(tmp);
        }
    }

    public class EquipSlot : ItemSlot
    {
        protected override IEnumerable<Descriptor> AcceptedDescriptors()
        {
            yield return Descriptor.HandSlot;
        }

        public override void OnItemAdd(Item item)
        {
            item.OnEquip(GameService.Player);
        }

        public override void OnItemRemove(Item item)
        {
            item.OnUnequip(GameService.Player);
        }
    }

    public class RightHandSlot : EquipSlot
    {
        protected override IEnumerable<Descriptor> AcceptedDescriptors()
        {
            yield return Descriptor.HandSlot;
            yield return Descriptor.RightHandSlot;
        }
    }

    public class LeftHandSlot : EquipSlot
    {
        protected override IEnumerable<Descriptor> AcceptedDescriptors()
        {
            yield return Descriptor.HandSlot;
            yield return Descriptor.LeftHandSlot;
        }
    }

    public class MiscSlot : ItemSlot
    {
        protected override IEnumerable<Descriptor> AcceptedDescriptors()
        {
            yield return Descriptor.MiscSlot;
        }

        public override void OnItemAdd(Item item)
        {
            item.OnEquip(GameService.Player);
        }

        public override void OnItemRemove(Item item)
        {
            item.OnUnequip(GameService.Player);
        }
    }

    public class ChannelSlot : ItemSlot
    {
        protected override IEnumerable<Descriptor> AcceptedDescriptors()
        {
            yield return Descriptor.ChannelSlot;
        }

        public override void OnItemAdd(Item item)
        {
            item.OnEquip(GameService.Player);
        }

        public override void OnItemRemove(Item item)
        {
            item.OnUnequip(GameService.Player);
        }
    }
}
