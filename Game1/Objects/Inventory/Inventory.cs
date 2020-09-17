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
        public List<InventorySlot> slots = new List<InventorySlot>();
        public InventorySlot CurrentSlot;

        public static Inventory Create()
        {
            var inventory = new Inventory();
            for (int i = 0; i < inventory.Rows; i++)
                for (int j = 0; j < inventory.Cols; j++)
                {
                    inventory.slots.Add(new InventorySlot(i, j));
                }
            // TODO: make the component ask the inventory about it instead
            inventory.CurrentSlot = inventory.slots[0];
            return inventory;
        }

        public void AddItem(Item item)
        {
            slots.Find(slot => slot.Item == null).Item = item;
        }

        public void SetCurrentSlot(InventorySlot slot)
        {
            CurrentSlot.IsCurrent = false;
            CurrentSlot = slot;
            slot.IsCurrent = true;
        }

        public InventorySlot GetSlot(int row, int col)
        {
            return slots[row * Cols + col];
        }

        public void MoveLeft()
        {
            int row = CurrentSlot.Row, col = CurrentSlot.Column;
            col = (col - 1 + Cols) % Cols;
            SetCurrentSlot(GetSlot(row, col));
        }

        public void MoveUp()
        {
            int row = CurrentSlot.Row, col = CurrentSlot.Column;
            row = (row - 1 + Rows) % Rows;
            SetCurrentSlot(GetSlot(row, col));
        }

        public void MoveRight()
        {
            int row = CurrentSlot.Row, col = CurrentSlot.Column;
            col = (col + 1 + Cols) % Cols;
            SetCurrentSlot(GetSlot(row, col));
        }

        public void MoveDown()
        {
            int row = CurrentSlot.Row, col = CurrentSlot.Column;
            row = (row + 1 + Rows) % Rows;
            SetCurrentSlot(GetSlot(row, col));
        }
    }

    public abstract class Slot
    {
        public Item Item { get; set; }
        public bool IsCurrent { get; set; }
        protected virtual IEnumerable<Descriptor> AcceptedDescriptors() { return Enumerable.Empty<Descriptor>(); }
        public bool AcceptsItem(Item item) { return AcceptedDescriptors().Intersect(item.Descriptors).Any(); }
        public virtual void OnItemAdd(Item item) { }
        public virtual void OnItemRemove(Item item) { }

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
            item = Item.Copy();
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
            Item = item.Copy();
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

    public class InventorySlot : Slot
    {
        /// <summary>
        /// Zero-based slot index
        /// </summary>
        public int Column { get; set; }

        public int Row { get; set; }

        public bool IsHovered { get; set; }
        public bool IsHighlighted => IsCurrent || IsHovered;
        protected override IEnumerable<Descriptor> AcceptedDescriptors()
        {
            yield return Descriptor.Item;
        }

        public InventorySlot(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }

    public class EquipSlot : Slot
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

    public class MiscSlot : Slot
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

    public class ChannelSlot : Slot
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
