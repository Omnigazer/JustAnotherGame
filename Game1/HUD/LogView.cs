using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    public class LogView : ViewControl
    {
        Queue<String> LogQueue { get; set; }
        public LogView(Queue<String> log_queue)
        {
            LogQueue = log_queue;
        }

        public override void SetupNode()
        {
            Width = 500;
            Height = 700;
        }

        public void DrawLogs()
        {
            var spriteBatch = GraphicsService.Instance;
            Point log_position = GlobalRect.Location;
            // Draw directly via the SpriteBatch instance bypassing y-axis flip
            GraphicsService.Instance.Draw(GameContent.Instance.whitePixel, GlobalRect, Color.Gray * 0.8f);
            foreach (var (message, i) in LogQueue.Select((x, i) => (x, i)))
            {
                spriteBatch.DrawString(GameContent.Instance.defaultFont, message, (log_position + new Point(20, 20 + 20 * i)).ToVector2(), Color.White);
            }
        }

        public override void DrawSelf()
        {
            DrawLogs();
        }
    }
}
