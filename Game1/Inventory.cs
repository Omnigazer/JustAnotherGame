using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public class Inventory
    {
        public int Rows => 4;
        public int Cols => 4;
        public List<InventorySlot> slots = new List<InventorySlot>();
        public InventorySlot CurrentSlot => slots[row * Cols + col];

        public int row, col;

        public Inventory()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    slots.Add(new InventorySlot(i, j));
                }
            // TODO: make the component ask the inventory about it instead
            CurrentSlot.IsCurrent = true;
        }

        public void AddItem(WieldedItem item)
        {
            slots.Find(slot => slot.item == null).item = item;
        }

        public void MoveLeft()
        {
            CurrentSlot.IsCurrent = false;
            col = (((col - 1) % Cols) + Cols) % Cols;
            CurrentSlot.IsCurrent = true;
        }

        public void MoveUp()
        {
            CurrentSlot.IsCurrent = false;
            row = (((row - 1) % Rows) + Rows) % Rows;
            CurrentSlot.IsCurrent = true;
        }

        public void MoveRight()
        {
            CurrentSlot.IsCurrent = false;
            col = (((col + 1) % Cols) + Cols) % Cols;
            CurrentSlot.IsCurrent = true;
        }

        public void MoveDown()
        {
            CurrentSlot.IsCurrent = false;
            row = (((row + 1) % Rows) + Rows) % Rows;
            CurrentSlot.IsCurrent = true;
        }
    }

    public class InventorySlot
    {
        /// <summary>
        /// Zero-based slot index
        /// </summary>
        public int Column { get; set; }
        public int Row { get; set; }

        public WieldedItem item;


        public bool IsCurrent { get; set; }

        public InventorySlot(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
