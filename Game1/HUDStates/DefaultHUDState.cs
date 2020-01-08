using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Views.HUD;

namespace Omniplatformer.HUDStates
{
    public class DefaultHUDState : HUDState
    {
        HUDContainer playerHUD;

        public DefaultHUDState()
        {
            playerHUD = new HUDContainer();
            Root.MouseDown += onMouseDown;
            Root.MouseUp += onMouseUp;
            SetupControls();
            SetupGUI();
        }

        public override void RegisterChildren()
        {
            Root.RegisterChild(playerHUD);
            var logs = new LogView(Game.Logs)
            {
                Node = {PositionType = Facebook.Yoga.YogaPositionType.Absolute, Right = 50, Top = 200}
            };
            Root.RegisterChild(logs);
        }

        public override IEnumerable<string> GetStatusMessages()
        {
            yield return $"Current object: {Game.GetObjectAtCursor()}";
            foreach (var msg in StatusMessages)
            {
                yield return msg;
            }
            StatusMessages.Clear();
        }

        public void SetupControls()
        {
            static void noop() { }

            Controls = new Dictionary<Keys, (Action, Action, bool)>()
            {
                {  Keys.A, (Game.WalkLeft, noop, true) },
                {  Keys.D, (Game.WalkRight, noop, true) },
                {  Keys.S, (Game.Duck, Game.Stand, false) },
                {  Keys.W, (Game.GoUp, noop, true) },
                {  Keys.Space, (Game.Jump, Game.StopJumping, false) },
                {  Keys.I, (Game.OpenInventory, noop, false) },
                {  Keys.Z, (Game.Fire, noop, false) },
                {  Keys.C, (Game.OpenChar, noop, false) },
                {  Keys.E, (Game.OpenChest, noop, false) },
                {  Keys.OemMinus, (Game.ZoomOut, noop, true) },
                {  Keys.OemPlus, (Game.ZoomIn, noop, true) },
                {  Keys.OemCloseBrackets, (Game.ResetZoom, noop, true) },
                {  Keys.F11, (Game.OpenEditor, noop, false) },
                {  Keys.F12, (Game.RenderSystem.ToggleFullScreen, noop, false) }
            };
        }

        private void onMouseUp(object sender, MouseEventArgs e)
        {
            Game.PerformMouseAction(e, false);
        }

        private void onMouseDown(object sender, MouseEventArgs e)
        {
            Game.PerformMouseAction(e, true);
        }
    }
}
