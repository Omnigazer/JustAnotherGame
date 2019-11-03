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

namespace Omniplatformer.HUDStates
{
    public class CharHUDState : HUDState
    {
        HUDContainer playerHUD;
        CharView view;

        public CharHUDState()
        {
            playerHUD = new HUDContainer();
            view = new CharView();
            SetupControls();
            var col = new Column()
            {
                playerHUD
            };
            //col.RegisterChild();
            Root.RegisterChild(col);
            // Root.RegisterChild(playerHUD);
            col = new Column()
            {
                view
            };
            Root.RegisterChild(col);
            // Root.RegisterChild(view);
            SetupGUI();
        }

        public void SetupControls()
        {
            Action noop = delegate { };
            Controls = new Dictionary<Keys, (Action, Action, bool)>()
            {
                {  Keys.C, (Game.CloseChar, noop, false) }
            };
        }
    }
}
