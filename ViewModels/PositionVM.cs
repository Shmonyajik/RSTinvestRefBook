using RSTinvestRefBook.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace RSTinvestRefBook.ViewModels
{
    internal class PositionVM: INotifyPropertyChanged
    {
        private ObservableCollection<Position> items;
        public ObservableCollection<Position> Items
        {
            get { return items; }
            set { items = value; OnPropertyChanged(nameof(Items)); }
        }

        public PositionVM()
        {
            Items = new ObservableCollection<Position>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
