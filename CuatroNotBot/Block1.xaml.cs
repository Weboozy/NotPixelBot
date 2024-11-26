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

namespace CuatroNotBot
{
    /// <summary>
    /// Interaction logic for Block1.xaml
    /// </summary>
    public partial class Block1 : Window,IBaseMethods
    {
        public Block1()
        {
            InitializeComponent();
            InitializeBlock1();
        }

        private void InitializeBlock1()
        {
            for (int i = 0; i < 16; i++)
            {
                block1.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(15) });
                block1.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15) });
            }
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Grid grid = new Grid();
                    grid.Background = new SolidColorBrush(Colors.Transparent);
                    Grid.SetColumn(grid, j);
                    Grid.SetRow(grid, i);
                    block1.Children.Add(grid);
                }
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
