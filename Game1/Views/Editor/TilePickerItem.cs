﻿using Microsoft.Xna.Framework;
using Omniplatformer.Content;
using Omniplatformer.Services;

namespace Omniplatformer.Views.Editor
{
    public class TilePickerItem : ViewControl
    {
        public short TileType { get; set; }
        public TilePickerItem(short type)
        {
            TileType = type;
        }

        public override void DrawSelf()
        {
            DrawItem();
        }

        public override void SetupNode()
        {
            Node.Margin = 5;
            BorderThickness = 2;
        }

        public void DrawItem()
        {
            var spriteBatch = GraphicsService.Instance;
            float alpha = 1.0f;
            spriteBatch.Draw(GameContent.Instance.whitePixel, GlobalRect, Color.DarkGray * alpha);
            spriteBatch.Draw(GameContent.Instance.atlas, GlobalRect, GameContent.Instance.atlas_meta[TileType], Color.White);
            if (Hover)
                DrawBorder();
        }
    }
}
