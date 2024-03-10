using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

