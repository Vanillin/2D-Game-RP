using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TwoD_Game_RP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double compressH = 0.5;
        double compressW = 1;
        GamePoint LeftUpCorner;
        int sizeGamePoleH;
        int sizeGamePoleW;
        double pixelSizeGamePole;
        internal bool SeeInCurcle = true;

        int sizeInventH = 2;
        int sizeInventW = 7;
        double pixelSizeInvent;

        private List<UIElement> SystemObj;
        internal int oblwatch = 8;
        internal int oblsee = 6;


        public Location CurrentLocation;
        public List<Location> Locations = new List<Location>();
        public Player player;

        private DispatcherTimer timerReloadAnimation = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        private DispatcherTimer timerReloadAnalyzeTask = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(5000)
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

            player = new PlayerFirst("Детектив", "playerm", new GamePoint(19, 5), sizeInventH, sizeInventW,
                new List<Item>()
                {
                    new Telephone(), new NoteBook()
                },
                new CustomSortedEnum<string>()
                {
                    "start1Ep",
                    "test",
                });
            player.GiveGunInHand(new Pistol());

            GoToLocationStart("Eosha");
            AnalyzeSizeGamePole();
            TimerAnimation_Tick(null, null);
        }
        private void AnalyzeSizeGamePole()
        {
            if (SeeInCurcle)
                ChangeSizeGamePole(oblwatch * 2 + 1, oblwatch * 2 + 1, player.Cord);
            else
                ChangeSizeGamePole(CurrentLocation.Height, CurrentLocation.Width, player.Cord);
        }
        internal void ChangeSizeGamePole(int height, int wight, GamePoint player)
        {
            if (height % 2 == 0 && height != CurrentLocation.Height) throw new Exception("Размеры поля должны быть нечетными!");
            if (wight % 2 == 0 && wight != CurrentLocation.Width) throw new Exception("Размеры поля должны быть нечетными!");
            sizeGamePoleH = height;
            sizeGamePoleW = wight;
            if (Map.ActualHeight == 0)
                pixelSizeGamePole = Math.Min(Map.Height / (sizeGamePoleH * compressH + 1), Map.Width / (sizeGamePoleW * compressW));
            else
                pixelSizeGamePole = Math.Min(Map.ActualHeight / (sizeGamePoleH * compressH + 1), Map.ActualWidth / (sizeGamePoleW * compressW));

            ChangeLeftUpCornerGamePole(height, wight, player);
        }
        private void ChangeLeftUpCornerGamePole(int height, int wight, GamePoint player)
        {
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
                ChangeLeftUpCornerGamePole(oblwatch * 2 + 1, oblwatch * 2 + 1, player.Cord);
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
                CurrentLocation.Display(Map, pixelSizeGamePole, SystemObj);
            }

            ChangeSizeInventoryPlayer();
            DisplayPlayerInventory(InventoryPlayer, pixelSizeInvent);
            //переключение новой картинки для анимации
        }
        public void GoToLocationStart(string name)
        {
            timerReloadAnimation.IsEnabled = false;
            Location loc = Locations.Find(x => x.SystemName == name);
            if (loc == null)
            {
                if (name == "Eosha")
                {
                    loc = MemoryLocations.GetEosha(player, compressH, compressW);
                }
                else if (name == "Mine")
                {
                    loc = MemoryLocations.GetMine(player, compressH, compressW);
                }
                else if (name == "UnderEosha")
                {
                    loc = MemoryLocations.GetUnderEosha(player, compressH, compressW);
                }
                else
                {
                    throw new CustomException($"Location {name} in not find");
                }
            }
            CurrentLocation = loc;
            timerReloadAnimation.IsEnabled = true;
        }
        public void GoToLocation(string name)
        {
            timerReloadAnimation.IsEnabled = false;

            string nameCurrentLoc = CurrentLocation.SystemName;
            CurrentLocation.RemoveFirstLayerWithCell(player);

            Location loc = Locations.Find(x => x.SystemName == name);
            if (loc == null)
            {
                if (name == "Eosha")
                {
                    loc = MemoryLocations.GetEosha(player, compressH, compressW);
                }
                else if (name == "Mine")
                {
                    loc = MemoryLocations.GetMine(player, compressH, compressW);
                }
                else if (name == "UnderEosha")
                {
                    loc = MemoryLocations.GetUnderEosha(player, compressH, compressW);
                }
                else
                {
                    throw new CustomException($"Location {name} in not find");
                }
            }
            CurrentLocation = loc;
            player.Cord = CurrentLocation.GetGamePointTransitSpawn(nameCurrentLoc);
            CurrentLocation.AddFirstLayerWithCell(player);

            if (SeeInCurcle)
                ChangeSizeGamePole(oblwatch * 2 + 1, oblwatch * 2 + 1, player.Cord);
            else
                ChangeSizeGamePole(CurrentLocation.Height, CurrentLocation.Width, player.Cord);

            timerReloadAnimation.IsEnabled = true;
        }
        private void DoActionAll()
        {
            List<SystemSket> list = new List<SystemSket>();
            foreach (var v in CurrentLocation.GetLives())
            {
                if (v is Skelet && (v as Skelet).IsAlive) list.Add(v);
                else if (!(v is Skelet)) list.Add(v);
            }
            foreach (var v in list)
            {
                v.DoAction(CurrentLocation);
            }
            //    bool isDoPlayer = true;
            //    if (!isDoPlayer)
            //    {
            //        Thread.Sleep(500);
            //    }
        }
        private void DisplayPlayerInventory(Canvas canvasInv, double size)
        {
            if (player.GetGunInHand() != null)
                GunInHandImage.Source = new BitmapImage(new Uri(player.GetGunInHand().Picture.Picture(), UriKind.Relative));
            else
                GunInHandImage.Source = null;
            if (player.GetGunInBack() != null)
                GunInBackImage.Source = new BitmapImage(new Uri(player.GetGunInBack().Picture.Picture(), UriKind.Relative));
            else
                GunInBackImage.Source = null;

            HealthBar.Value = player.HealthPercent * 100;
            player.DisplayInventory(canvasInv, size);
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
        private (int, int) FindPointClick(Point point)
        {
            return ((int)Math.Truncate(point.X / (pixelSizeGamePole * compressW)), (int)Math.Truncate(point.Y / (pixelSizeGamePole * compressH)) - 1);
        }
        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SystemObj = new List<UIElement>();
            Point pointMouse = e.GetPosition(Map);
            (int W, int H) = FindPointClick(pointMouse);
            (W, H) = (W + (int)LeftUpCorner.Y, H + (int)LeftUpCorner.X);

            KeyDownToMove(H, W);
        }
        private void KeyDownToMove(int H, int W)
        {
            if (W < 0 || W >= CurrentLocation.Width || H < 0 || H >= CurrentLocation.Height) return;

            string nameloc = CurrentLocation.GetSystemNameLocTransitCell(H, W);
            if (nameloc != null)
            {
                GoToLocation(nameloc);
            }
            else if (!CurrentLocation.GetIsBlockCell(H, W))
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
            (int W, int H) = FindPointClick(pointMouse);
            (W, H) = (W + (int)LeftUpCorner.Y, H + (int)LeftUpCorner.X);
            if (W < 0 || W >= CurrentLocation.Width || H < 0 || H >= CurrentLocation.Height) return;
            SystemSket people = CurrentLocation.FindLives(H, W);
            if (people == null) return;
            if (people is Door)
            {
                ClickOnDoor(people);
            }
            else if (!(people is Player) && people is Skelet)
            {
                ClickOnSkelet(people as Skelet);
            }
        }
        private void ClickOnDoor(SystemSket door)
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

            Button Attack = new Button()
            {
                Content = "Атаковать",
                Opacity = 0.6,
                FontSize = 20,
            };
            Attack.Click += MenuPersonAttack_Click;
            Attack.Tag = people;

            menu.Children.Add(Attack);


            Button Dialog = new Button()
            {
                Content = "Поговорить",
                Opacity = 0.6,
                FontSize = 20,
            };
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
            SystemSket door = (SystemSket)button.Tag;
            CurrentLocation.RemoveFirstLayerWithCell(door);
        }

        private void MenuPersonAttack_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Skelet people = (Skelet)button.Tag;

            player.ClearActions();
            player.ClearGlobalActions();
            player.EnqueueUpGlobalAction(new ActionAttack(people, false));
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

        private void ReverseGunsBtn_Click(object sender, RoutedEventArgs e)
        {
            player.ChangeGunInHandAndInBack();
        }

        //====================================================================================================================
        //====================================================================================================================
        //====================================================================================================================
        //============================================       DIALOGWINDOW     ================================================
        //====================================================================================================================
        //====================================================================================================================
        //====================================================================================================================

        string namePerson;
        CustomDictionary<string, Phrase> phrases;
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
            namePerson = Person.Name;
            DialogInform informPhrases = Information.GetPhrasesPerson(Person.SystemName, lengthPhrase);

            string str = GetStartPhraseInDialog(informPhrases.StartedTasksPerson);
            if (str != null)
            {
                phrases = informPhrases.Phrases;
                CreateDialog(str);
            }
            else
            {
                var liststr = GetStartPhraseInDialogPlayer(informPhrases.StartedTasksPlayer);
                if (liststr.Count != 0)
                {
                    phrases = informPhrases.Phrases;
                    foreach (var v in liststr)
                    {
                        AddButton(v);
                    }
                }
                else
                {
                    phrases = null;
                    CreateClearDialog();
                }
            }
        }
        private string GetStartPhraseInDialog(List<(string, string)> startedPhrases)
        {
            foreach (var phraseTask in startedPhrases)
            {
                foreach (var task in player.Tasks.GetUsingTask())
                {
                    if (task.SystemName == phraseTask.Item2)
                        return phraseTask.Item1;
                }
            }
            return null;
            //throw new Exception("Невозможно найти стартовый диалог");
        }
        private List<string> GetStartPhraseInDialogPlayer(List<(string, string)> startedPhrases)
        {
            List<string> retur = new List<string>();
            foreach (var phraseTask in startedPhrases)
            {
                foreach (var task in player.Tasks.GetUsingTask())
                {
                    if (task.SystemName == phraseTask.Item2)
                        retur.Add(phraseTask.Item1);
                }
            }
            return retur;
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

            foreach (var v in phrases[index].NextSystemNames)
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
            AddDialog(index, "r");
            ClearDialog();
            if (phrases[index].NextSystemNames.Count == 1)
            {
                CreateDialog(phrases[index].NextSystemNames[0]);
            }
            //throw new Exception("У фразы игрока должен быть только один последующий индекс диалога");
        }
        private void AddButton(string index)
        {
            Button b = new Button()
            {
                Tag = index,
                Content = phrases[index].Text,
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
            foreach (var task in phrases[index].ComplitedTaskSystemNames)
            {
                player.Tasks.ComplitedTask(task);
            }

            Label label;
            if (side == "r")
            {
                label = new Label()
                {
                    Content = player.Name,
                    FontSize = 16,
                    Background = Brushes.Transparent,
                };
            }
            else
            {
                label = new Label()
                {
                    Content = namePerson,
                    FontSize = 16,
                    Background = Brushes.Transparent,
                };
            }
            Border LabelName = new Border()
            {
                CornerRadius = new CornerRadius(10),
                Background = Brushes.DarkGray,
                Padding = new Thickness(3),
                Margin = new Thickness(0, 2, 0, 0),
                Child = label
            };
            Border LabelText = new Border()
            {
                CornerRadius = new CornerRadius(10),
                Background = Brushes.LightGray,
                Padding = new Thickness(3),
                Margin = new Thickness(0, 0, 0, 2),
                Child = new Label()
                {
                    Content = phrases[index].Text,
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
            Item item = player.SearchInBackpack(H, W);
            if (item != null)
            {
                if (InventorySearchWindow.Visibility != Visibility.Collapsed)
                {
                    Skelet skelet = (Skelet)InventoryCanvas.Tag;
                    skelet.AddInBackpack(item);
                    player.RemoveInBackpack(item);

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
            Item item = skelet.SearchInBackpack(H, W);
            if (item != null)
            {
                skelet.RemoveInBackpack(item);
                player.AddInBackpack(item);

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

        //============================================ Test System ========================================

        private void TestWindow_Click(object sender, RoutedEventArgs e)
        {
            TestWindow tw = new TestWindow(this);
            tw.Show();
        }
    }
}
