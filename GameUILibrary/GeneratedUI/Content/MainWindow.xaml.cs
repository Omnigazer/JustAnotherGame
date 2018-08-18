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
    public partial class MainWindow : UIRoot {
        
        private Grid e_0;
        
        private Grid e_1;
        
        private ProgressBar e_2;
        
        private TextBlock textBlock;
        
        private Button button;
        
        private Button button_Copy;
        
        private Button button_Copy1;
        
        private ListBox listBox;
        
        private ListBox listBox_Copy;
        
        private ItemsControl e_5;
        
        private Grid e_8;
        
        private TextBlock e_9;
        
        private TextBlock e_10;
        
        private TextBlock e_11;
        
        public MainWindow() : 
                base() {
            this.Initialize();
        }
        
        public MainWindow(int width, int height) : 
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
            this.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            InitializeElementResources(this);
            // e_0 element
            this.e_0 = new Grid();
            this.Content = this.e_0;
            this.e_0.Name = "e_0";
            // e_1 element
            this.e_1 = new Grid();
            this.e_0.Children.Add(this.e_1);
            this.e_1.Name = "e_1";
            this.e_1.Height = 497F;
            this.e_1.Width = 466F;
            this.e_1.Margin = new Thickness(298F, 86F, 0F, 0F);
            this.e_1.HorizontalAlignment = HorizontalAlignment.Left;
            this.e_1.VerticalAlignment = VerticalAlignment.Top;
            this.e_1.Background = new SolidColorBrush(new ColorW(240, 248, 255, 255));
            // e_2 element
            this.e_2 = new ProgressBar();
            this.e_1.Children.Add(this.e_2);
            this.e_2.Name = "e_2";
            this.e_2.Height = 10F;
            this.e_2.Width = 100F;
            this.e_2.Margin = new Thickness(48F, 76F, 0F, 0F);
            this.e_2.HorizontalAlignment = HorizontalAlignment.Left;
            this.e_2.VerticalAlignment = VerticalAlignment.Top;
            CustomEffect e_2_cef = new CustomEffect();
            e_2_cef.EffectAsset = "distorteffect";
            this.e_2.Effect = e_2_cef;
            this.e_2.BorderBrush = new SolidColorBrush(new ColorW(0, 0, 0, 255));
            ImageBrush e_2_Foreground = new ImageBrush();
            BitmapImage e_2_Foreground_bm = new BitmapImage();
            e_2_Foreground_bm.TextureAsset = "Textures/testliquid";
            e_2_Foreground.ImageSource = e_2_Foreground_bm;
            this.e_2.Foreground = e_2_Foreground;
            Binding binding_e_2_Value = new Binding("ItemCount");
            this.e_2.SetBinding(ProgressBar.ValueProperty, binding_e_2_Value);
            // textBlock element
            this.textBlock = new TextBlock();
            this.e_1.Children.Add(this.textBlock);
            this.textBlock.Name = "textBlock";
            this.textBlock.Margin = new Thickness(52F, 10F, 0F, 0F);
            this.textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            this.textBlock.VerticalAlignment = VerticalAlignment.Top;
            this.textBlock.Text = "Some text";
            this.textBlock.TextWrapping = TextWrapping.Wrap;
            // button element
            this.button = new Button();
            this.e_1.Children.Add(this.button);
            this.button.Name = "button";
            this.button.Height = 24F;
            this.button.Width = 24F;
            this.button.Margin = new Thickness(49F, 47F, 0F, 0F);
            this.button.HorizontalAlignment = HorizontalAlignment.Left;
            this.button.VerticalAlignment = VerticalAlignment.Top;
            this.button.Content = "";
            Binding binding_button_Command = new Binding("ButtonCommand");
            this.button.SetBinding(Button.CommandProperty, binding_button_Command);
            // button_Copy element
            this.button_Copy = new Button();
            this.e_1.Children.Add(this.button_Copy);
            this.button_Copy.Name = "button_Copy";
            this.button_Copy.Height = 24F;
            this.button_Copy.Width = 24F;
            this.button_Copy.Margin = new Thickness(78F, 47F, 0F, 0F);
            this.button_Copy.HorizontalAlignment = HorizontalAlignment.Left;
            this.button_Copy.VerticalAlignment = VerticalAlignment.Top;
            this.button_Copy.Content = "";
            // button_Copy1 element
            this.button_Copy1 = new Button();
            this.e_1.Children.Add(this.button_Copy1);
            this.button_Copy1.Name = "button_Copy1";
            this.button_Copy1.Height = 24F;
            this.button_Copy1.Width = 24F;
            this.button_Copy1.Margin = new Thickness(107F, 47F, 0F, 0F);
            this.button_Copy1.HorizontalAlignment = HorizontalAlignment.Left;
            this.button_Copy1.VerticalAlignment = VerticalAlignment.Top;
            this.button_Copy1.Content = "";
            // listBox element
            this.listBox = new ListBox();
            this.e_1.Children.Add(this.listBox);
            this.listBox.Name = "listBox";
            this.listBox.Height = 131F;
            this.listBox.Width = 100F;
            this.listBox.Margin = new Thickness(48F, 91F, 0F, 0F);
            this.listBox.HorizontalAlignment = HorizontalAlignment.Left;
            this.listBox.VerticalAlignment = VerticalAlignment.Top;
            EventTrigger listBox_ET_0 = new EventTrigger(ListBox.MouseDownEvent, this.listBox);
            listBox.Triggers.Add(listBox_ET_0);
            EventTrigger.SetCommandPath(listBox_ET_0, "ButtonCommand");
            this.listBox.BorderBrush = new SolidColorBrush(new ColorW(53, 105, 197, 255));
            this.listBox.BorderThickness = new Thickness(4F, 4F, 4F, 4F);
            this.listBox.ItemsSource = Get_listBox_Items();
            DragDrop.SetIsDragSource(this.listBox, true);
            DragDrop.SetIsDropTarget(this.listBox, true);
            Binding binding_listBox_ItemsSource = new Binding("ListBoxData1");
            this.listBox.SetBinding(ListBox.ItemsSourceProperty, binding_listBox_ItemsSource);
            // listBox_Copy element
            this.listBox_Copy = new ListBox();
            this.e_1.Children.Add(this.listBox_Copy);
            this.listBox_Copy.Name = "listBox_Copy";
            this.listBox_Copy.Height = 141F;
            this.listBox_Copy.Width = 109F;
            this.listBox_Copy.Margin = new Thickness(173F, 91F, 0F, 0F);
            this.listBox_Copy.HorizontalAlignment = HorizontalAlignment.Left;
            this.listBox_Copy.VerticalAlignment = VerticalAlignment.Top;
            this.listBox_Copy.BorderBrush = new SolidColorBrush(new ColorW(79, 123, 203, 255));
            this.listBox_Copy.BorderThickness = new Thickness(4F, 4F, 4F, 4F);
            this.listBox_Copy.ItemsSource = Get_listBox_Copy_Items();
            DragDrop.SetIsDragSource(this.listBox_Copy, true);
            DragDrop.SetIsDropTarget(this.listBox_Copy, true);
            Binding binding_listBox_Copy_ItemsSource = new Binding("ListBoxData2");
            this.listBox_Copy.SetBinding(ListBox.ItemsSourceProperty, binding_listBox_Copy_ItemsSource);
            // e_5 element
            this.e_5 = new ItemsControl();
            this.e_1.Children.Add(this.e_5);
            this.e_5.Name = "e_5";
            this.e_5.Height = 240F;
            this.e_5.Width = 240F;
            this.e_5.Margin = new Thickness(125F, 247F, 0F, 0F);
            this.e_5.HorizontalAlignment = HorizontalAlignment.Left;
            Func<UIElement, UIElement> e_5_iptFunc = e_5_iptMethod;
            ControlTemplate e_5_ipt = new ControlTemplate(e_5_iptFunc);
            this.e_5.ItemsPanel = e_5_ipt;
            Func<UIElement, UIElement> e_5_dtFunc = e_5_dtMethod;
            this.e_5.ItemTemplate = new DataTemplate(e_5_dtFunc);
            Binding binding_e_5_ItemsSource = new Binding("InventoryData");
            this.e_5.SetBinding(ItemsControl.ItemsSourceProperty, binding_e_5_ItemsSource);
            // e_8 element
            this.e_8 = new Grid();
            this.e_1.Children.Add(this.e_8);
            this.e_8.Name = "e_8";
            this.e_8.Height = 100F;
            this.e_8.Width = 100F;
            this.e_8.Margin = new Thickness(338F, 117F, 0F, 0F);
            this.e_8.HorizontalAlignment = HorizontalAlignment.Left;
            this.e_8.VerticalAlignment = VerticalAlignment.Top;
            RowDefinition row_e_8_0 = new RowDefinition();
            row_e_8_0.Height = new GridLength(1F, GridUnitType.Star);
            this.e_8.RowDefinitions.Add(row_e_8_0);
            RowDefinition row_e_8_1 = new RowDefinition();
            row_e_8_1.Height = new GridLength(1F, GridUnitType.Star);
            this.e_8.RowDefinitions.Add(row_e_8_1);
            RowDefinition row_e_8_2 = new RowDefinition();
            row_e_8_2.Height = new GridLength(1F, GridUnitType.Star);
            this.e_8.RowDefinitions.Add(row_e_8_2);
            RowDefinition row_e_8_3 = new RowDefinition();
            row_e_8_3.Height = new GridLength(1F, GridUnitType.Star);
            this.e_8.RowDefinitions.Add(row_e_8_3);
            ColumnDefinition col_e_8_0 = new ColumnDefinition();
            col_e_8_0.Width = new GridLength(1F, GridUnitType.Star);
            this.e_8.ColumnDefinitions.Add(col_e_8_0);
            ColumnDefinition col_e_8_1 = new ColumnDefinition();
            col_e_8_1.Width = new GridLength(1F, GridUnitType.Star);
            this.e_8.ColumnDefinitions.Add(col_e_8_1);
            ColumnDefinition col_e_8_2 = new ColumnDefinition();
            col_e_8_2.Width = new GridLength(1F, GridUnitType.Star);
            this.e_8.ColumnDefinitions.Add(col_e_8_2);
            ColumnDefinition col_e_8_3 = new ColumnDefinition();
            col_e_8_3.Width = new GridLength(1F, GridUnitType.Star);
            this.e_8.ColumnDefinitions.Add(col_e_8_3);
            // e_9 element
            this.e_9 = new TextBlock();
            this.e_8.Children.Add(this.e_9);
            this.e_9.Name = "e_9";
            this.e_9.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_9.VerticalAlignment = VerticalAlignment.Center;
            this.e_9.Text = "A";
            Grid.SetColumn(this.e_9, 0);
            Grid.SetRow(this.e_9, 0);
            // e_10 element
            this.e_10 = new TextBlock();
            this.e_8.Children.Add(this.e_10);
            this.e_10.Name = "e_10";
            this.e_10.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_10.VerticalAlignment = VerticalAlignment.Center;
            this.e_10.Text = "B";
            Grid.SetColumn(this.e_10, 1);
            Grid.SetRow(this.e_10, 1);
            // e_11 element
            this.e_11 = new TextBlock();
            this.e_8.Children.Add(this.e_11);
            this.e_11.Name = "e_11";
            this.e_11.HorizontalAlignment = HorizontalAlignment.Center;
            this.e_11.VerticalAlignment = VerticalAlignment.Center;
            this.e_11.Text = "C";
            Grid.SetColumn(this.e_11, 2);
            Grid.SetRow(this.e_11, 2);
            ImageManager.Instance.AddImage("Textures/testliquid");
            EffectManager.Instance.AddEffect("distorteffect");
        }
        
        private static void InitializeElementResources(UIElement elem) {
            elem.Resources.MergedDictionaries.Add(Dictionary.Instance);
        }
        
        private static System.Collections.ObjectModel.ObservableCollection<object> Get_listBox_Items() {
            System.Collections.ObjectModel.ObservableCollection<object> items = new System.Collections.ObjectModel.ObservableCollection<object>();
            // e_3 element
            ListBoxItem e_3 = new ListBoxItem();
            e_3.Name = "e_3";
            e_3.Content = "CUNTENT";
            items.Add(e_3);
            return items;
        }
        
        private static System.Collections.ObjectModel.ObservableCollection<object> Get_listBox_Copy_Items() {
            System.Collections.ObjectModel.ObservableCollection<object> items = new System.Collections.ObjectModel.ObservableCollection<object>();
            // e_4 element
            ListBoxItem e_4 = new ListBoxItem();
            e_4.Name = "e_4";
            e_4.Content = "ASDASD";
            items.Add(e_4);
            return items;
        }
        
        private static UIElement e_5_iptMethod(UIElement parent) {
            // e_6 element
            Grid e_6 = new Grid();
            e_6.Parent = parent;
            e_6.Name = "e_6";
            e_6.Height = 240F;
            e_6.Width = 240F;
            e_6.HorizontalAlignment = HorizontalAlignment.Left;
            e_6.VerticalAlignment = VerticalAlignment.Top;
            e_6.Background = new SolidColorBrush(new ColorW(243, 25, 25, 0));
            e_6.IsItemsHost = true;
            RowDefinition row_e_6_0 = new RowDefinition();
            row_e_6_0.Height = new GridLength(1F, GridUnitType.Star);
            e_6.RowDefinitions.Add(row_e_6_0);
            RowDefinition row_e_6_1 = new RowDefinition();
            row_e_6_1.Height = new GridLength(1F, GridUnitType.Star);
            e_6.RowDefinitions.Add(row_e_6_1);
            RowDefinition row_e_6_2 = new RowDefinition();
            row_e_6_2.Height = new GridLength(1F, GridUnitType.Star);
            e_6.RowDefinitions.Add(row_e_6_2);
            RowDefinition row_e_6_3 = new RowDefinition();
            row_e_6_3.Height = new GridLength(1F, GridUnitType.Star);
            e_6.RowDefinitions.Add(row_e_6_3);
            ColumnDefinition col_e_6_0 = new ColumnDefinition();
            col_e_6_0.Width = new GridLength(1F, GridUnitType.Star);
            e_6.ColumnDefinitions.Add(col_e_6_0);
            ColumnDefinition col_e_6_1 = new ColumnDefinition();
            col_e_6_1.Width = new GridLength(1F, GridUnitType.Star);
            e_6.ColumnDefinitions.Add(col_e_6_1);
            ColumnDefinition col_e_6_2 = new ColumnDefinition();
            col_e_6_2.Width = new GridLength(1F, GridUnitType.Star);
            e_6.ColumnDefinitions.Add(col_e_6_2);
            ColumnDefinition col_e_6_3 = new ColumnDefinition();
            col_e_6_3.Width = new GridLength(1F, GridUnitType.Star);
            e_6.ColumnDefinitions.Add(col_e_6_3);
            return e_6;
        }
        
        private static UIElement e_5_dtMethod(UIElement parent) {
            // e_7 element
            TextBlock e_7 = new TextBlock();
            e_7.Parent = parent;
            e_7.Name = "e_7";
            e_7.HorizontalAlignment = HorizontalAlignment.Center;
            e_7.VerticalAlignment = VerticalAlignment.Center;
            e_7.Background = new SolidColorBrush(new ColorW(1, 255, 255, 25));
            Grid.SetColumn(e_7, 1);
            Grid.SetRow(e_7, 2);
            Binding binding_e_7_Text = new Binding("Name");
            e_7.SetBinding(TextBlock.TextProperty, binding_e_7_Text);
            return e_7;
        }
    }
}
