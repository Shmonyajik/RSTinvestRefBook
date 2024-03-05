using RSTinvestRefBook.Models;
using RSTinvestRefBook.Repositories;
using RSTinvestRefBook.Services;
using RSTinvestRefBook.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RSTinvestRefBook
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRefBookService _refBookService;
        private PositionVM _positionVM;
        public MainWindow() : base()
        {
            InitializeComponent();
        }
        public MainWindow(IRefBookService refBookService ) : this()
        {
            _refBookService = refBookService;
            InitializeComponent();
        }

        private async void EditRecords_Click(object sender, RoutedEventArgs e)
        {
            var positions = new List<Position>(_positionVM.Items);
            var response = await _refBookService.EditPositionsList(positions);
            if(response.StatusCode!=Enums.StatusCode.OK)
            {
                MessageBox.Show(response.Description);
                
            }
            else
            {
                MessageBox.Show("Успешно сохранено");
            }
        }

        private void DeleteSelectedRecords_Click(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = MainTabControl.FindName("RefBookGrid") as DataGrid;

           var selectedItems = dataGrid.SelectedItems;
            var itemsToDelete = new List<Position>();

            foreach (var selectedItem in selectedItems)
            {
                itemsToDelete.Add(selectedItem as Position);
            }

            foreach (var itemToDelete in itemsToDelete)
            {
                _positionVM.Items.Remove(itemToDelete);
            }

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var response = await _refBookService.GetAllPositions();
            if(response.StatusCode!=Enums.StatusCode.OK)
            {
                MessageBox.Show(response.Description);
                return;
            }
            var positions = new ObservableCollection<Position>(response.Data);
            _positionVM = new PositionVM
            {
                Items = positions
            };
            DataGrid refBookTable = MainTabControl.FindName("RefBookGrid") as DataGrid;
            refBookTable.DataContext = _positionVM;
        }

        
    }
}
