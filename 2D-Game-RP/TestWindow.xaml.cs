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

            if (parent.SeeInCurcle)
                parent.ChangeSizeGamePole(parent.oblwatch * 2 + 1, parent.oblwatch * 2 + 1, parent.player.Cord);
            else
                parent.ChangeSizeGamePole(parent.CurrentLocation.Height, parent.CurrentLocation.Width, parent.player.Cord);
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

        private void TransiteLocBtn_Click(object sender, RoutedEventArgs e)
        {
            string nameLoc = NameLocTxt.Text;
            try
            {
                parent.GoToLocation(nameLoc);
            }
            catch (CustomException ce)
            {
                MessageBox.Show(ce.Message);
            }
        }
    }
}
