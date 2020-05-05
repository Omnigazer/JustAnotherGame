using Microsoft.Xna.Framework;
using Omniplatformer.Content;
using Omniplatformer.HUDStates;
using Omniplatformer.Services;
using Omniplatformer.Views.BasicControls;

namespace Omniplatformer.Views.Editor
{
    public class TilePicker : ViewControl
    {
        const int slot_width = 40, slot_height = 40;
        const int slot_margin = 5;
        const int cols = 3;
        const int rows = 10;

        bool IsBackground { get; set; }

        public TilePicker()
        {

        }

        public override void SetupNode()
        {
            Node.FlexDirection = Facebook.Yoga.YogaFlexDirection.Row;
            Node.Wrap = Facebook.Yoga.YogaWrap.Wrap;

            Clear();
            Width = slot_width * cols + (cols) * slot_margin * 2;
            Height = slot_height * rows + (rows) * slot_margin * 2;

            var checkbox = new CheckBox();
            checkbox.Margin = 5;
            checkbox.MouseClick += (sender, e) => { IsBackground = !IsBackground; (((EditorHUDState)GameService.Instance.HUDState)).background = IsBackground; };

            var row = new Row();
            row.RelativeWidth = 100;
            row.RegisterChild(checkbox);
            RegisterChild(row);

            foreach (short key in GameContent.Instance.atlas_meta.Keys)
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
