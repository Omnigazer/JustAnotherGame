using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Omniplatformer.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUDStates
{
    public class CharHUDState : HUDState
    {
        HUDContainer playerHUD;
        CharView view;
        // InventoryView PlayerInventoryView { get; set; }
        // InventoryView TargetInventoryView { get; set; }

        // TODO: TEMPORARY
        // Inventory inv;

        public CharHUDState(HUDContainer hud)
        {
            playerHUD = hud;
            view = new CharView();
            // PlayerInventoryView = new InventoryView(inv, false);
            // TODO: remove this reference
            // this.inv = inv;
            SetupControls();
            MouseUp += OnMouseUp;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                var mouse = Mouse.GetState();
                var slot = view.GetSlotAtPosition(mouse.Position);
                if (slot != null)
                {
                    Game.Player.UpgradeSkill(slot.Skill);
                }
            }
        }

        public override void Draw()
        {
            view.Draw();
            playerHUD.Draw();
            base.Draw();
        }

        public void SetupControls()
        {
            Action noop = delegate { };
            Controls = new Dictionary<Keys, (Action, Action, bool)>()
            {
                // {  Keys.X, (Game.player.WieldCurrentSlot, noop, false) },
                // {  Keys.C, (Game.CloseChest, noop, false) },
                {  Keys.Escape, (Game.CloseChar, noop, false) }
            };
        }

        public override void HandleControls()
        {
            Game.StopMoving();
            base.HandleControls();
        }
    }
}
