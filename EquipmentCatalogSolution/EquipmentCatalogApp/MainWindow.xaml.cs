using EquipmentCatalogLibrary;
using System;
using System.Collections.Generic;
using System.Windows;

namespace EquipmentCatalogApp
{
    public partial class MainWindow : Window
    {
        private readonly EquipmentCatalogManager _manager;
        private List<Equipment> _equipments;

        public MainWindow()
        {
            InitializeComponent();
            _manager = new EquipmentCatalogManager("equipment.xml", "equipment.xsd");
            _equipments = new List<Equipment>();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _equipments = _manager.LoadCatalog();
                EquipmentGrid.ItemsSource = _equipments;
                StatusText.Text = "Данные успешно загружены";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Ошибка загрузки";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _manager.SaveCatalog(_equipments);
                StatusText.Text = "Данные успешно сохранены";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Ошибка сохранения";
            }
        }
    }
}