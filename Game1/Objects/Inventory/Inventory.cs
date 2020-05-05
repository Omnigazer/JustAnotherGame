using System.Collections.Generic;
using System.Linq;
using Omniplatformer.Enums;
using Omniplatformer.Objects.Items;
using Omniplatformer.Services;

namespace Omniplatformer.Objects.Inventory
{
    public class Inventory
    {
        public int Rows => 4;
        public int Cols => 8;
        public List<InventorySlot> slots = new List<InventorySlot>();
        public InventorySlot CurrentSlot;

        // public int row, col;

        public Inventory()
        {

        }

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
