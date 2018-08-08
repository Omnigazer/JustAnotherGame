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
    public partial class MainWindow : UserControl {

        private Grid e_0;

        private RadioButton radioButton;

        private RadioButton radioButton1;

        private ProgressBar e_1;

        public MainWindow() {
            Style style = UserControlStyle.CreateUserControlStyle();
            style.TargetType = this.GetType();
            this.Style = style;
            this.InitializeComponent();
        }

        private void InitializeComponent() {
            // e_0 element
            this.e_0 = new Grid();
            this.Content = this.e_0;
            this.e_0.Name = "e_0";
            ColumnDefinition col_e_0_0 = new ColumnDefinition();
            col_e_0_0.Width = new GridLength(34F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_0);
            ColumnDefinition col_e_0_1 = new ColumnDefinition();
            col_e_0_1.Width = new GridLength(191F, GridUnitType.Star);
            this.e_0.ColumnDefinitions.Add(col_e_0_1);
            // radioButton element
            this.radioButton = new RadioButton();
            this.e_0.Children.Add(this.radioButton);
            this.radioButton.Name = "radioButton";
            this.radioButton.Margin = new Thickness(10F, 4F, 0F, 0F);
            this.radioButton.HorizontalAlignment = HorizontalAlignment.Left;
            this.radioButton.VerticalAlignment = VerticalAlignment.Top;
            this.radioButton.Content = "One";
            this.radioButton.IsChecked = false;
            Grid.SetColumnSpan(this.radioButton, 2);
            // radioButton1 element
            this.radioButton1 = new RadioButton();
            this.e_0.Children.Add(this.radioButton1);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Margin = new Thickness(10F, 25F, 0F, 0F);
            this.radioButton1.HorizontalAlignment = HorizontalAlignment.Left;
            this.radioButton1.VerticalAlignment = VerticalAlignment.Top;
            this.radioButton1.Content = "Two";
            this.radioButton1.IsChecked = true;
            Grid.SetColumnSpan(this.radioButton1, 2);
            // e_1 element
            this.e_1 = new ProgressBar();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            this.e_1.Height = 10F;
            this.e_1.Width = 100F;
            this.e_1.Margin = new Thickness(10F, 46F, 0F, 0F);
            this.e_1.HorizontalAlignment = HorizontalAlignment.Left;
            this.e_1.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetColumnSpan(this.e_1, 2);
            Binding binding_e_1_Value = new Binding("ItemCount");
            this.e_1.SetBinding(ProgressBar.ValueProperty, binding_e_1_Value);
        }
    }
}
