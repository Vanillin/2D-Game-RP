using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace TwoD_Game_RP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GamePoint LeftUpCorner;
        int sizeGamePoleH;
        int sizeGamePoleW;
        double pixelSizeGamePole;
        bool SeeInCurcle = true;

        int sizeInventH = 1;
        int sizeInventW = 7;
        double pixelSizeInvent;

        private List<UIElement> SystemObj;
        private int oblwatch = 8;
        private int oblsee = 6;


        public Location CurrentLocation;
        public List<Location> Locations = new List<Location>();
        public Player player;

        private DispatcherTimer timerReloadAnimation = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        private DispatcherTimer timerReloadAnalyzeTask = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(1500)
        };
        private void TimerAnalyzeTask_Tick(object sender, EventArgs e)
        {
            MainScripts.TimerAnalyze(this);
        }

        public MainWindow()
        {
            CreateMainWindow();
        }
        public MainWindow(string PlayerName, PlayerGender Gender)
        {
            CreateMainWindow();
        }
        private void CreateMainWindow()
        {
            //Information.Serialization();

            InitializeComponent();
            SystemObj = new List<UIElement>();
            timerReloadAnimation.Tick += TimerAnimation_Tick;
            timerReloadAnimation.IsEnabled = true;
            timerReloadAnalyzeTask.Tick += TimerAnalyzeTask_Tick;
            timerReloadAnalyzeTask.IsEnabled = true;

            player = new PlayerFirst("Детектив", "player", new GamePoint(19, 5), sizeInventH, sizeInventW,
                new List<Item>()
                {
                    new Telephone(), new NoteBook()
                },
                new CustomSortedEnum<string>()
                {
                    //"trainingButton"
                });

            SeeInCurcle = false;
            GoToLocation("Eosha");
            TimerAnimation_Tick(null, null);

            // SystemViewBtn.Visibility = Visibility.Visible;
        }
        private void ChangeSizeGamePole(int height, int wight, GamePoint player)
        {
            if (height % 2 == 0 && height != CurrentLocation.Height) throw new Exception("Размеры поля должны быть нечетными!");
            if (wight % 2 == 0 && wight != CurrentLocation.Width) throw new Exception("Размеры поля должны быть нечетными!");
            sizeGamePoleH = height;
            sizeGamePoleW = wight;
            pixelSizeGamePole = Math.Min(Map.ActualHeight / (sizeGamePoleH), Map.ActualWidth / (sizeGamePoleW));
            LeftUpCorner = new GamePoint(player.X - (height - 1) / 2, player.Y - (wight - 1) / 2);

            //if (LeftUpCorner.X < 0) LeftUpCorner.X = 0;
            //if (LeftUpCorner.Y < 0) LeftUpCorner.Y = 0;
            //if (LeftUpCorner.X > CurrentLocation.Height - sizeGamePoleH) LeftUpCorner.X = CurrentLocation.Height - sizeGamePoleH;
            //if (LeftUpCorner.Y > CurrentLocation.Width - sizeGamePoleW) LeftUpCorner.Y = CurrentLocation.Width - sizeGamePoleW;
        }
        private void ChangeSizeInventoryPlayer()
        {
            pixelSizeInvent = Math.Min(InventoryPlayer.ActualHeight / (sizeInventH), InventoryPlayer.ActualWidth / (sizeInventW));
        }
        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            DoActionAll();
            if (SeeInCurcle)
            {
                ChangeSizeGamePole(oblwatch * 2 + 1, oblwatch * 2 + 1, player.Cord);
                CustomSortedEnum<GamePoint> deleted = new CustomSortedEnum<GamePoint>();
                foreach (var v in CurrentLocation.GetLives())
                {
                    if (!v.IsClarity) deleted.Add(v.Cord);
                }
                var OblSee = CurrentLocation.GetWatchCircle_WithAngleOutOfPoint(player.Cord, oblsee - 0.1,
                    Math.Max((int)player.Cord.X - oblsee, 0), Math.Max((int)player.Cord.Y - oblsee, 0),
                    Math.Min((int)player.Cord.X + oblsee + 1, CurrentLocation.Height), Math.Min((int)player.Cord.Y + oblsee + 1, CurrentLocation.Width),
                    deleted);
                var OblWatch = CurrentLocation.GetWatchCirlce(player.Cord, oblwatch - 0.1,
                    Math.Max((int)player.Cord.X - oblwatch, 0), Math.Max((int)player.Cord.Y - oblwatch, 0),
                    Math.Min((int)player.Cord.X + oblwatch + 1, CurrentLocation.Height), Math.Min((int)player.Cord.Y + oblwatch + 1, CurrentLocation.Width));

                CurrentLocation.DisplayPointsForCornerAndDark(OblSee, OblWatch, LeftUpCorner, Map, pixelSizeGamePole, SystemObj);
            }
            else
            {
                ChangeSizeGamePole(CurrentLocation.Height, CurrentLocation.Width, player.Cord);
                CurrentLocation.DisplayForDark(Map, pixelSizeGamePole, SystemObj);
            }

            ChangeSizeInventoryPlayer();
            player.DisplayInventory(InventoryPlayer, pixelSizeInvent);
            //переключение новой картинки для анимации
        }
        private void GoToLocation(string name)
        {
            //сброс персонажа при переходе из локации

            Location loc = Locations.Find(x => x.SystemName == name);
            if (loc == null)
            {
                if (name == "Eosha")
                {
                    loc = MemoryLocations.GetEosha();
                }

                //Location newLoc = MemoryLocations.GetGarden(player, sizeInventH, sizeInventW);
                //CurrentLocation = newLoc;
            }
            CurrentLocation = loc;
            ChangeSizeGamePole(CurrentLocation.Height, CurrentLocation.Width, player.Cord);
        }
        private void DoActionAll()
        {
            foreach (var v in CurrentLocation.GetLives())
            {
                v.DoAction(CurrentLocation);
            }
        }

        //===============================================        Click      ===============================================

        bool IsDown = false;
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            SystemObj = new List<UIElement>();
            if (IsDown == false)
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.W:
                        { KeyDownToMove((int)player.Cord.X - 1, (int)player.Cord.Y); break; }
                    case System.Windows.Input.Key.A:
                        { KeyDownToMove((int)player.Cord.X, (int)player.Cord.Y - 1); break; }
                    case System.Windows.Input.Key.S:
                        { KeyDownToMove((int)player.Cord.X + 1, (int)player.Cord.Y); break; }
                    case System.Windows.Input.Key.D:
                        { KeyDownToMove((int)player.Cord.X, (int)player.Cord.Y + 1); break; }
                }
                IsDown = true;
            }
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            IsDown = false;
        }
        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SystemObj = new List<UIElement>();
            Point pointMouse = e.GetPosition(Map);
            (int W, int H) = ((int)Math.Truncate(pointMouse.X / pixelSizeGamePole), (int)Math.Truncate(pointMouse.Y / pixelSizeGamePole));
            (W, H) = (W + (int)LeftUpCorner.Y, H + (int)LeftUpCorner.X);

            KeyDownToMove(H, W);
        }
        private void KeyDownToMove(int H, int W)
        {
            if (W < 0 || W >= CurrentLocation.Width || H < 0 || H >= CurrentLocation.Height) return;
            if (!CurrentLocation.GetIsBlockCell(H, W))
            {
                player.ClearActions();
                player.ClearGlobalActions();
                player.EnqueueUpGlobalAction(new ActionMove(new GamePoint(H, W), false));

                //случай перехода на новую локацию или застревания где-либо
            }
        }
        private void Map_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SystemObj = new List<UIElement>();
            Point pointMouse = e.GetPosition(Map);
            (int W, int H) = ((int)Math.Truncate(pointMouse.X / pixelSizeGamePole), (int)Math.Truncate(pointMouse.Y / pixelSizeGamePole));
            (W, H) = (W + (int)LeftUpCorner.Y, H + (int)LeftUpCorner.X);
            if (W < 0 || W >= CurrentLocation.Width || H < 0 || H >= CurrentLocation.Height) return;
            Skelet people = CurrentLocation.FindLives(H, W);
            if (people == null) return;
            if (people is Door)
            {
                ClickOnDoor(people);
            }
            else if (!(people is Player))
            {
                ClickOnSkelet(people);
            }
        }
        private void ClickOnDoor(Skelet door)
        {
            bool ShelterKey = false; //(CurrentLocation.GetIsWatchCell((int)people.Cord.X, (int)people.Cord.Y));

            StackPanel menu = new StackPanel();
            Canvas.SetLeft(menu, pixelSizeGamePole * (door.Cord.Y + 1 - LeftUpCorner.Y));
            Canvas.SetTop(menu, pixelSizeGamePole * (door.Cord.X - LeftUpCorner.X));

            Button Open = new Button()
            {
                Content = "Открыть",
                Opacity = 0.6,
                FontSize = 20,
            };
            Open.Click += MenuDoorOpen_Click;
            Open.Tag = door;

            double dx = Math.Abs(door.Cord.X - player.Cord.X);
            double dy = Math.Abs(door.Cord.Y - player.Cord.Y);
            bool Near = (dx <= 2 && dy <= 1) || (dx <= 1 && dy <= 2);
            if (ShelterKey || !Near)
            {
                Open.IsEnabled = false;
            }
            if ((door as Door).IsLock)
            {
                Open.IsEnabled = false;
                Open.Content = "Заперто";
            }
            menu.Children.Add(Open);
            SystemObj.Add(menu);

        }
        private void ClickOnSkelet(Skelet people)
        {
            bool ShelterKey = false; //(CurrentLocation.GetIsWatchCell((int)people.Cord.X, (int)people.Cord.Y));

            StackPanel menu = new StackPanel();
            Canvas.SetLeft(menu, pixelSizeGamePole * (people.Cord.Y + 1 - LeftUpCorner.Y));
            Canvas.SetTop(menu, pixelSizeGamePole * (people.Cord.X - LeftUpCorner.X));

            Button Dialog = new Button()
            {
                Content = "Поговорить",
                Opacity = 0.6,
                FontSize = 20,
            };
            if (people is Dead)
            {
                Dialog.Content = "Осмотреть";
            }
            Dialog.Click += MenuPersonDialog_Click;
            Dialog.Tag = people;

            Button Check = new Button()
            {
                Content = "Обыскать",
                Opacity = 0.6,
                FontSize = 20,
            };
            Check.Click += MenuPersonCheck_Click;
            Check.Tag = people;

            double dx = Math.Abs(people.Cord.X - player.Cord.X);
            double dy = Math.Abs(people.Cord.Y - player.Cord.Y);
            bool Near = (dx <= 2 && dy <= 1) || (dx <= 1 && dy <= 2);
            if (ShelterKey || !Near)
            {
                Check.IsEnabled = false;
                Dialog.IsEnabled = false;
            }
            if (/*people.HealthInf() <= 0 ||*/ people.Fraction == NPSGroup.Box)
            {
                menu.Children.Add(Check);
            }
            else
            {
                menu.Children.Add(Dialog);
            }
            SystemObj.Add(menu);

        }
        private void MenuDoorOpen_Click(object sender, RoutedEventArgs e)
        {
            SystemObj = new List<UIElement>();
            Button button = (Button)sender;
            Skelet door = (Skelet)button.Tag;
            CurrentLocation.RemoveFirstLayerWithCell(door);
        }
        private void MenuPersonCheck_Click(object sender, RoutedEventArgs e)
        {
            if (InventorySearchWindow.Visibility == Visibility.Collapsed)
            {
                Button button = (Button)sender;
                Skelet people = (Skelet)button.Tag;

                timerReloadAnimation.IsEnabled = false;
                SystemObj = new List<UIElement>();
                InventorySearchWindow.Visibility = Visibility.Visible;
                CreateInventoryWindow(people);
            }
        }
        public void MenuPersonDialog_Click(Skelet people)
        {
            if (DialogWindow.Visibility == Visibility.Collapsed)
            {
                timerReloadAnimation.IsEnabled = false;
                SystemObj = new List<UIElement>();
                DialogWindow.Visibility = Visibility.Visible;
                CreateWindowDialog(people);
            }
        }
        private void MenuPersonDialog_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Skelet people = (Skelet)button.Tag;
            MenuPersonDialog_Click(people);
        }
        private void MenuTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskWindow.Visibility == Visibility.Collapsed)
            {
                timerReloadAnimation.IsEnabled = false;
                SystemObj = new List<UIElement>();
                TaskWindow.Visibility = Visibility.Visible;
                CreateTaskWindow();
            }
        }
        private void SystemViewBtn_Click(object sender, RoutedEventArgs e)
        {
            SeeInCurcle = !SeeInCurcle;
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //====================================================================================================================
        //====================================================================================================================
        //====================================================================================================================
        //============================================       DIALOGWINDOW     ================================================
        //====================================================================================================================
        //====================================================================================================================
        //====================================================================================================================

        Dictionary<string, (Phrase phrase, string skeletName)> AllPhrases;
        int lengthPhrase = 60;
        private void ExitDialogBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogWindow.Visibility = Visibility.Collapsed;
            DialogStackPanel.Children.Clear();
            DialogBtn.Children.Clear();
            timerReloadAnimation.IsEnabled = true;
        }
        private void CreateWindowDialog(Skelet Person)
        {
            AllPhrases = new Dictionary<string, (Phrase phrase, string skelet)>();

            foreach (Phrase phrase in Information.GetPhrase(Person.SystemName, lengthPhrase))
            {
                AllPhrases.Add(phrase.Index, (phrase, Person.Name));
            }
            foreach (Phrase phrase in Information.GetPhrase(player.SystemName, lengthPhrase))
            {
                AllPhrases.Add(phrase.Index, (phrase, player.Name));
            }

            string str = GetStartPhraseInDialog(Person);
            if (str != null)
            {
                CreateDialog(str);
            }
            else
            {
                CreateClearDialog();
            }
        }
        private string GetStartPhraseInDialog(Skelet Person)
        {
            foreach (var startdialog in Information.GetStartPhrases(Person.SystemName))
            {
                foreach (var task in player.Tasks.GetUsingTask())
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
            DialogStackPanel.Children.Remove(DialogBtn);
            DialogBtn.Children.Clear();
        }
        private void CreateClearDialog()
        {
            Border LabelName = new Border()
            {
                CornerRadius = new CornerRadius(10),
                Background = Brushes.DarkGray,
                Padding = new Thickness(3),
                Margin = new Thickness(0, 2, 0, 0),
                Child = new Label()
                {
                    Content = "...",
                    FontSize = 16,
                    Background = Brushes.Transparent,
                },
            };
            Border LabelText = new Border()
            {
                CornerRadius = new CornerRadius(10),
                Background = Brushes.LightGray,
                Padding = new Thickness(3),
                Margin = new Thickness(0, 0, 0, 2),
                Child = new Label()
                {
                    Content = "...",
                    FontSize = 20,
                    Background = Brushes.Transparent,
                }
            };
            LabelName.HorizontalAlignment = HorizontalAlignment.Left;
            LabelText.HorizontalAlignment = HorizontalAlignment.Left;

            DialogStackPanel.Children.Add(LabelName);
            DialogStackPanel.Children.Add(LabelText);
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
            foreach (var v in DialogStackPanel.Children)
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
        private void AddButton(string index)
        {
            Button b = new Button()
            {
                Tag = index,
                Content = AllPhrases[index].phrase.Dialog,
                Background = Brushes.Transparent,
                FontSize = 20,
            };
            b.Click += Question_Click;
            Border bor = new Border()
            {
                CornerRadius = new CornerRadius(10),
                Background = Brushes.AntiqueWhite,
                Padding = new Thickness(3),
                Margin = new Thickness(0, 2, 0, 2),
                Child = b,
            };

            DialogBtn.Children.Add(bor);
            ScrollView.ScrollToEnd();
        }
        private void AddDialog(string index, string side)
        {
            Phrase phrase = AllPhrases[index].phrase;

            foreach (var task in phrase.EndingTasks)
            {
                player.Tasks.ComplitedTask(task);
            }

            Border LabelName = new Border()
            {
                CornerRadius = new CornerRadius(10),
                Background = Brushes.DarkGray,
                Padding = new Thickness(3),
                Margin = new Thickness(0, 2, 0, 0),
                Child = new Label()
                {
                    Content = AllPhrases[index].skeletName,
                    FontSize = 16,
                    Background = Brushes.Transparent,
                },
            };
            Border LabelText = new Border()
            {
                CornerRadius = new CornerRadius(10),
                Background = Brushes.LightGray,
                Padding = new Thickness(3),
                Margin = new Thickness(0, 0, 0, 2),
                Child = new Label()
                {
                    Content = phrase.Dialog,
                    FontSize = 20,
                    Background = Brushes.Transparent,
                }
            };

            if (side == "r")
            {
                LabelName.HorizontalAlignment = HorizontalAlignment.Right;
                LabelText.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else
            {
                LabelName.HorizontalAlignment = HorizontalAlignment.Left;
                LabelText.HorizontalAlignment = HorizontalAlignment.Left;
            }
            DialogStackPanel.Children.Add(LabelName);
            DialogStackPanel.Children.Add(LabelText);
            ScrollView.ScrollToEnd();
        }


        //====================================================================================================================
        //====================================================================================================================
        //====================================================================================================================
        //==========================================       INVENTORYWINDOW     ===============================================
        //====================================================================================================================
        //====================================================================================================================
        //====================================================================================================================

        private void ExitInventorySearchBtn_Click(object sender, RoutedEventArgs e)
        {
            InventorySearchWindow.Visibility = Visibility.Collapsed;
            timerReloadAnimation.IsEnabled = true;
        }
        private void CreateInventoryWindow(Skelet skelet)
        {
            InventoryCanvas.Tag = skelet;
            skelet.DisplayInventory(InventoryCanvas, pixelSizeInvent);
        }
        private void InventoryPlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pointMouse = e.GetPosition(InventoryPlayer);
            (int W, int H) = ((int)Math.Truncate((pointMouse.X) / pixelSizeInvent),
                (int)Math.Truncate((pointMouse.Y) / pixelSizeInvent));
            Item item = player.InventSearchItem(H, W);
            if (item != null)
            {
                if (InventorySearchWindow.Visibility != Visibility.Collapsed)
                {
                    Skelet skelet = (Skelet)InventoryCanvas.Tag;
                    skelet.InventAdd(item);
                    player.InventRemove(item);

                    player.DisplayInventory(InventoryPlayer, pixelSizeInvent);
                    skelet.DisplayInventory(InventoryCanvas, pixelSizeInvent);
                }
                else
                {
                    if (item is NoteBook)
                    {
                        MenuTask_Click(null, null);
                    }
                    else if (item is Telephone)
                    {
                        ClickOnTelephone(null, null);
                    }
                }
            }
        }
        private void InventoryPlayer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void InventoryCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pointMouse = e.GetPosition(InventoryCanvas);
            (int W, int H) = ((int)Math.Truncate((pointMouse.X) / pixelSizeInvent),
                (int)Math.Truncate((pointMouse.Y) / pixelSizeInvent));
            Skelet skelet = (Skelet)InventoryCanvas.Tag;
            Item item = skelet.InventSearchItem(H, W);
            if (item != null)
            {
                skelet.InventRemove(item);
                player.InventAdd(item);

                player.DisplayInventory(InventoryPlayer, pixelSizeInvent);
                skelet.DisplayInventory(InventoryCanvas, pixelSizeInvent);
            }
        }
        private void InventoryCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        public bool CanTalkVanya = false;
        private void ClickOnTelephone(object sender, MouseButtonEventArgs e)
        {
            StackPanel menu = new StackPanel();
            Canvas.SetLeft(menu, pixelSizeGamePole * (player.Cord.Y + 1 - LeftUpCorner.Y));
            Canvas.SetTop(menu, pixelSizeGamePole * (player.Cord.X - LeftUpCorner.X));

            Button Agenstv = new Button()
            {
                Content = "Позвонить в Агенство",
                Opacity = 0.6,
                FontSize = 20,
            };
            Agenstv.Click += MenuCall_Click;
            Agenstv.Tag = new Agency(new GamePoint(0, 0), '0');
            menu.Children.Add(Agenstv);

            if (CanTalkVanya)
            {
                Button Vanya = new Button()
                {
                    Content = "Позвонить Ване",
                    Opacity = 0.6,
                    FontSize = 20,
                };
                Vanya.Click += MenuCall_Click;
                Vanya.Tag = new Vanya(new GamePoint(0, 0), '0');
                menu.Children.Add(Vanya);
            }

            SystemObj.Add(menu);
        }
        private void MenuCall_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Skelet people = (Skelet)button.Tag;

            timerReloadAnimation.IsEnabled = false;
            SystemObj = new List<UIElement>();
            DialogWindow.Visibility = Visibility.Visible;
            CreateWindowDialog(people);
        }

        //====================================================================================================================
        //====================================================================================================================
        //====================================================================================================================
        //=============================================       TASKWINDOW     =================================================
        //====================================================================================================================
        //====================================================================================================================
        //====================================================================================================================

        private void ExitTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            TaskWindow.Visibility = Visibility.Collapsed;
            ListTasks.Children.Clear();
            timerReloadAnimation.IsEnabled = true;
        }
        private void CreateTaskWindow()
        {
            foreach (var task in player.Tasks.GetDescriptionUsingTask())
            {
                ListTasks.Children.Add(new Border()
                {
                    CornerRadius = new CornerRadius(10),
                    Background = Brushes.DarkGray,
                    Padding = new Thickness(3),
                    Margin = new Thickness(0, 2, 0, 0),
                    Child = new Label()
                    {
                        Content = task.Name,
                        FontSize = 22,
                        Background = Brushes.Transparent,
                    },
                });
                ListTasks.Children.Add(new Border()
                {
                    CornerRadius = new CornerRadius(10),
                    Background = Brushes.LightGray,
                    Padding = new Thickness(3),
                    Margin = new Thickness(0, 0, 0, 2),
                    Child = new Label()
                    {
                        Content = task.Description,
                        FontSize = 16,
                        Background = Brushes.Transparent,
                    }
                });
            }
        }
    }
}
