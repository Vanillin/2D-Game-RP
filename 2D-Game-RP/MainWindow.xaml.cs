using System;
using System.Collections.Generic;
using System.Configuration;
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
        enum GameMode
        {
            E, Q, F, R   //r - attack, f - active, e - move, q - view
        }
        GameMode _activeGameMode = GameMode.E;
        ImageBrush active = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["Textures"], $"lampOn.png"), UriKind.Relative)),
            Stretch = Stretch.Fill,
        };
        ImageBrush notactive = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["Textures"], $"lampOff.png"), UriKind.Relative)),
            Stretch = Stretch.Fill,
        };
        //Brush active = Brushes.Tan;
        //Brush notactive = Brushes.AntiqueWhite;
        private void EBtn_Click(object sender, RoutedEventArgs e)
        {
            _activeGameMode = GameMode.E;
            EBtn.Background = active;
            QBtn.Background = notactive;
            RBtn.Background = notactive;
            FBtn.Background = notactive;
            this.Cursor = null;
        }
        private void QBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_activeGameMode == GameMode.Q)
            {
                EBtn_Click(null, null);
            }
            else
            {
                _activeGameMode = GameMode.Q;
                EBtn.Background = notactive;
                QBtn.Background = active;
                RBtn.Background = notactive;
                FBtn.Background = notactive;
                this.Cursor = Cursors.Help;
            }
        }
        private void FBtn_Click(object sender, RoutedEventArgs e)
        {

            if (_activeGameMode == GameMode.F)
            {
                EBtn_Click(null, null);
            }
            else
            {
                _activeGameMode = GameMode.F;
                EBtn.Background = notactive;
                QBtn.Background = notactive;
                RBtn.Background = notactive;
                FBtn.Background = active;
                this.Cursor = Cursors.Hand;
            }
        }
        private void RBtn_Click(object sender, RoutedEventArgs e)
        {

            if (_activeGameMode == GameMode.R)
            {
                EBtn_Click(null, null);
            }
            else
            {
                _activeGameMode = GameMode.R;
                EBtn.Background = notactive;
                QBtn.Background = notactive;
                RBtn.Background = active;
                FBtn.Background = notactive;
                this.Cursor = Cursors.Cross;
            }
        }

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
        internal int oblwatch = 9;
        internal int oblsee = 7;
        internal int oblseeenemy = 6;

        public Location CurrentLocation;
        public List<Location> Locations = new List<Location>();
        public PlayerSkelet player;
        const int timeTimerSmall = 690/2;
        const int timeTimerBig = 1150/2;

        private DispatcherTimer timerReloadAnimation = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(timeTimerSmall)
        };
        private DispatcherTimer timerReloadAnalyzeTask = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(timeTimerSmall)
        };
        private DispatcherTimer timerReloadDoAll = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(timeTimerSmall)
        };

        public MainWindow()
        {
            CreateMainWindow();
        }
        public MainWindow(string PlayerName, PlayerGender Gender)
        {
            CreateMainWindow();
        }
        public MainWindow(string PlayerName, PlayerGender Gender, List<Actuals> actuals)
        {
            CreateMainWindow();
        }
        private void CreateMainWindow()
        {
            //Information.Serialization();

            InitializeComponent();
            EBtn_Click(null, null);
            SystemObj = new List<UIElement>();
            timerReloadAnimation.Tick += TimerAnimation_Tick;
            timerReloadAnimation.IsEnabled = true;
            //Thread.Sleep(20);
            //timerReloadAnalyzeTask.Tick += TimerAnalyzeTask_Tick;
            //timerReloadAnalyzeTask.IsEnabled = true;
            //timerReloadDoAll.Tick += TimerReloadDoAll_Tick;
            //timerReloadDoAll.IsEnabled = true;

            player = new PlayerFirst("Детектив", "playerm", new GamePoint(19, 5), sizeInventH, sizeInventW,
                new List<Item>()
                {
                    new NoteBook()
                },
                new CustomSortedEnum<string>()
                {
                    "start1Ep",
                    "test",
                }, 
                200);
            player.GiveGunInHand(new SmallToz());

            GoToLocationStart("Eosha");
            AnalyzeSizeGamePole();
            TimerAnimation_Tick(null, null);
        }

        private void AnalyzeSizeGamePole()
        {
            if (SeeInCurcle)
                ChangeSizeGamePole(oblwatch * 2 + 1, oblwatch * 2 + 1, player.GPoint);
            else
                ChangeSizeGamePole(CurrentLocation.Height, CurrentLocation.Width, player.GPoint);
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
            Map.Height = pixelSizeGamePole * (sizeGamePoleH * compressH + 1);
            Map.Width = pixelSizeGamePole * (sizeGamePoleW * compressW);

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
            //if (InventoryPlayer.ActualHeight == 0)
            //    pixelSizeInvent = Math.Min(InventoryPlayer.Height / (sizeInventH), InventoryPlayer.Width / (sizeInventW));
            //else
            //    pixelSizeInvent = Math.Min(InventoryPlayer.ActualHeight / (sizeInventH), InventoryPlayer.ActualWidth / (sizeInventW));
            //InventoryPlayer.Height = pixelSizeInvent * (sizeInventH);
            //InventoryPlayer.Width = pixelSizeInvent * (sizeInventW);

            pixelSizeInvent = 70;
        }

        private void ChangeTimerInterval() //не должен обнулять время
        {
            //if (player.IsEmptyAction())
            //    timerReloadAnimation.Interval = TimeSpan.FromMilliseconds(timeTimerBig);
            //else
            //    timerReloadAnimation.Interval = TimeSpan.FromMilliseconds(timeTimerSmall);
        }
        private void TimerAnalyzeTask_Tick(object sender, EventArgs e)
        {
            MainScripts.TimerAnalyze(this);
        }
        private void TimerReloadDoAll_Tick(object sender, EventArgs e)
        {
            DoActionAll();
            ChangeTimerInterval();
        }
        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            TimerAnalyzeTask_Tick(null, null);
            TimerReloadDoAll_Tick(null, null);

            if (SeeInCurcle)
            {
                ChangeLeftUpCornerGamePole(oblwatch * 2 + 1, oblwatch * 2 + 1, player.GPoint);
                CustomSortedEnum<GamePoint> deleted = new CustomSortedEnum<GamePoint>();
                foreach (var v in CurrentLocation.GetLives())
                {
                    if (!v.IsClarity) deleted.Add(v.GPoint);
                }
                var OblSee = CurrentLocation.GetWatchCircle_WithAngleOutOfPoint(player.GPoint, oblsee - 0.1,
                    Math.Max((int)player.GPoint.X - oblsee, 0), Math.Max((int)player.GPoint.Y - oblsee, 0),
                    Math.Min((int)player.GPoint.X + oblsee + 1, CurrentLocation.Height), Math.Min((int)player.GPoint.Y + oblsee + 1, CurrentLocation.Width),
                    deleted);
                var OblWatch = CurrentLocation.GetWatchCirlce(player.GPoint, oblwatch - 0.1,
                    Math.Max((int)player.GPoint.X - oblwatch, 0), Math.Max((int)player.GPoint.Y - oblwatch, 0),
                    Math.Min((int)player.GPoint.X + oblwatch + 1, CurrentLocation.Height), Math.Min((int)player.GPoint.Y + oblwatch + 1, CurrentLocation.Width));

                CurrentLocation.DisplayPointsForCornerAndDark(OblSee, OblWatch, LeftUpCorner, Map, pixelSizeGamePole, SystemObj);
            }
            else
            {
                CurrentLocation.Display(Map, pixelSizeGamePole, SystemObj);
            }

            ChangeSizeInventoryPlayer();
            //DisplayPlayerInventory(InventoryHotBar, pixelSizeInvent);
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
                    loc = MemoryLocations.GetEosha(player, oblwatch, compressH, compressW);
                }
                else if (name == "Mine")
                {
                    loc = MemoryLocations.GetMine(oblseeenemy, compressH, compressW);
                }
                else if (name == "UnderEosha")
                {
                    loc = MemoryLocations.GetUnderEosha(compressH, compressW);
                }
                else
                {
                    throw new CustomException($"Location {name} in not find");
                }
                Locations.Add(loc);
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
                    loc = MemoryLocations.GetEosha(player, oblwatch, compressH, compressW);
                }
                else if (name == "Mine")
                {
                    loc = MemoryLocations.GetMine(oblseeenemy, compressH, compressW);
                }
                else if (name == "UnderEosha")
                {
                    loc = MemoryLocations.GetUnderEosha(compressH, compressW);
                }
                else
                {
                    throw new CustomException($"Location {name} in not find");
                }
            }
            CurrentLocation = loc;
            player.GPoint = CurrentLocation.GetGamePointTransitSpawn(nameCurrentLoc);
            CurrentLocation.AddFirstLayerWithCell(player);

            if (SeeInCurcle)
                ChangeSizeGamePole(oblwatch * 2 + 1, oblwatch * 2 + 1, player.GPoint);
            else
                ChangeSizeGamePole(CurrentLocation.Height, CurrentLocation.Width, player.GPoint);

            timerReloadAnimation.IsEnabled = true;
        }
        private void DoActionAll()
        {
            List<SystemSkelet> list = new List<SystemSkelet>();

            if (!(player).IsAlive && CurrentLocation.SystemName == "Mine")
            {
                player.ClearGlobalActions();
                player.ClearActions();
                player.EnqueueUpGlobalAction(new ActionTeleport(new GamePoint(32, 25), false));
                player.PlusHealth(100);
            }

            foreach (var v in CurrentLocation.GetLives())
            {
                if (v is AliveSkelet && (v as AliveSkelet).IsAlive) list.Add(v);
                else if (!(v is AliveSkelet)) list.Add(v);
            }
            foreach (var v in list)
            {
                v.DoAction(CurrentLocation);
            }
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
            //player.DisplayInventory(canvasInv, size);
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
                        { KeyDownToMove((int)player.GPoint.X - 1, (int)player.GPoint.Y); break; }
                    case System.Windows.Input.Key.A:
                        { KeyDownToMove((int)player.GPoint.X, (int)player.GPoint.Y - 1); break; }
                    case System.Windows.Input.Key.S:
                        { KeyDownToMove((int)player.GPoint.X + 1, (int)player.GPoint.Y); break; }
                    case System.Windows.Input.Key.D:
                        { KeyDownToMove((int)player.GPoint.X, (int)player.GPoint.Y + 1); break; }
                    case System.Windows.Input.Key.Q:
                        { QBtn_Click(null, null); break; }
                    case System.Windows.Input.Key.E:
                        { EBtn_Click(null, null); break; }
                    case System.Windows.Input.Key.R:
                        { RBtn_Click(null, null); break; }
                    case System.Windows.Input.Key.F:
                        { FBtn_Click(null, null); break; }
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
            (int W, int H) = FindPointClick(pointMouse);
            (W, H) = (W + (int)LeftUpCorner.Y, H + (int)LeftUpCorner.X + 1);

            if (W < 0 || W >= CurrentLocation.Width || H < 0 || H >= CurrentLocation.Height) return;

            if (_activeGameMode == GameMode.E)
            {
                KeyDownToMove(H, W);
            }
            else if (_activeGameMode == GameMode.R)
            {
                SystemSkelet people;
                if (H + 1 < CurrentLocation.Height && CurrentLocation.FindLives(H + 1, W) != null)
                {
                    people = CurrentLocation.FindLives(H + 1, W);
                }
                else
                {
                    people = CurrentLocation.FindLives(H, W);
                }
                if (people == null) return;
                if (people is AliveSkelet && (people as AliveSkelet).IsAlive && !(people is PlayerSkelet)) MenuPersonAttack_Click(people as AliveSkelet);
            }
            else if (_activeGameMode == GameMode.F)
            {
                SystemSkelet people;
                if (H + 1 < CurrentLocation.Height && CurrentLocation.FindLives(H + 1, W) != null)
                {
                    people = CurrentLocation.FindLives(H + 1, W);
                }
                else
                {
                    people = CurrentLocation.FindLives(H, W);
                }
                if (people == null) return;
                if (people is DoorSkelet) MenuDoorOpen_Click(people as DoorSkelet);
                if (people is StorageSkelet) MenuPersonCheck_Click(people as StorageSkelet);
            }
            else if (_activeGameMode == GameMode.Q)
            {
                ClickOnWatch(H, W); 
            }
        }
        private void Map_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SystemObj = new List<UIElement>();
            Point pointMouse = e.GetPosition(Map);
            (int W, int H) = FindPointClick(pointMouse);
            (W, H) = (W + (int)LeftUpCorner.Y, H + (int)LeftUpCorner.X + 1);

            if (W < 0 || W >= CurrentLocation.Width || H < 0 || H >= CurrentLocation.Height) return;

            SystemSkelet people;
            if (H + 1 < CurrentLocation.Height && CurrentLocation.FindLives(H + 1, W) != null)
            {
                people = CurrentLocation.FindLives(H + 1, W);
            }
            else
            {
                people = CurrentLocation.FindLives(H, W);
            }
            if (people == null) return;

            if (people is DoorSkelet)
            {
                ClickOnDoor(people as DoorSkelet);
            }
            else if (people is StorageSkelet)
            {
                ClickOnBox(people as StorageSkelet);
            }
            else if (!(people is PlayerSkelet) && people is AliveSkelet)
            {
                ClickOnSkelet(people as AliveSkelet);
            }
        }
        private (int, int) FindPointClick(Point point)
        {
            return ((int)Math.Truncate(point.X / (pixelSizeGamePole * compressW)), (int)Math.Truncate(point.Y / (pixelSizeGamePole * compressH)) - 1);
        }
        private void KeyDownToMove(int H, int W)
        {
            if (W < 0 || W >= CurrentLocation.Width || H < 0 || H >= CurrentLocation.Height) return;

            string nameloc = CurrentLocation.GetSystemNameLocTransitCell(H, W);
            if (nameloc != null)
            {
                player.ClearActions();
                player.ClearGlobalActions();
                GoToLocation(nameloc);
            }
            else if (!CurrentLocation.GetIsBlockCell(H, W))
            {
                player.ClearActions();
                player.ClearGlobalActions();
                player.EnqueueUpGlobalAction(new ActionMove(new GamePoint(H, W), false));

                //случай перехода на новую локацию или застревания где-либо
            }
            ChangeTimerInterval();
        }
        private void ClickOnWatch(int H, int W)
        {
            bool IsFind = CurrentLocation.IsDescriptionsCell(H, W, out string description);
            if (IsFind)
                WatchTextBlock.Text = description;
            else
                WatchTextBlock.Text = "";
        }
        private void ClickOnDoor(DoorSkelet door)
        {
            bool ShelterKey = false; //(CurrentLocation.GetIsWatchCell((int)people.Cord.X, (int)people.Cord.Y));

            StackPanel menu = new StackPanel();
            Canvas.SetLeft(menu, pixelSizeGamePole * compressW * (door.GPoint.Y + 1 - LeftUpCorner.Y));
            Canvas.SetTop(menu, pixelSizeGamePole * compressH * (door.GPoint.X - LeftUpCorner.X));

            Button Open = new Button()
            {
                Content = "Открыть",
                Opacity = 0.6,
                FontSize = 20,
            };
            Open.Click += MenuDoorOpen_Click;
            Open.Tag = door;

            double dx = Math.Abs(door.GPoint.X - player.GPoint.X);
            double dy = Math.Abs(door.GPoint.Y - player.GPoint.Y);
            bool Near = (dx <= 2 && dy <= 1) || (dx <= 1 && dy <= 2);
            if (ShelterKey || !Near)
            {
                Open.IsEnabled = false;
            }
            if (door.IsLock)
            {
                Open.IsEnabled = false;
                Open.Content = "Заперто";
            }
            menu.Children.Add(Open);
            SystemObj.Add(menu);

        }
        private void ClickOnSkelet(AliveSkelet people)
        {
            bool ShelterKey = false; //(CurrentLocation.GetIsWatchCell((int)people.Cord.X, (int)people.Cord.Y));

            StackPanel menu = new StackPanel();
            Canvas.SetLeft(menu, pixelSizeGamePole * compressW * (people.GPoint.Y + 1 - LeftUpCorner.Y));
            Canvas.SetTop(menu, pixelSizeGamePole * compressH * (people.GPoint.X - LeftUpCorner.X));

            Button Attack = new Button()
            {
                Content = "Атаковать",
                Opacity = 0.6,
                FontSize = 20,
            };
            Attack.Click += MenuPersonAttack_Click;
            Attack.Tag = people;

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

            double dx = Math.Abs(people.GPoint.X - player.GPoint.X);
            double dy = Math.Abs(people.GPoint.Y - player.GPoint.Y);
            bool Near = (dx <= 2 && dy <= 1) || (dx <= 1 && dy <= 2);
            if (ShelterKey || !Near)
            {
                Check.IsEnabled = false;
                Dialog.IsEnabled = false;
            }
            if (!people.IsAlive)
            {
                menu.Children.Add(Check);
            }
            else
            {
                menu.Children.Add(Attack);
            }

            if (people.IsAlive && player.FriendFranction.Contains(people.Fraction))
            {
                menu.Children.Add(Dialog);
            }
            SystemObj.Add(menu);

        }
        private void ClickOnBox(StorageSkelet people)
        {
            bool ShelterKey = false; //(CurrentLocation.GetIsWatchCell((int)people.Cord.X, (int)people.Cord.Y));

            StackPanel menu = new StackPanel();
            Canvas.SetLeft(menu, pixelSizeGamePole * compressW * (people.GPoint.Y + 1 - LeftUpCorner.Y));
            Canvas.SetTop(menu, pixelSizeGamePole * compressH * (people.GPoint.X - LeftUpCorner.X));

            Button Check = new Button()
            {
                Content = "Обыскать",
                Opacity = 0.6,
                FontSize = 20,
            };
            Check.Click += MenuPersonCheck_Click;
            Check.Tag = people;

            double dx = Math.Abs(people.GPoint.X - player.GPoint.X);
            double dy = Math.Abs(people.GPoint.Y - player.GPoint.Y);
            bool Near = (dx <= 2 && dy <= 1) || (dx <= 1 && dy <= 2);
            if (ShelterKey || !Near)
            {
                Check.IsEnabled = false;
            }
            menu.Children.Add(Check);
            SystemObj.Add(menu);
        }

        private void MenuDoorOpen_Click(object sender, RoutedEventArgs e)
        {
            SystemObj = new List<UIElement>();
            Button button = (Button)sender;
            DoorSkelet door = (DoorSkelet)button.Tag;
            MenuDoorOpen_Click(door);
        }
        private void MenuDoorOpen_Click(DoorSkelet door)
        {
            CurrentLocation.RemoveFirstLayerWithCell(door);
        }
        private void MenuPersonAttack_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            AliveSkelet people = (AliveSkelet)button.Tag;
            MenuPersonAttack_Click(people);
        }
        private void MenuPersonAttack_Click(AliveSkelet people)
        {
            player.ClearActions();
            player.ClearGlobalActions();
            player.EnqueueUpGlobalAction(new ActionAttack(people, false));
            ChangeTimerInterval();
        }
        private void MenuPersonCheck_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            StorageSkelet box = (StorageSkelet)button.Tag;
            MenuPersonCheck_Click(box);
        }
        private void MenuPersonCheck_Click(StorageSkelet box)
        {
            if (InventoryEnemyWindow.Visibility == Visibility.Collapsed)
            {
                OpenInventoryWindow(box);
            }
        }
        private void MenuPersonDialog_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            AliveSkelet people = (AliveSkelet)button.Tag;
            MenuPersonDialog_Click(people);
        }
        public void MenuPersonDialog_Click(AliveSkelet people)
        {
            if (DialogWindow.Visibility == Visibility.Collapsed)
            {
                timerReloadAnimation.IsEnabled = false;
                SystemObj = new List<UIElement>();
                DialogWindow.Visibility = Visibility.Visible;
                CreateWindowDialog(people);
            }
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
        private void CreateWindowDialog(AliveSkelet Person)
        {
            namePerson = Person.Name;
            DialogInform informPhrases = Information.GetPhrasesPerson(Person.SystemName, lengthPhrase);

            var str = GetStartPhraseInDialog(informPhrases.StartedTasksPerson);
            if (str.Item1 != null)
            {
                phrases = informPhrases.Phrases;
                if (str.Item2)
                    ExitDialogBtn.IsEnabled = false;
                CreateDialog(str.Item1);
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
        private (string, bool) GetStartPhraseInDialog(List<(string, string, bool)> startedPhrases)
        {
            foreach (var phraseTask in startedPhrases)
            {
                foreach (var task in player.Tasks.GetUsingTask())
                {
                    if (task.SystemName == phraseTask.Item2)
                        return (phraseTask.Item1, phraseTask.Item3);
                }
            }
            return (null, false);
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

            if (phrases[index].NextSystemNames.Count == 0)
            {
                ExitDialogBtn.IsEnabled = true;
            }
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
            else
            {
                ExitDialogBtn.IsEnabled = true;
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

        private void InventoryBtn_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryPlayerWindow.Visibility == Visibility.Collapsed)
            {
                timerReloadAnimation.IsEnabled = false;
                SystemObj = new List<UIElement>();
                InventoryPlayerWindow.Visibility = Visibility.Visible;
                player.DisplayInventory(InventoryPlayerCanvas, pixelSizeInvent);
            }
            else
            {
                timerReloadAnimation.IsEnabled = true;
                InventoryPlayerWindow.Visibility = Visibility.Collapsed;
            }
        }
        private void ExitInventorySearchBtn_Click(object sender, RoutedEventArgs e)
        {
            InventoryEnemyWindow.Visibility = Visibility.Collapsed;
            timerReloadAnimation.IsEnabled = true;
        }
        private void OpenInventoryWindow(StorageSkelet skelet)
        {
            timerReloadAnimation.IsEnabled = false;
            SystemObj = new List<UIElement>();
            InventoryEnemyWindow.Visibility = Visibility.Visible;

            InventoryEnemyCanvas.Tag = skelet;
            skelet.DisplayInventory(InventoryEnemyCanvas, pixelSizeInvent);
        }
        private void InventoryHotBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Point pointMouse = e.GetPosition(InventoryHotBar);
            //(int W, int H) = ((int)Math.Truncate((pointMouse.X) / pixelSizeInvent),
            //    (int)Math.Truncate((pointMouse.Y) / pixelSizeInvent));
            //Item item = player.SearchInBackpack(H, W);
            //if (item != null)
            //{
            //    if (InventorySearchWindow.Visibility != Visibility.Collapsed)
            //    {
            //        IBoxSkelet skelet = (IBoxSkelet)InventoryCanvas.Tag;
            //        skelet.AddInBackpack(item);
            //        player.RemoveInBackpack(item);

            //        player.DisplayInventory(InventoryHotBar, pixelSizeInvent);
            //        skelet.DisplayInventory(InventoryCanvas, pixelSizeInvent);
            //    }
            //    else
            //    {
            //        if (item is NoteBook)
            //        {
            //            MenuTask_Click(null, null);
            //        }
            //    }
            //}
        }
        private void InventoryHotBar_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void InventoryPlayerCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (InventoryEnemyWindow.Visibility != Visibility.Collapsed)
            {
                Point pointMouse = e.GetPosition(InventoryPlayerCanvas);
                (int W, int H) = ((int)Math.Truncate((pointMouse.X) / pixelSizeInvent),
                    (int)Math.Truncate((pointMouse.Y) / pixelSizeInvent));
                Item item = player.SearchInBackpack(H, W);

                var skelet = InventoryEnemyCanvas.Tag;
                if (item != null)
                {
                    player.RemoveInBackpack(item);
                    (skelet as IBoxElement).AddInBackpack(item);

                    player.DisplayInventory(InventoryPlayerCanvas, pixelSizeInvent);
                    (skelet as IDisplayCanvas).Display(InventoryEnemyCanvas, pixelSizeInvent);
                }
            }
        }
        private void InventoryPlayerCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void InventoryEnemyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pointMouse = e.GetPosition(InventoryEnemyCanvas);
            (int W, int H) = ((int)Math.Truncate((pointMouse.X) / pixelSizeInvent),
                (int)Math.Truncate((pointMouse.Y) / pixelSizeInvent));
            var skelet = InventoryEnemyCanvas.Tag;
            Item item = (skelet as IBoxElement).SearchInBackpack(H, W);
            if (item != null)
            {
                (skelet as IBoxElement).RemoveInBackpack(item);
                player.AddInBackpack(item);

                player.DisplayInventory(InventoryPlayerCanvas, pixelSizeInvent);
                (skelet as IDisplayCanvas).Display(InventoryEnemyCanvas, pixelSizeInvent);
            }
        }
        private void InventoryEnemyCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        public bool CanTalkVanya = false;
        private void ClickOnTelephone(object sender, MouseButtonEventArgs e)
        {
        }
        private void MenuCall_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            AliveSkelet people = (AliveSkelet)button.Tag;

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
