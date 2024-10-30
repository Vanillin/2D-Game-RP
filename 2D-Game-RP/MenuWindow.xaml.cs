using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Xml.Linq;

namespace TwoD_Game_RP
{
    /// <summary>
    /// Логика взаимодействия для MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();

            MenuContinue.IsEnabled = false;
        }
        public void MenuInGame()
        {
            MenuStartNewGame.IsEnabled = false; 
            MenuContinue.IsEnabled = true;
        }
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            //завершение программы всей Application.Exit();
        }

        private void MenuStartNewGame_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow("1", PlayerGender.Man);
            mainWindow.Show();
            //StartHistoryWindow startHistoryWindow = new StartHistoryWindow();
            //startHistoryWindow.ShowDialog();
            this.Close();
            //((MenuWindow)this.Owner).Close();

            //CreateWindow createWindow = new CreateWindow();
            //createWindow.Owner = this;
            ////this.Close();
            //createWindow.ShowDialog();
        }

        private void MenuContinue_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuLessons_Click(object sender, RoutedEventArgs e)
        {
            LessonsWindow createWindow = new LessonsWindow();
            createWindow.Owner = this;
            //this.Close();
            createWindow.ShowDialog();
        }
    }
}
