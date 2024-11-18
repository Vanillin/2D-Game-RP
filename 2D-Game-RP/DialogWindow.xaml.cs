using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TwoD_Game_RP
{
    /// <summary>
    /// Логика взаимодействия для DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        Dictionary<string, Phrase> Phrase;
        Player Player;
        Skelet Person;
        StackPanel DialogBtn;

        public DialogWindow(Player Player, Skelet Person)
        {
            Phrase = new Dictionary<string, Phrase>();
            foreach (Phrase phrase in Information.GetPhrase("randStalker"))
            {
                Phrase.Add(phrase.Index, phrase);
            }
            this.Player = Player;
            this.Person = Person;
            this.DialogBtn = new StackPanel();
            InitializeComponent();

            CreatePersones(Player);
            CreatePersones(Person);
            //Player.DisplayInventory(InventoryPlayer, 20);
            CreateDialog("start");
        }
        private void CreateDialog(string index)
        {
            DialogWin.Children.Remove(DialogBtn);
            DialogBtn.Children.Clear();
            AddDialog(Phrase[index].Dialog, "l");
            foreach(var v in Phrase[index].NextIndexes)
            {
                AddButton(v);
            }
            DialogWin.Children.Add(DialogBtn);
        }
        private void Question_Click(object sender, RoutedEventArgs e)
        {
            foreach (var v in DialogWin.Children)
            {
                if (v is Button)
                {
                    Button but = (Button)v;
                    but.IsEnabled = false;
                }
            }
            string index = ((Button)sender).Tag.ToString();
            AddDialog(Phrase[index].Dialog, "r");
            if (Phrase[index].NextIndexes.Count != 1)
            {
                throw new Exception("У фразы игрока должен быть только один последующий индекс диалога");
            }
            CreateDialog(Phrase[index].NextIndexes[0]);
        }
        private void CreatePersones(Skelet skelet)
        {
            Grid grid = new Grid()
            {
                Margin = new Thickness(5),
            };
            ColumnDefinitionCollection cd = grid.ColumnDefinitions;
            cd.Add(new ColumnDefinition());
            cd.Add(new ColumnDefinition());

            Image image = new Image()
            {
                Height = 80,
                Width = 200,
                Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{skelet.SystemName}.png"), UriKind.Relative)),
            };
            Grid.SetColumn(image, 0);
            grid.Children.Add(image);

            Label name = new Label()
            {
                FontSize = 16,
                Content = $"Имя: {skelet.Name}"
            };
            Grid.SetColumn(name, 1);
            grid.Children.Add(name);

            StackTalk.Children.Add(grid);
        }
        private void AddButton(string index)
        {
            Button b = new Button()
            {
                Tag = index,
                Content = Phrase[index].Dialog,
                FontSize = 20,
                Margin = new Thickness(0, 2, 0, 2),
            };
            b.Click += Question_Click;
            DialogBtn.Children.Add(b);
            ScrollView.ScrollToEnd();
        }
        private void AddDialog(string dialog, string side)
        {
            if (side == "r")
            {
                DialogWin.Children.Add(new Label()
                {
                    Content = Player.Name,
                    FontSize = 16,
                    Background = new SolidColorBrush(Colors.DarkGray),
                    HorizontalAlignment = HorizontalAlignment.Right,
                });
                DialogWin.Children.Add(new Label()
                {
                    Content = dialog,
                    FontSize = 20,
                    Background = new SolidColorBrush(Colors.LightGray),
                    HorizontalAlignment = HorizontalAlignment.Right,
                });
            }
            else
            {
                DialogWin.Children.Add(new Label()
                {
                    Content = Person.Name,
                    FontSize = 16,
                    Background = new SolidColorBrush(Colors.DarkGray),
                    HorizontalAlignment = HorizontalAlignment.Left,
                });
                DialogWin.Children.Add(new Label()
                {
                    Content = dialog,
                    FontSize = 20,
                    Background = new SolidColorBrush(Colors.LightGray),
                    HorizontalAlignment = HorizontalAlignment.Left,
                });
            }
            ScrollView.ScrollToEnd();
        }
    }
}
