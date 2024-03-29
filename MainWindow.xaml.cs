﻿using RSTinvestRefBook.Models;
using RSTinvestRefBook.Repositories;
using RSTinvestRefBook.Services;
using RSTinvestRefBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
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
            if (dataGrid == null)
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
            var regexPattern = ConfigurationManager.AppSettings["HexIdRegexPattern"];
            var filePath = ConfigurationManager.AppSettings["RefBookFilePath"];
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            if (string.IsNullOrEmpty(regexPattern))
            {
                throw new ConfigurationErrorsException("HexIdRegexPattern не найден в файле конфигурации.");
            }
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ConfigurationErrorsException("RefBookFilePath не найден в файле конфигурации.");
            }
            if (!File.Exists(fullPath))
            {
                MessageBoxResult result = MessageBox.Show("Файл справочника не найден. Создать новый файл?", "RefBook.csv", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    string directoryPath = Path.GetDirectoryName(fullPath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    using (StreamWriter writer = new StreamWriter(fullPath, true))
                    {
                        writer.WriteLine("Id,HexId,Name");
                    }

                }
                else
                {
                    Environment.Exit(0);
                }
            }
            var response = await _refBookService.GetAllPositionsAsync();
            if(response.StatusCode!=Enums.StatusCode.OK)
            {
                MessageBox.Show(response.Description);
                return;
            }
            
            var positions = new ObservableCollection<Position>(response.Data);
            _positionVM.Items = positions;
        }
        private void AcceptShipmentGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null)
            {
                throw new ArgumentNullException(nameof(dataGrid), "DataGrid не должен быть null.");
            }
            if (dataGrid.Columns.Count > 0)
            {
                double columnWidth = dataGrid.ActualWidth / dataGrid.Columns.Count;
                foreach (var column in dataGrid.Columns)
                {
                    column.Width = new DataGridLength(columnWidth);
                }
            }
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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem selectedTab)
            {
  
                if (selectedTab.Header.ToString() == "Positions Moving")
                {
                    AcceptTextBox.Visibility = Visibility.Visible;
                    ShipmentTextBox.Visibility = Visibility.Visible;
                    ClearBtn.Visibility = Visibility.Visible;
                }
                else
                {
                
                    AcceptTextBox.Visibility = Visibility.Collapsed;
                    ShipmentTextBox.Visibility = Visibility.Collapsed;
                    ClearBtn.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Введите HEX")
            {
                textBox.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Введите HEX";
            }
        }
    }
}
