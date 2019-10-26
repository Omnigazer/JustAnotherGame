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
        const int cols = 3;
        const int rows = 10;

        bool IsBackground { get; set; }

        public TilePicker(bool background)
        {
            IsBackground = background;
        }

        public override void SetupNode()
        {
            Node.FlexDirection = Facebook.Yoga.YogaFlexDirection.Row;
            Node.Wrap = Facebook.Yoga.YogaWrap.Wrap;

            Children.Clear();
            Width = slot_width * cols + (cols) * slot_margin * 2;
            Height = slot_height * rows + (rows) * slot_margin * 2;

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
                Height = slot_height
            };
            item.MouseClick += (sender, e) => {
                var editor = ((EditorHUDState)GameService.Instance.HUDState);
                editor.current_tile = type;
                editor.background = IsBackground;
            };
            RegisterChild(item);
        }

        // TODO: find a better place to extract this
        public override void DrawSelf()
        {
            DrawContainer();
        }

        public void DrawContainer()
        {
            var spriteBatch = GraphicsService.Instance;
            float alpha = 0.6f;
            spriteBatch.Draw(GameContent.Instance.whitePixel, GlobalRect, Color.DarkGray * alpha);
        }
    }
}
