using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
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
        Dictionary<string, (Phrase phrase, string skeletName)> AllPhrases;
        Player player;

        public DialogWindow(Player Player, Skelet Person)
        {
            AllPhrases = new Dictionary<string, (Phrase phrase, string skelet)>();
            player = Player;

            foreach (Phrase phrase in Information.GetPhrase(Person.SystemName))
            {
                AllPhrases.Add(phrase.Index, (phrase, Person.Name));
            }
            foreach (Phrase phrase in Information.GetPhrase(Player.SystemName))
            {
                AllPhrases.Add(phrase.Index, (phrase, Player.Name));
            }

            InitializeComponent();

            CreatePersones(Player);
            CreatePersones(Person);
            //Player.DisplayInventory(InventoryPlayer, 20);
            string str = GetStartPhraseInDialog(Player, Person);
            if (str != null)
            {
                CreateDialog(str);
            }
            else
            {
                CreateClearDialog();
            }
        }
        private string GetStartPhraseInDialog(Player Player, Skelet Person)
        {
            foreach (var startdialog in Information.GetStartPhrases(Person.SystemName))
            {
                foreach (var task in Player.Tasks)
                {
                    if (task.SystemName == AllPhrases[startdialog].phrase.TaskToStart)
                        return startdialog;
                }
            }
            return null;
            //throw new Exception("Невозможно найти стартовый диалог");
        }
        private void ClearDialog()
        {
            DialogWin.Children.Remove(DialogBtn);
            DialogBtn.Children.Clear();
        }
        private void CreateClearDialog()
        {
            DialogWin.Children.Add(new Label()
            {
                Content = "...",
                FontSize = 16,
                Background = new SolidColorBrush(Colors.DarkGray),
                HorizontalAlignment = HorizontalAlignment.Left,
            });
            DialogWin.Children.Add(new Label()
            {
                Content = "...",
                FontSize = 20,
                Background = new SolidColorBrush(Colors.LightGray),
                HorizontalAlignment = HorizontalAlignment.Left,
            });
            ScrollView.ScrollToEnd();
        }
        private void CreateDialog(string index)
        {
            AddDialog(index, "l");
            foreach (var v in AllPhrases[index].phrase.NextIndexes)
            {
                AddButton(v);
            }
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
            Phrase phrase = AllPhrases[index].phrase;
            AddDialog(index, "r");
            ClearDialog();
            if (phrase.NextIndexes.Count == 1)
            {
                CreateDialog(phrase.NextIndexes[0]);
            }
            //throw new Exception("У фразы игрока должен быть только один последующий индекс диалога");
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
                Content = AllPhrases[index].phrase.Dialog,
                FontSize = 20,
                Margin = new Thickness(0, 2, 0, 2),
            };
            b.Click += Question_Click;
            DialogBtn.Children.Add(b);
            ScrollView.ScrollToEnd();
        }
        private void AddDialog(string index, string side)
        {
            Phrase phrase = AllPhrases[index].phrase;
            foreach (var task in phrase.NewTasks)
            {
                player.Tasks.Add(Information.FindTask(task));
            }
            foreach (var task in phrase.EndingTasks)
            {
                player.Tasks.Remove(Information.FindTask(task));
                player.CompliteTasks.Add(task);
            }
            if (side == "r")
            {
                DialogWin.Children.Add(new Label()
                {
                    Content = AllPhrases[index].skeletName,
                    FontSize = 16,
                    Background = new SolidColorBrush(Colors.DarkGray),
                    HorizontalAlignment = HorizontalAlignment.Right,
                });
                DialogWin.Children.Add(new Label()
                {
                    Content = phrase.Dialog,
                    FontSize = 20,
                    Background = new SolidColorBrush(Colors.LightGray),
                    HorizontalAlignment = HorizontalAlignment.Right,
                });
            }
            else
            {
                DialogWin.Children.Add(new Label()
                {
                    Content = AllPhrases[index].skeletName,
                    FontSize = 16,
                    Background = new SolidColorBrush(Colors.DarkGray),
                    HorizontalAlignment = HorizontalAlignment.Left,
                });
                DialogWin.Children.Add(new Label()
                {
                    Content = phrase.Dialog,
                    FontSize = 20,
                    Background = new SolidColorBrush(Colors.LightGray),
                    HorizontalAlignment = HorizontalAlignment.Left,
                });
            }
            ScrollView.ScrollToEnd();
        }
    }
}
