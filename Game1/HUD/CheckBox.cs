﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    public class CheckBox : ViewControl
    {
        public bool IsChecked { get; set; }
        public CheckBox()
        {
            Node.FlexDirection = Facebook.Yoga.YogaFlexDirection.Row;
        }

        public override void SetupNode()
        {
            Width = 30;
            Height = 30;
            BorderThickness = 1;
            MouseClick += onMouseClick;
        }

        private void onMouseClick(object sender, HUDStates.MouseEventArgs e)
        {
            IsChecked = !IsChecked;
        }

        public override void DrawSelf()
        {
            DrawBorder(Color.Black);
            var spriteBatch = GraphicsService.Instance;
            var color = IsChecked ? Color.Green : Color.Transparent;
            spriteBatch.Draw(GameContent.Instance.whitePixel, GlobalRect, color);
        }
    }
}
