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

namespace TwoD_Game_RP
{
    /// <summary>
    /// Логика взаимодействия для TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        MainWindow parent;
        public TestWindow(MainWindow window)
        {
            parent = window;
            InitializeComponent();
        }

        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            parent.SeeInCurcle = !parent.SeeInCurcle;
        }

        private void CompliteTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            string nameTask = NameTaskTxt.Text;
            try
            {
                parent.player.Tasks.ComplitedTask(nameTask);
            }
            catch (CustomException ce)
            {
                MessageBox.Show(ce.Message);
            }
        }
    }
}
