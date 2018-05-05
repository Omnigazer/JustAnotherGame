using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using Omniplatformer.Enums;
using System;
using System.Collections.Generic;

namespace Omniplatformer.HUD
{
    /// <summary>
    /// Handles the inventory view logic
    /// </summary>
    public class CharView
    {
        Player Player => GameService.Player;
        // SkillSlot slot;
        List<SkillSlot> slots;

        public CharView()
        {
            // slot = new SkillSlot(Enums.Skill.Melee, 0);
            slots = new List<SkillSlot>();
            int i = 0;
            foreach(Skill skill in Enum.GetValues(typeof(Skill)))
            {
                slots.Add(new SkillSlot(skill, i++));
                //slots.Add(new SkillSlot(arr[, 0));
            }
        }

        public void Draw()
        {
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            DrawAvailablePoints();
            foreach (var slot in slots)
            {
                int value = 0;
                if (Player.Skills.ContainsKey(slot.Skill))
                    value = Player.Skills[slot.Skill];
                DrawSkill(slot, value);
            }
            spriteBatch.End();
        }

        public Rectangle GetRect(SkillSlot slot)
        {
            var slot_offset = new Point(0, (slot_height + slot_margin) * slot.Index);
            return new Rectangle(position + slot_offset, new Point(slot_width, slot_height));
        }

        // slots starting position
        private Point position => new Point(700, 150);
        const int slot_width = 700, slot_height = 70;
        const int slot_margin = 15;

        public void DrawSkill(SkillSlot slot, int value)
        {
            var spriteBatch = GraphicsService.Instance;
            // Point slot_position = new Point(position.X + (slot_width + slot_margin) * slot.Column, position.Y + (slot_height + slot_margin) * slot.Row);
            Point slot_position = new Point(position.X, position.Y + (slot_height + slot_margin) * slot.Index);
            Point string_position = new Point(slot_position.X + 10, slot_position.Y + 22);
            Point size = new Point(slot_width, slot_height);
            Rectangle outer_rect = new Rectangle(slot_position, size);

            // Rectangle inner_rect = new Rectangle(bar_position + border_size, size);
            // float alpha = slot.IsHighlighted ? 1 : 0.3f;
            // spriteBatch.Draw(GameContent.Instance.whitePixel, outer_rect, Color.Gray * alpha);
            spriteBatch.Draw(GameContent.Instance.whitePixel, outer_rect, Color.Gray);
            spriteBatch.DrawString(GameContent.Instance.defaultFont, slot.Skill.ToString(), string_position.ToVector2(), Color.White);
            spriteBatch.DrawString(GameContent.Instance.defaultFont, value.ToString(), string_position.ToVector2() + new Vector2(200, 0), Color.White);
        }

        public void DrawAvailablePoints()
        {
            var spriteBatch = GraphicsService.Instance;

            Point slot_position = new Point(position.X, position.Y - (slot_height + slot_margin));
            Point string_position = new Point(slot_position.X + 10, slot_position.Y + 22);
            Point size = new Point(slot_width, slot_height);
            Rectangle outer_rect = new Rectangle(slot_position, size);

            var points = Player.SkillPoints;

            spriteBatch.Draw(GameContent.Instance.whitePixel, outer_rect, Color.Gray);
            spriteBatch.DrawString(GameContent.Instance.defaultFont, "Skill Points", string_position.ToVector2(), Color.White);
            spriteBatch.DrawString(GameContent.Instance.defaultFont, points.ToString(), string_position.ToVector2() + new Vector2(200, 0), Color.White);
        }

        public SkillSlot GetSlotAtPosition(Point pos)
        {
            foreach (var slot in slots)
            {
                var rect = GetRect(slot);
                if (rect.Contains(pos))
                {
                    return slot;
                }
            }
            return null;
        }

        /*
        public void HoverSlot(InventorySlot slot)
        {
            foreach (var i_slot in Inventory.slots)
            {
                i_slot.IsHovered = (i_slot == slot);
            }
        }
        */
    }

    public class SkillSlot
    {
        public Skill Skill { get; set; }
        public int Index { get; set; }

        public SkillSlot(Skill skill, int index)
        {
            Skill = skill;
            Index = index;
        }

        public void Draw()
        {

        }
    }
}
