using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Views.BasicControls;
using Omniplatformer.Views.Character;
using Omniplatformer.Views.HUD;
using Omniplatformer.Views.InventoryNS;
using Omniplatformer.Objects.Items;
using Omniplatformer.Objects.InventoryNS;

namespace Omniplatformer.HUDStates
{
    public class CraftHUDState : HUDState, IInventoryController
    {
        HUDContainer playerHUD;
        InventoryView PlayerInventoryView { get; set; }
        CraftView view;

        private Inventory PlayerInventory { get; set; }

        public CraftHUDState(Inventory playerInventory)
        {
            playerHUD = new HUDContainer();
            this.PlayerInventory = playerInventory;
            view = new CraftView();
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
            col = new Column()
            {
                view
            };
            Root.RegisterChild(col);

            PlayerInventoryView = new InventoryView(this, PlayerInventory)
            {
                Node =
                {
                    Top = 20,
                    Right = 20,
                    PositionType = Facebook.Yoga.YogaPositionType.Absolute,
                    FlexDirection = Facebook.Yoga.YogaFlexDirection.Row,
                    Wrap = Facebook.Yoga.YogaWrap.Wrap
                }
            };

            Root.RegisterChild(PlayerInventoryView);
        }

        public void SetupControls()
        {
            Action noop = delegate { };
            Controls = new Dictionary<Keys, (Action, Action, bool)>()
            {
                {  Keys.Escape, (Game.SetDefaultHUD, noop, false) },
                {  Keys.B, (Game.SetDefaultHUD, noop, false) }
            };
        }

        public void OnSlotLeftClick(ItemSlot slot)
        {
        }

        public void OnSlotRightClick(ItemSlot slot)
        {
        }
    }
}
