using RSTinvestRefBook.Models;
using RSTinvestRefBook.Repositories;
using RSTinvestRefBook.Services;
using RSTinvestRefBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
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
        private MainVM<Position> _positionVM;
        private MainVM<ShipmentAcceptance> _acceptanceVM;
        private MainVM<ShipmentAcceptance> _shipmentVM;
        public MainWindow() : base()
        {
            InitializeComponent();
        }
        public MainWindow(IRefBookService refBookService ) : this()
        {
            InitializeComponent();
            _refBookService = refBookService;
            InitializeMVVM();
        }

        private async void EditRecords_Click(object sender, RoutedEventArgs e)
        {
            var positions = new List<Position>(_positionVM.Items);
            var response = await _refBookService.EditPositionsListAsync(positions);
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
            if (dataGrid != null)
            {
                throw new ArgumentNullException(nameof(dataGrid), "DataGrid не должен быть null.");
            }
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
            var response = await _refBookService.GetAllPositionsAsync();
            if(response.StatusCode!=Enums.StatusCode.OK)
            {
                MessageBox.Show(response.Description);
                return;
            }
            
            var positions = new ObservableCollection<Position>(response.Data);
            _positionVM.Items = positions;
        }

        private async void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                TextBox textBox = sender as TextBox;

                if (textBox == null)
                {
                    throw new ArgumentNullException(nameof(textBox), "TextBox не должен быть null.");
                }
                if (string.IsNullOrWhiteSpace(textBox.Text) )
                {
                    MessageBox.Show("Введите HEX идентификатор.");
                    return;
                }

                var hexIds = textBox.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var response = await _refBookService.GetPositionsByHexIdsAsync(hexIds);

                if(response.StatusCode!=Enums.StatusCode.OK)
                {
                    MessageBox.Show(response.Description);
                    return;
                }
                foreach( var position in response.Data)
                {
                    FillTable(position, textBox.Name);
                }

            }
        }

        private void FillTable (Position position, string name)
        {
            var acceptItem = _acceptanceVM.Items.FirstOrDefault(item => item.Name == position.Name);
            var shipmentItem = _shipmentVM.Items.FirstOrDefault(item => item.Name == position.Name);

            if (name == "AcceptTextBox")
            {
                
                if (acceptItem == null)
                {
                    if(shipmentItem == null)//нет ни где
                    {
                        _acceptanceVM.Items.Add(new ShipmentAcceptance { Name = position.Name, Quantity = 1 });
                    }
                    else//есть в шипмент
                    {
                        if (shipmentItem.Quantity == 1)
                            _shipmentVM.Items.Remove(shipmentItem);
                        else
                            shipmentItem.Quantity--;

                        _acceptanceVM.Items.Add(new ShipmentAcceptance { Name = position.Name, Quantity = 1 });
                    }
                }
                else
                {
                    if(shipmentItem==null)//есть в аксептенс
                    {
                        acceptItem.Quantity++;
                    }
                    else//есть и там и там
                    {
                        if (shipmentItem.Quantity == 1)
                            _shipmentVM.Items.Remove(shipmentItem);
                        else
                            shipmentItem.Quantity--;

                        acceptItem.Quantity++;
                    }
                }
                
            }
            else if(name == "ShipmentTextBox")
            {
                if(shipmentItem == null)
                {
                    if(acceptItem == null)//нет ни где
                    {
                        _shipmentVM.Items.Add(new ShipmentAcceptance { Name = position.Name, Quantity = 1 });
                    }
                    else//есть в аксептанс
                    {
                        if (acceptItem.Quantity == 1)
                            _acceptanceVM.Items.Remove(acceptItem);
                        else
                            acceptItem.Quantity--;

                        _shipmentVM.Items.Add(new ShipmentAcceptance { Name = position.Name, Quantity = 1 });
                    }
                }
                else
                {
                    if (acceptItem == null)//есть в шипмент
                    {
                        shipmentItem.Quantity++;
                    }
                    else//есть и там и там
                    {
                        if (acceptItem.Quantity == 1)
                            _acceptanceVM.Items.Remove(acceptItem);
                        else
                            acceptItem.Quantity--;

                        shipmentItem.Quantity++;
                    }
                }
                
            }
            AcceptGrid.Items.Refresh();
            ShipmentGrid.Items.Refresh();
        }

        private void InitializeMVVM()
        {
            _positionVM = new MainVM<Position>();
            _acceptanceVM = new MainVM<ShipmentAcceptance>();
            _shipmentVM = new MainVM<ShipmentAcceptance>();

            RefBookGrid.DataContext = _positionVM;
            AcceptGrid.DataContext = _acceptanceVM;
            ShipmentGrid.DataContext = _shipmentVM;

        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _shipmentVM.Items.Clear();
            _acceptanceVM.Items.Clear();
        }
    }
}
