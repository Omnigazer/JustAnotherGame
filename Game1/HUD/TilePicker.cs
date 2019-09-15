using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omniplatformer.Components;
using Omniplatformer.HUDStates;
using System.Collections.Generic;
using Omniplatformer.Utility;

namespace Omniplatformer.HUD
{
    public class TilePicker : ViewControl
    {
        const int slot_width = 40, slot_height = 40;
        const int slot_margin = 5;
        const int cols = 5;
        const int rows = 10;

        public TilePicker()
        {
            Init();
        }

        public void Init()
        {
            Children.Clear();
            Width = slot_width * cols + (cols + 1) * slot_margin;
            Height = slot_height * rows + (rows + 1) * slot_margin;
            Position = new Point(40, 150);

            foreach(short key in GameContent.Instance.atlas_meta.Keys)
            {
                RegisterTile(key);
            }
        }

        public void RegisterTile(short type)
        {
            int col = Children.Count % cols;
            int row = Children.Count / cols;
            var item = new TilePickerItem(type)
            {
                Width = slot_width,
                Height = slot_height,
                Position = new Point(
                    slot_margin + (col * (slot_width + slot_margin)),
                    slot_margin + (row * (slot_height + slot_margin))
                )
            };
            item.MouseClick += (sender, e) => { ((EditorHUDState)GameService.Instance.HUDState).current_tile = type; };
            RegisterChild(item);
        }

        // TODO: find a better place to extract this
        public override void Draw()
        {
            DrawContainer();
            base.Draw();
        }

        public void DrawContainer()
        {
            var spriteBatch = GraphicsService.Instance;
            float alpha = 0.6f;
            spriteBatch.Draw(GameContent.Instance.whitePixel, GlobalRect, Color.DarkGray * alpha);
        }
    }

    public class TilePickerItem : ViewControl
    {
        public short TileType { get; set; }
        public TilePickerItem(short type)
        {
            TileType = type;
        }

        public override void Draw()
        {
            base.Draw();
            DrawItem();
        }

        public void DrawItem()
        {
            var spriteBatch = GraphicsService.Instance;
            float alpha = 1.0f;
            spriteBatch.Draw(GameContent.Instance.whitePixel, GlobalRect, Color.DarkGray * alpha);
            spriteBatch.Draw(GameContent.Instance.atlas, GlobalRect, GameContent.Instance.atlas_meta[TileType], Color.White);
            if (Hover)
                DrawBorder(thickness: 2);
        }
    }
}
