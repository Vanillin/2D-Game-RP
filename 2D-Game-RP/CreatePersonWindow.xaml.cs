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
    /// Логика взаимодействия для CreatePersonWindow.xaml
    /// </summary>
    public partial class CreatePersonWindow : Window
    {
        public CreatePersonWindow()
        {
            InitializeComponent();
            StartTextHistoryLb.Text = Information.GetTexts("textStart1");
            StartTextPersonLb.Text = Information.GetTexts("textStart2");
        }
        private void CreatePerson_Click(object sender, RoutedEventArgs e)
        {
            var actuals = new List<Actuals>();
            if (SniperRB.IsEnabled) actuals.Add(Actuals.Sniper);
            if (MechanicRB.IsEnabled) actuals.Add(Actuals.Mechanic);
            MainWindow mainWindow = new MainWindow(NamePerson.Text, PlayerGender.Man, actuals);
            this.Close();
            mainWindow.Show();
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowHistoryWorld.Visibility = Visibility.Collapsed;
            WindowCreatePerson.Visibility = Visibility.Visible;
        }

        private void GenderM_Click(object sender, RoutedEventArgs e)
        {
            PicturePlayerImg.Source = new BitmapImage( new Uri("gamedata/textures/player/playerm-map.png", UriKind.RelativeOrAbsolute));
        }
        private void GenderG_Click(object sender, RoutedEventArgs e)
        {
            PicturePlayerImg.Source = new BitmapImage(new Uri("gamedata/textures/player/playerg-map.png", UriKind.RelativeOrAbsolute));
        }
    }
}
