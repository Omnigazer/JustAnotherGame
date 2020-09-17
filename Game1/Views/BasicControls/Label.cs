using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Content;
using System.Text;
using Facebook;
using Facebook.Yoga;

namespace Omniplatformer.Views.BasicControls
{
    public class Label : ViewControl
    {
        public string Text { get; set; }
        public override bool ConsumesEvents => false;
        public Label(string text = null)
        {
            Text = text;
        }

        public override void SetupNode()
        {
            Padding = 5;
            BorderThickness = 3;
            Node.SetMeasureFunction(MeasureText);
        }

        public override void DrawSelf()
        {
            DrawBorder(Color.Brown);
            Services.GraphicsService.Instance.DrawString(GameContent.Instance.defaultFont, Text, GlobalPosition.ToVector2() + new Vector2(Padding, Padding), Color.White);
            // Omniplatformer.Services.GraphicsService.Instance.Measure
            // Node.SetMeasureFunction()
            // Node.SetMeasureFunction()
            /*
            var func = new Facebook.Yoga.MeasureFunction();
            Node.SetMeasureFunction(
                func
                );
            */
            // Facebook.Yoga.MeasureFunction
        }

        public YogaSize MeasureText(YogaNode node, float width, YogaMeasureMode width_mode, float height, YogaMeasureMode height_mode)
        {
            var font = GameContent.Instance.defaultFont;
            var (w, h) = font.MeasureString(Text);
            // TODO: first try
            return new YogaSize()
            {
                width = w,
                height = h
            };
        }

        string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }
    }
}
