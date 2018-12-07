using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omniplatformer.Components;
using Omniplatformer.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUDStates
{
    public class DefaultHUDState : HUDState
    {
        HUDContainer playerHUD;
        Game1 Game => GameService.Instance;

        public DefaultHUDState(HUDContainer hud)
        {
            playerHUD = hud;
            MouseUp += DefaultHUDState_MouseUp;
            MouseDown += DefaultHUDState_MouseDown;
            SetupControls();
        }

        public override void Draw()
        {
            playerHUD.Draw();

            // TODO: TEST
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            int log_width = 500, log_margin = 50;
            Point log_position = new Point(Game.GraphicsDevice.PresentationParameters.BackBufferWidth - log_width - log_margin, 300);
            Point log_size = new Point(log_width, 700);
            var rect = new Rectangle(log_position, log_size);
            // Draw directly via the SpriteBatch instance bypassing y-axis flip
            GraphicsService.Instance.Draw(GameContent.Instance.whitePixel, rect, Color.Gray * 0.8f);
            foreach (var (message, i) in Game.Logs.Select((x, i) => (x, i)))
            {
                // GraphicsService.Instance.Draw(GameContent.Instance.whitePixel, rect, Color.Gray);
                spriteBatch.DrawString(GameContent.Instance.defaultFont, message, (log_position + new Point(20, 20 + 20 * i)).ToVector2(), Color.White);
            }
            spriteBatch.End();
            base.Draw();
        }

        public override IEnumerable<string> GetStatusMessages()
        {
            yield return String.Format("Current object: {0}", Game.GetObjectAtCursor());
            foreach (var msg in status_messages)
            {
                yield return msg;
            }
            status_messages.Clear();
        }

        public void SetupControls()
        {
            Action noop = delegate { };
            Controls = new Dictionary<Keys, (Action, Action, bool)>()
            {
                {  Keys.A, (Game.WalkLeft, noop, true) },
                {  Keys.D, (Game.WalkRight, noop, true) },
                {  Keys.S, (Game.Duck, Game.Stand, false) },
                {  Keys.W, (Game.GoUp, noop, true) },
                {  Keys.Space, (Game.Jump, Game.StopJumping, false) },
                {  Keys.I, (Game.OpenInventory, noop, false) },
                {  Keys.Z, (Game.Fire, noop, false) },
                {  Keys.X, (Game.Swing, noop, false) },
                {  Keys.C, (Game.OpenChar, noop, false) },
                {  Keys.E, (Game.OpenChest, noop, false) },
                {  Keys.OemMinus, (Game.ZoomOut, noop, true) },
                {  Keys.OemPlus, (Game.ZoomIn, noop, true) },
                {  Keys.OemCloseBrackets, (Game.ResetZoom, noop, true) },
                {  Keys.F11, (Game.OpenEditor, noop, true) }
            };
        }

        public override void HandleControls()
        {
            // TODO: possibly refactor this
            // reset the player's "intention to move" (move_direction) by default as a workaround
            Game.StopMoving();
            base.HandleControls();
        }

        private void DefaultHUDState_MouseUp(object sender, MouseEventArgs e)
        {
            var coords = new Position(Game.RenderSystem.ScreenToGame(e.Position), new Vector2(0,0));
            if (e.Button == MouseButton.Left)
                Game.Swing();
            else
                // Game.player.Fire(coords);
                Game.player.StopBlocking();
        }

        private void DefaultHUDState_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButton.Right)
                Game.player.StartBlocking();
        }
    }
}
