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

namespace VectorEditor
{
    /// <summary>
    /// Логика взаимодействия для NewImageWindow.xaml
    /// </summary>
    public partial class NewImageWindow : Window
    {
        public NewImageWindow()
        {
            InitializeComponent();
        }

        public double ImageWidth { get; set; }
        public double ImageHeight { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double w, h;
            if (!double.TryParse(widthTextBox.Text, out w))
            {
                MessageBox.Show("Width has invalid format");
                return;
            }
            if (!double.TryParse(heightTextBox.Text, out h))
            {
                MessageBox.Show("Height has invalid format");
                return;
            }
            ImageWidth = w;
            ImageHeight = h;
            DialogResult = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
