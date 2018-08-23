using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Items;

namespace Omniplatformer
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
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                {
                    slots.Add(new InventorySlot(i, j));
                }
            // TODO: make the component ask the inventory about it instead
            CurrentSlot = slots[0];
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

        public InventorySlot(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
