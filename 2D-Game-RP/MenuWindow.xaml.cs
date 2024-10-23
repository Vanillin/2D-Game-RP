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

namespace Game_STALKER_Exclusion_Zone
{
    /// <summary>
    /// Логика взаимодействия для MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
            Icon = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["Textures"], $"icon.png"), UriKind.Relative));

            Image MenuBack = new Image()
            {
                Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["Textures"], $"menu.png"), UriKind.Relative)),
                Stretch = Stretch.Fill,
            };
            MenuBack.Width = 1940;
            MenuBack.Height = 1024; 
            Canvas.SetLeft(MenuBack, 0);
            Canvas.SetTop(MenuBack, 0);
            BackGround.Children.Add(MenuBack);

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
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            //StartHistoryWindow startHistoryWindow = new StartHistoryWindow();
            //startHistoryWindow.ShowDialog();
            this.Close();
            //((MenuWindow)this.Owner).Close();
            mainWindow.Init("1", PlayerGender.Man);

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
