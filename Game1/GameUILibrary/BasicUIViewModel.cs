using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Controls;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Media.Imaging;
using EmptyKeys.UserInterface.Mvvm;
using GameData;

namespace GameUILibrary
{
    /// <summary>
    /// Example of MVVM View Model
    /// </summary>
    public class BasicUIViewModel : ViewModelBase
    {
        private int item_count;
        private ObservableCollection<DragDropItem> listBoxData1 = new ObservableCollection<DragDropItem>();
        private ObservableCollection<DragDropItem> listBoxData2 = new ObservableCollection<DragDropItem>();
        private ObservableCollection<DragDropItem> listBoxData3 = new ObservableCollection<DragDropItem>();
        private ObservableCollection<DragDropItem> inventoryData = new ObservableCollection<DragDropItem>();
        private int internal_counter = 0;
        public int ItemCount { get => item_count; set => SetProperty(ref item_count, value); }

        public ObservableCollection<DragDropItem> ListBoxData1
        {
            get => listBoxData1; set => SetProperty(ref listBoxData1, value);
        }

        public ObservableCollection<DragDropItem> ListBoxData2
        {
            get => listBoxData2; set => SetProperty(ref listBoxData2, value);
        }

        public ObservableCollection<DragDropItem> ListBoxData3
        {
            get => listBoxData3; set => SetProperty(ref listBoxData3, value);
        }

        public ObservableCollection<DragDropItem> InventoryData
        {
            get => inventoryData; set => SetProperty(ref inventoryData, value);
        }

        public ICommand ButtonCommand { get; set; }

        public BasicUIViewModel()
        {
            ButtonCommand = new RelayCommand(new Action<object>(OnButtonClick));
            ListBoxData1.Add(new DragDropItem() { Name = "AAAAAAAAAA" });
            ListBoxData2.Add(new DragDropItem() { Name = "BBBBBBBBBB" });
            ListBoxData3.Add(new DragDropItem() { Name = "CCCCCCCCCC" });
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    InventoryData.Add(new DragDropItem() { Name = "ASS", RowIndex = i, ColumnIndex = j });
                }
            }
        }

        public void OnButtonClick(object param)
        {
            ItemCount = (ItemCount + 1) % 50;
        }

        public void Update(double elapsedTime)
        {
            return;
            internal_counter = (internal_counter + 1) % 2;
            if (internal_counter == 0)
                ItemCount = (ItemCount + 1) % 50;
        }
    }
}