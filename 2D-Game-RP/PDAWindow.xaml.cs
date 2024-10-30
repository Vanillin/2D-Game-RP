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
using System.Configuration;
using System.Windows.Controls.Primitives;

namespace TwoD_Game_RP
{
    /// <summary>
    /// Логика взаимодействия для PDAWindow.xaml
    /// </summary>
    public partial class PDAWindow : Window
    {
        public PDAWindow(Player player)
        {
            InitializeComponent();
            Icon = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["Textures"], $"icon.png"), UriKind.Relative));

            Image PDAMapBack = new Image()
            {
                Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["Textures"], $"map.png"), UriKind.Relative)),
                Stretch = Stretch.Fill,
            };
            PDAMapBack.Width = PDAMap.Width;
            PDAMapBack.Height = PDAMap.Height;
            PDAMap.Children.Add(PDAMapBack);

            foreach (var task in player.Tasks)
            {
                ListTasks.Children.Add(new Label()
                {
                    Content = $"{task.Name}",
                    FontSize = 22,
                });
                ListTasks.Children.Add(new Label()
                {
                    Content = $"{task.SecondName}",
                    FontSize = 16,
                });
                ListTasks.Children.Add(new Separator());

                Image MenuBack = new Image()
                {
                    Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["Textures"], $"kvest.png"), UriKind.Relative)),
                    Stretch = Stretch.Fill,
                };
                MenuBack.Width = 50;
                MenuBack.Height = 50;
                Canvas.SetLeft(MenuBack, PDAMap.Width * task.CordOnMapY - 25);
                Canvas.SetTop(MenuBack, PDAMap.Height * task.CordOnMapX - 25);
                PDAMap.Children.Add(MenuBack);
            }
        }
    }
}
