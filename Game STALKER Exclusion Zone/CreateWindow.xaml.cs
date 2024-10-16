using LibraryForStalkerEZ;
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

namespace Game_STALKER_Exclusion_Zone
{
    /// <summary>
    /// Логика взаимодействия для CreateWindow.xaml
    /// </summary>
    public partial class CreateWindow : Window
    {
        public CreateWindow()
        {
            InitializeComponent();
            Icon = new BitmapImage(new Uri("gamedata/textures/icon.png", UriKind.Relative));

            Image men = new Image
            {
                Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"PlayerMan.png"), UriKind.Relative)) 
            };
            ImageMen.Content = men;
            Image woman = new Image
            {
                Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"PlayerWoman.png"), UriKind.Relative))
            };
            ImageWoman.Content = woman;
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if ((RadioButMen.IsChecked == true || RadioButWoman.IsChecked == true) && TextBoxNamePlayer.Text != null)
            {
                PlayerGender playerGender = PlayerGender.Man;
                if (RadioButWoman.IsChecked == true)
                {
                    playerGender = PlayerGender.Woman;
                }

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                StartHistoryWindow startHistoryWindow = new StartHistoryWindow();
                startHistoryWindow.ShowDialog();

                this.Close();
                ((MenuWindow)this.Owner).Close();

                mainWindow.Init(TextBoxNamePlayer.Text.ToString(), playerGender);
            }
        }

        private void ImageMen_Click(object sender, RoutedEventArgs e)
        {
            RadioButMen.IsChecked = true;
        }

        private void ImageWoman_Click(object sender, RoutedEventArgs e)
        {
            RadioButWoman.IsChecked = true;
        }
    }
}
