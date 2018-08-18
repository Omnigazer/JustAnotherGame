using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmptyKeys.UserInterface.Mvvm;

namespace GameData
{
    public class DragDropItem : BindableBase
    {
        private string name;
        private int rowindex;
        private int columnindex;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public int RowIndex
        {
            get { return rowindex; }
            set { SetProperty(ref rowindex, value); }
        }

        public int ColumnIndex
        {
            get { return columnindex; }
            set { SetProperty(ref columnindex, value); }
        }
    }
}