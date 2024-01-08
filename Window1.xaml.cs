using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace lab5
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public string author { get; private set; }
        public string song{ get; private set; }
        public Window1()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика обработки при нажатии кнопки "OK"
            if (!string.IsNullOrEmpty(TextBox1.Text) && !string.IsNullOrEmpty(TextBox2.Text))
            {
                author = TextBox1.Text;
                song = TextBox2.Text;
                DialogResult = true; // Устанавливаем DialogResult в true
                Close(); // Закрываем диалоговое окно
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика обработки при нажатии кнопки "Отмена"
            DialogResult = false; // Устанавливаем DialogResult в false
            Close(); // Закрываем диалоговое окно
        }
     }
}
