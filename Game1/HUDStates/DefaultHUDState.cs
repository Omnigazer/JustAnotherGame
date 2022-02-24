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
using Omniplatformer.Views.BasicControls;
using Omniplatformer.Views.Character;

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
            var col = new Column()
            {
                playerHUD
            };
            Root.RegisterChild(col);

            var logs = new LogView(Game.Logs)
            {
                Node = { PositionType = Facebook.Yoga.YogaPositionType.Absolute, Right = 50, Top = 200 }
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
                {  Keys.Pause, (Game.TogglePause, noop, false) },
                {  Keys.A, (Game.WalkLeft, noop, true) },
                {  Keys.D, (Game.WalkRight, noop, true) },
                {  Keys.S, (Game.Crouch, Game.Stand, false) },
                {  Keys.W, (Game.GoUp, noop, true) },
                {  Keys.Space, (Game.Jump, Game.StopJumping, false) },
                {  Keys.I, (Game.OpenInventory, noop, false) },
                {  Keys.Z, (Game.Fire, noop, false) },
                {  Keys.C, (Game.OpenChar, noop, false) },
                {  Keys.B, (Game.OpenCraft, noop, false) },
                {  Keys.E, (Game.Interact, noop, false) },
                {  Keys.OemMinus, (Game.ZoomOut, noop, true) },
                {  Keys.OemPlus, (Game.ZoomIn, noop, true) },
                {  Keys.OemCloseBrackets, (Game.ResetZoom, noop, true) },
                {  Keys.F11, (Game.OpenEditor, noop, false) },
                {  Keys.F12, (Game.RenderSystem.ToggleFullScreen, noop, false) },

                // Quick slots
                {  Keys.D1, (() => Game.SetCurrentQuickSlot(0), noop, false) },
                {  Keys.D2, (() => Game.SetCurrentQuickSlot(1), noop, false) },
                {  Keys.D3, (() => Game.SetCurrentQuickSlot(2), noop, false) },
                {  Keys.D4, (() => Game.SetCurrentQuickSlot(3), noop, false) },
                {  Keys.D5, (() => Game.SetCurrentQuickSlot(4), noop, false) },
                {  Keys.D6, (() => Game.SetCurrentQuickSlot(5), noop, false) },
                {  Keys.D7, (() => Game.SetCurrentQuickSlot(6), noop, false) },
                {  Keys.D8, (() => Game.SetCurrentQuickSlot(7), noop, false) },
                {  Keys.D9, (() => Game.SetCurrentQuickSlot(8), noop, false) },
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
