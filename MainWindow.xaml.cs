using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;


namespace lab5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Catalog myCatalog;
        //ICollectionView collectionView;

        public MainWindow()
        {

            InitializeComponent();
            myCatalog = new Catalog();

             // Установка списка в качестве источника данных для ListView
            myListView.ItemsSource = myCatalog.compositions;
            
            // Добавление элементов в ComboBox
            myComboBox.Items.Add("Автор");
            myComboBox.Items.Add("Название");
            myComboBox.SelectedIndex = 0;
        }

        private void LoadJSON_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            myCatalog.compositions.Clear();
            myLabel.Content = myCatalog.KeepData_JSON(false);
            CollectionViewSource.GetDefaultView(myListView.ItemsSource).Refresh();
        }
        private void LoadXML_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            myCatalog.compositions.Clear();
            myLabel.Content = myCatalog.KeepData_XML(false);
            myListView.ItemsSource = myCatalog.compositions;
            CollectionViewSource.GetDefaultView(myListView.ItemsSource).Refresh();
        }
        private void LoadSQL_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            myCatalog.compositions.Clear();
            myLabel.Content = myCatalog.KeepData_SQL(false);
            CollectionViewSource.GetDefaultView(myListView.ItemsSource).Refresh();
        }
        private void SaveJSON_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            myLabel.Content = myCatalog.KeepData_JSON(true);
        }
        private void SaveXML_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            myLabel.Content = myCatalog.KeepData_XML(true);
        }
        private void SaveSQL_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            myLabel.Content = myCatalog.KeepData_SQL(true);
        }
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Создание и вызов диалогового окна
            Window1 dialogWindow = new Window1();
            bool? result = dialogWindow.ShowDialog();
            if (result == true)     // Нажата кнопка "OK"
            {
                //Добавление композиции в каталог
                myCatalog.compositions.Add(new Composition(dialogWindow.author, dialogWindow.song));
                CollectionViewSource.GetDefaultView(myListView.ItemsSource).Refresh();
                myLabel.Content = "Композиция добавлена в каталог";
            }
        }
        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            if (myListView.SelectedItem != null)
            {
                Composition item = (Composition)myListView.SelectedItem;
                myCatalog.compositions.Remove(item);
                CollectionViewSource.GetDefaultView(myListView.ItemsSource).Refresh();
                myLabel.Content = "Композиция удалена из каталога";
            }
            else
            {
                myLabel.Content = "Композиция для удаления из каталога не выбрана!";
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            string newPath;

            if (myStackPanel.IsVisible)
            {
                myStackPanel.Visibility = Visibility.Hidden;
                newPath = "component/FilterOn.png";

                // Получение ICollectionView и сброс фильтра
                ICollectionView collectionView = CollectionViewSource.GetDefaultView(myListView.ItemsSource);
                collectionView.Filter = null;
            }
            else
            {
                newPath = "component/FilterOff.png";
                myStackPanel.Visibility = Visibility.Visible;
            }

            // Создание нового объекта BitmapImage
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(newPath, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();
            myImage.Source = bitmap;
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (myComboBox.SelectedItem != null)
            {   
                // Получите представление CollectionViewSource
                ICollectionView view = CollectionViewSource.GetDefaultView(myCatalog.compositions);

                // Пример фильтрации по свойству Property1, где значение должно быть "Значение1"
                view.Filter = item =>
                {
                    if (item is Composition myItem)
                    {
                        if (myComboBox.SelectedIndex == 0) return myItem.author.Contains(myFilterText.Text);
                        if (myComboBox.SelectedIndex == 1) return myItem.song.Contains(myFilterText.Text);
                    }
                    return false;
                };
            }
        }
    }
}
