using Microsoft.Xna.Framework;
using Omniplatformer.HUDStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    public class StatusView : ViewControl
    {
        HUDState hud;

        public StatusView(HUDState hud)
        {
            this.hud = hud;
        }

        public override void SetupNode()
        {
            Width = 50;
        }

        public void DrawMessages()
        {
            // TODO: TEST
            var spriteBatch = GraphicsService.Instance;
            // Point log_position = new Point(log_margin, 300);
            Point log_position = GlobalRect.Location;
            int i = 0;
            void displayMessage(string message)
            {
                spriteBatch.DrawString(GameContent.Instance.defaultFont, message, (log_position + new Point(20, 20 + 20 * i++)).ToVector2(), Color.White);
            }
            // Draw directly via the SpriteBatch instance bypassing y-axis flip
            // GraphicsService.Instance.Draw(GameContent.Instance.whitePixel, rect, Color.Gray * 0.8f);
            foreach (var msg in hud.GetStatusMessages())
            {
                displayMessage(msg);
            }
            hud.status_messages.Clear();
        }

        public override void DrawSelf()
        {
            DrawMessages();
        }
    }
}
