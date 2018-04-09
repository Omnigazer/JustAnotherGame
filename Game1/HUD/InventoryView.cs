using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using System.Collections.Generic;

namespace Omniplatformer.HUD
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

    /// <summary>
    /// Handles the view logic
    /// </summary>
    public class InventoryView
    {
        Inventory Inventory { get; set; }

        public InventoryView(Inventory inventory)
        {
            Inventory = inventory;
        }

        public void Draw()
        {
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            foreach (var slot in Inventory.slots)
            {
                DrawSlot(slot);
            }
            spriteBatch.End();
        }

        // slots starting position
        Point position = new Point(700, 150);

        public void DrawSlot(InventorySlot slot)
        {
            const int slot_width = 100, slot_height = 100;
            const int slot_margin = 25;

            var spriteBatch = GraphicsService.Instance;
            Point slot_position = new Point(position.X + (slot_width + slot_margin) * slot.Column, position.Y + (slot_height + slot_margin) * slot.Row);
            Point size = new Point(slot_width, slot_height);
            Rectangle outer_rect = new Rectangle(slot_position, size);

            // Rectangle inner_rect = new Rectangle(bar_position + border_size, size);
            float alpha = slot.IsCurrent ? 1 : 0.3f;
            spriteBatch.Draw(GameContent.Instance.whitePixel, outer_rect, Color.Gray * alpha);
            outer_rect.Inflate(-5, -5);
            if (slot.item != null)
            {
                var renderable = (RenderComponent)slot.item;
                spriteBatch.Draw(renderable.Texture, outer_rect, Color.White);
            }
        }
    }
}
