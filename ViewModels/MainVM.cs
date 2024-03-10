using RSTinvestRefBook.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace RSTinvestRefBook.ViewModels
{
    internal class MainVM<T> :  INotifyPropertyChanged 
    {
        private ObservableCollection<T> items;
        public ObservableCollection<T> Items
        {
            get { return items; }
            set { items = value;  OnPropertyChanged(nameof(Items)); }
        }
        
        public MainVM()
        {
            Items = new ObservableCollection<T>();
            Items.CollectionChanged += Items_CollectionChanged;
        }
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Items));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

