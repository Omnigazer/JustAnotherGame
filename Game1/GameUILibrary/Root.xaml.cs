// -----------------------------------------------------------
//  
//  This file was generated, please do not modify.
//  
// -----------------------------------------------------------
namespace EmptyKeys.UserInterface.Generated {
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using EmptyKeys.UserInterface;
    using EmptyKeys.UserInterface.Charts;
    using EmptyKeys.UserInterface.Data;
    using EmptyKeys.UserInterface.Controls;
    using EmptyKeys.UserInterface.Controls.Primitives;
    using EmptyKeys.UserInterface.Input;
    using EmptyKeys.UserInterface.Interactions.Core;
    using EmptyKeys.UserInterface.Interactivity;
    using EmptyKeys.UserInterface.Media;
    using EmptyKeys.UserInterface.Media.Effects;
    using EmptyKeys.UserInterface.Media.Animation;
    using EmptyKeys.UserInterface.Media.Imaging;
    using EmptyKeys.UserInterface.Shapes;
    using EmptyKeys.UserInterface.Renderers;
    using EmptyKeys.UserInterface.Themes;
    
    
    [GeneratedCodeAttribute("Empty Keys UI Generator", "3.0.0.0")]
    public partial class Root : UIRoot {
        
        private Grid e_0;
        
        private TextBlock e_1;
        
        private Button button;
        
        private RadioButton radioButton;
        
        private RadioButton radioButton_Copy;
        
        public Root() : 
                base() {
            this.Initialize();
        }
        
        public Root(int width, int height) : 
                base(width, height) {
            this.Initialize();
        }
        
        private void Initialize() {
            Style style = RootStyle.CreateRootStyle();
            style.TargetType = this.GetType();
            this.Style = style;
            this.InitializeComponent();
        }
        
        private void InitializeComponent() {
            // e_0 element
            this.e_0 = new Grid();
            this.Content = this.e_0;
            this.e_0.Name = "e_0";
            // e_1 element
            this.e_1 = new TextBlock();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            this.e_1.Margin = new Thickness(10F, 10F, 676F, 603F);
            this.e_1.Text = "Hello World";
            this.e_1.FontSize = 20F;
            this.e_1.FontStyle = FontStyle.Bold;
            // button element
            this.button = new Button();
            this.e_0.Children.Add(this.button);
            this.button.Name = "button";
            this.button.Width = 75F;
            this.button.Margin = new Thickness(150F, 152F, 0F, 0F);
            this.button.HorizontalAlignment = HorizontalAlignment.Left;
            this.button.VerticalAlignment = VerticalAlignment.Top;
            this.button.Content = "Button";
            // radioButton element
            this.radioButton = new RadioButton();
            this.e_0.Children.Add(this.radioButton);
            this.radioButton.Name = "radioButton";
            this.radioButton.Margin = new Thickness(137F, 197F, 0F, 0F);
            this.radioButton.HorizontalAlignment = HorizontalAlignment.Left;
            this.radioButton.VerticalAlignment = VerticalAlignment.Top;
            this.radioButton.Content = "RadioButton";
            // radioButton_Copy element
            this.radioButton_Copy = new RadioButton();
            this.e_0.Children.Add(this.radioButton_Copy);
            this.radioButton_Copy.Name = "radioButton_Copy";
            this.radioButton_Copy.Margin = new Thickness(137F, 218F, 0F, 0F);
            this.radioButton_Copy.HorizontalAlignment = HorizontalAlignment.Left;
            this.radioButton_Copy.VerticalAlignment = VerticalAlignment.Top;
            this.radioButton_Copy.Content = "RadioButton";
            FontManager.Instance.AddFont("Segoe UI", 20F, FontStyle.Bold, "Segoe_UI_15_Bold");
        }
    }
}
