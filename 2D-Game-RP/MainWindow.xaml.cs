using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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

        int sizeInventH;
        int sizeInventW;
        double pixelSizeInvent;

        public Player player;

        bool ToSeePlayer = false;
        bool ToSeeEnemy = false;

        private static readonly int CountTimeAnimation = 250;
        private DispatcherTimer timerReloadAnimation = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(CountTimeAnimation)
        };
        private void ChangeSizeGamePole(int height, int wight, GamePoint player)
        {
            if (height % 2 == 0 && height != CurrentLocation.Height) throw new Exception("Размеры поля должны быть нечетными!");
            if (wight % 2 == 0 && wight != CurrentLocation.Width) throw new Exception("Размеры поля должны быть нечетными!");
            sizeGamePoleH = height;
            sizeGamePoleW = wight;
            pixelSizeGamePole = Math.Min(Map.ActualHeight / (sizeGamePoleH + 2), Map.ActualWidth / (sizeGamePoleW + 2));
            LeftUpCorner = new GamePoint(player.X - (height - 1) / 2, player.Y - (wight - 1) / 2);
            if (LeftUpCorner.X < 0) LeftUpCorner.X = 0;
            if (LeftUpCorner.Y < 0) LeftUpCorner.Y = 0;
            if (LeftUpCorner.X > CurrentLocation.Height - sizeGamePoleH) LeftUpCorner.X = CurrentLocation.Height - sizeGamePoleH;
            if (LeftUpCorner.Y > CurrentLocation.Width - sizeGamePoleW) LeftUpCorner.Y = CurrentLocation.Width - sizeGamePoleW;
        }
        private void ChangeSizeInventoryPlayer(int height, int wight)
        {
            sizeInventH = height;
            sizeInventW = wight;
            pixelSizeInvent = Math.Min(InventoryPlayer.ActualHeight / (sizeInventH + 2), InventoryPlayer.ActualWidth / (sizeInventW + 2));
        }

        int x = 14;
        int y = 8;
        public MainWindow()
        {
            //Information.Serialization();

            InitializeComponent();
            Information.CreateDarkenPicCell();
            Time = 0;
            SystemObj = new List<UIElement>();
            timerReloadAnimation.Tick += TimerAnimation_Tick;
            timerReloadAnimation.IsEnabled = true;

            AddInformationPlayer("Герой", PlayerGender.Man, x, y, null,null, 1300, new List<Item>()
                );
            player.Tasks.Add(Information.FindTask("ГлавныйКвест"));
            player.Tasks.Add(Information.FindTask("СпроситьСталкеров"));

            GoToLocation("Garden");
            TimerAnimation_Tick(null, null);
        }
        public MainWindow(string PlayerName, PlayerGender Gender)
        {
            //Information.Serialization();

            InitializeComponent();
            Information.CreateDarkenPicCell();
            Time = 0;
            SystemObj = new List<UIElement>();
            timerReloadAnimation.Tick += TimerAnimation_Tick;
            timerReloadAnimation.IsEnabled = true;

            AddInformationPlayer(PlayerName, Gender, x, y, null,null, 1300, new List<Item>()
                );
            player.Tasks.Add(Information.FindTask("ГлавныйКвест"));
            player.Tasks.Add(Information.FindTask("СпроситьСталкеров"));

            GoToLocation("Garden");
            TimerAnimation_Tick(null, null);
        }
        private void AddInformationPlayer(string PlayerName, PlayerGender gender, int x, int y, Gun gun, Cloth cloth, int money, List<Item> inventory)
        {
            player = new Player(PlayerName, gender, new GamePoint(x, y), gun, cloth, money, inventory,
                new List<NPSGroup>() { { NPSGroup.Stalker }, { NPSGroup.Box } });
            //PicturePlayer.Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{player.SystemName}.png"), UriKind.Relative));
        }

        private int Time;
        private List<UIElement> SystemObj;
        private int oblwatch = 10;
        private int oblsee = 7;
        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            NamePlayer.Content = $"{Time}";
            Time++;

            DoActionAll();
            //if (ToSeePlayer)
            //if (ToSeeEnemy)
            if (ToSeePlayer)
            {
                ChangeSizeGamePole(oblwatch * 2 + 1, oblwatch * 2 + 1, player.Cord);
                SortedEnum<GamePoint> deleted = new SortedEnum<GamePoint>();
                foreach (var v in CurrentLocation.GetLives())
                {
                    if (!v.IsClarity) deleted.Add(v.Cord);
                }
                var OblSee = CurrentLocation.GrafLocToWatch.SearchSeeInCircleWithBlocksWithousSomePoint(player.Cord, oblsee,
                    Math.Max((int)player.Cord.X - oblsee, 0), Math.Max((int)player.Cord.Y - oblsee, 0),
                    Math.Min((int)player.Cord.X + oblsee + 1, CurrentLocation.Height), Math.Min((int)player.Cord.Y + oblsee + 1, CurrentLocation.Width),
                    deleted);
                var OblWatch = CurrentLocation.GrafLocToWatch.SearchSeeInCircle(player.Cord, oblwatch,
                    Math.Max((int)player.Cord.X - oblwatch, 0), Math.Max((int)player.Cord.Y - oblwatch, 0),
                    Math.Min((int)player.Cord.X + oblwatch + 1, CurrentLocation.Height), Math.Min((int)player.Cord.Y + oblwatch + 1, CurrentLocation.Width));
                //var SystemRamka = new StaticPicCell(Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"System\\Ramka.png"));
                //foreach (var v in OblSee)
                //{
                //    CurrentLocation.AddCell(SystemRamka, 4, (int)v.X, (int)v.Y);

                //    //Image newIm = new Image()
                //    //{
                //    //    Source = new BitmapImage(new Uri(pict.Picture(), UriKind.Relative)),
                //    //    Tag = new KeyValuePair<string, int>(pict.Picture(), 0)
                //    //};
                //}
                CurrentLocation.DisplayToPointsWithBorderAndView(OblSee, OblWatch, LeftUpCorner, Map, pixelSizeGamePole, SystemObj);
                //foreach (var v in OblSee)
                //{
                //    CurrentLocation.RemoveCell(SystemRamka, (int)v.X, (int)v.Y);
                //    //сделать потом более грамотное (выборочное) удаление и добавление новых системных изображений
                //}
            }
            else
            {
                ChangeSizeGamePole(CurrentLocation.Height, CurrentLocation.Width, player.Cord);
                CurrentLocation.Display(Map, pixelSizeGamePole, SystemObj);
            }

            ChangeSizeInventoryPlayer(7, 4);
            player.DisplayInventory(InventoryPlayer, pixelSizeInvent);
            //selectLevel.TakeNextPictureLevel();
        }
        private void DoActionAll()
        {
            Location current = CurrentLocation;
            foreach (var v in current.GetLives())
            {
                if (v.PeekGlobalAction() == null) continue;
                if (v.PeekAction() == null) v.CreateActions(v.PeekGlobalAction().CreateActions(v, current));
                
                if (!v.PeekAction().IsCanComplete(v, current))
                {
                    v.ClearActions();
                    v.CreateActions(v.PeekGlobalAction().CreateActions(v, current));
                }
                if (!v.PeekAction().IsCanComplete(v, current)) continue;

                v.PeekAction().CompleteAction(v, current);
                v.RemoveAction();
                while (v.PeekAction() != null && v.PeekAction().IsSystem)
                {
                    v.PeekAction().CompleteAction(v, current);
                    v.RemoveAction();
                }

            }
        }

        Location CurrentLocation;
        public List<Location> Locations = new List<Location>();
        private void GoToLocation(string name)
        {
            //сброс персонажа при переходе из локации

            if (!Locations.Exists(x => x.SystemName == name))
            {
                Location newLoc = Information.GetGardenLocation();
                CurrentLocation = newLoc;

                //создание переходов

                CurrentLocation.AddLivesWithCell(player);

                GamePoint p = new GamePoint(12, 12);

                TestSkelet stalker = new TestSkelet("Бегающий", "ф", null, NPSIntellect.StandAgressive, p, 0, new List<Item>(), new List<NPSGroup>() { { NPSGroup.Stalker }, { NPSGroup.Box } });
                stalker.EnqueueDownGlobalAction(new ActionMove(new GamePoint(11, 16), true));
                stalker.EnqueueDownGlobalAction(new ActionWait(4, true));
                stalker.EnqueueDownGlobalAction(new ActionMove(new GamePoint(12, 12), true));
                stalker.EnqueueDownGlobalAction(new ActionWait(4, true));
                CurrentLocation.AddLivesWithCell(stalker);

                GamePoint p2 = new GamePoint(12, 6);

                TestSkelet stalker2 = new TestSkelet("Зависший", "ф", null, NPSIntellect.StandAgressive, p2, 0, new List<Item>(), new List<NPSGroup>() { { NPSGroup.Stalker }, { NPSGroup.Box } });
                CurrentLocation.AddLivesWithCell(stalker2);
            }
            ChangeSizeGamePole(CurrentLocation.Height, CurrentLocation.Width, player.Cord);
            //pixelSize = Math.Min( Map.Height / (CurrentLocation.Height + 2) , Map.Width / (CurrentLocation.Width + 2));
            //выставление начальной позиции при переходе на локации
        }

        private void AddItem(Item item)
        {
            player.InventoryList.Add(item);
        }

        //-------------------------------------------------------------------------------Reload Create

        private void ReloadPlayerInformation()
        {
            if (!player.IsAlive) EndGame();
            NamePlayer.Content = $"Имя: {player.Name}";
            //FractionPlayer.Content = $"Фракция: {player.FractionString()}";
            //HealthPlayer.MaxHeight = 100;
            //HealthPlayer.Value = player.HealthInf();
            //MoneyPlayer.Content = $"Деньги: {player.Money}";

            //Image imageArmorPlayer = new Image()
            //{
            //    Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"],
            //        $"{player.ClothIng().SystemName}.png"), UriKind.Relative)),
            //};
            //Image imageGunPlayer = new Image()
            //{
            //    Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"],
            //        $"{player.GunInf().SystemName}.png"), UriKind.Relative)),
            //};
            //ArmorPlayer.Content = imageArmorPlayer;
            //GunPlayer.Content = imageGunPlayer;
            CreateAndReloadInventory();
        }
        private void CreateAndReloadInventory()
        {
            //int indi = 0; int indj = 0;
            //InventoryPlayer.Children.Clear();

            //foreach (Item item in player.InventoryList.ReferenceItem.Keys)
            //{
            //    foreach (GamePoint point in player.InventoryList.ReferenceItem[item])
            //    {
            //        Button itemBut = new Button();
            //        itemBut.Tag = item.SystemName;
            //        itemBut.Click += ItemBut_Click;
            //        itemBut.Width = pixelIY;
            //        itemBut.Height = pixelIX;
            //        Canvas.SetLeft(itemBut, pixelIOtstY / 2 + pixelIY * indj);
            //        Canvas.SetTop(itemBut, pixelIOtstX / 2 + pixelIX * indi);

            //        Image image = new Image()
            //        {
            //            Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"],
            //            $"{item.SystemName}.png"), UriKind.Relative)),
            //        };
            //        itemBut.Content = image;

            //        InventoryPlayer.Children.Add(itemBut);

            //        indj++;
            //        if (indj >= inventY)
            //        {
            //            indj = 0; indi++;
            //        }
            //    }                
            //}

            //while (indi < inventX)
            //{
            //    Button itemEmp = new Button();
            //    itemEmp.Width = pixelIY;
            //    itemEmp.Height = pixelIX;
            //    Canvas.SetLeft(itemEmp, pixelIOtstY / 2 + pixelIY * indj);
            //    Canvas.SetTop(itemEmp, pixelIOtstX / 2 + pixelIX * indi);

            //    Image imageBut = new Image()
            //    {
            //        Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"],
            //        "emptyitem.png"), UriKind.Relative)),
            //    };
            //    itemEmp.Content = imageBut;

            //    InventoryPlayer.Children.Add(itemEmp);

            //    indj++;
            //    if (indj >= inventY)
            //    {
            //        indj = 0; indi++;
            //    }
            //}
        }

        //-------------------------------------------------------------------------------Move
        private void MakeMovePerson(Skelet Person, GamePoint point)
        {
            //Location current = CurrentLocation;
            //current.RemoveCell(Person.picture, (int)Person.Cord.X, (int)Person.Cord.Y);
            //current.AddCell(Person.picture, 1, (int)Person.Cord.X, (int)Person.Cord.Y);
            //Person.Cord = new GamePoint(point.X, point.Y);

            //return;

            //int x = (int)point.X; int y = (int)point.Y;
            //if (x >= sizeX || x < 0 || y < 0 || y >= sizeY)
            //{
            //    return;
            //}

            //int cordX = (int)Person.Cord.X;
            //int cordY = (int)Person.Cord.Y;
            //if (Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] != null)
            //{

            //}
            //else if (Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y] == null ||
            //    Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y] == "shelter")
            //{
            //    Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] = Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[cordX, cordY];
            //    Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[cordX, cordY] = null;
            //    Person.Going(new GamePoint(x, y), Locations.Find(X => X.SystemName == NameCurrentLocation));
            //}
            //else if (Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y] == "anomaly")
            //{
            //    Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] = Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[cordX, cordY];
            //    Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[cordX, cordY] = null;
            //    Person.Going(new GamePoint(x, y), Locations.Find(X => X.SystemName == NameCurrentLocation));
            //    Person.Damaging(30);
            //}
            //else if (Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y].Split('-')[0] == "trans" && Person is Player)
            //{
            //    if (player.Tasks.Contains(Information.FindTask("ПереходЗавод")) &&
            //        Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y].Split('-')[1] == "zavod")
            //    {
            //        player.Tasks.Remove(Information.FindTask("ПереходЗавод"));
            //        player.CompliteTasks.Add(Information.FindTask("ПереходЗавод"));
            //        player.Tasks.Add(Information.FindTask("УбитьНаёмниковЗавод"));
            //    }
            //    else if (player.Tasks.Contains(Information.FindTask("ПереходТопи")) &&
            //        Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y].Split('-')[1] == "boloto")
            //    {
            //        player.Tasks.Remove(Information.FindTask("ПереходТопи"));
            //        player.CompliteTasks.Add(Information.FindTask("ПереходТопи"));
            //        player.Tasks.Add(Information.FindTask("НайтиАртефакт"));
            //    }
            //    else if (player.Tasks.Contains(Information.FindTask("ПереходАномалии")) &&
            //        Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y].Split('-')[1] == "electr")
            //    {
            //        player.Tasks.Remove(Information.FindTask("ПереходАномалии"));
            //        player.CompliteTasks.Add(Information.FindTask("ПереходАномалии"));
            //        player.Tasks.Add(Information.FindTask("НайтиФамильноеРужьё"));
            //    }

            //    Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[(int)Person.Cord.X, (int)Person.Cord.Y] = null;

            //    if (Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y].Split('-')[1] == "final")
            //    {
            //        EndGame();
            //    }

            //    MoveToLocation(Person, Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y].Split('-')[1]);
            //}
        }

        private Skelet CheckPlayer(Skelet Person)
        {
            int leng = int.MaxValue;
            Skelet personEnemy = null;
            Location current = CurrentLocation;
            foreach (Skelet enemy in current.GetLives())
            {
                if (Person.OblSee.Contains(enemy.Cord) && (enemy.IsAlive) && (!Person.FriendFranction.Contains(enemy.FractionInf())))
                {
                    int between = current.GrafLocToWatch.SearchWidth(Person.Cord, enemy.Cord).Count;
                    if (between < leng)
                    {
                        leng = between;
                        personEnemy = enemy;
                    }
                }
            }
            return personEnemy;
        }

        //-------------------------------------------------------------------------------Click

        private void InventoryPlayer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point pointMouse = e.GetPosition(InventoryPlayer);
            (int W, int H) = ((int)Math.Truncate(pointMouse.X / pixelSizeInvent) - 1, (int)Math.Truncate(pointMouse.Y / pixelSizeInvent) - 1);
            Item item = player.InventoryList.SearchItem(H, W);
            if (item != null)
            {
                item.Using(player);
                player.InventoryList.Remove(item);
            }
        }

        private void InventoryPlayer_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
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
        }
        private void Map_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SystemObj = new List<UIElement>();
            Point pointMouse = e.GetPosition(Map);
            (int W, int H) = ((int)Math.Truncate(pointMouse.X / pixelSizeGamePole) - 1, (int)Math.Truncate(pointMouse.Y / pixelSizeGamePole) - 1);
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
            (int W, int H) = ((int)Math.Truncate(pointMouse.X / pixelSizeGamePole) - 1, (int)Math.Truncate(pointMouse.Y / pixelSizeGamePole) - 1);
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
            Canvas.SetLeft(menu, pixelSizeGamePole + pixelSizeGamePole * (door.Cord.Y + 1 - LeftUpCorner.Y));
            Canvas.SetTop(menu, pixelSizeGamePole + pixelSizeGamePole * (door.Cord.X - LeftUpCorner.X));

            Button Open = new Button()
            {
                Content = "Открыть",
                Opacity = 0.6,
                FontSize = 10,
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
            menu.Children.Add(Open);
            SystemObj.Add(menu);

        }
        private void ClickOnSkelet(Skelet people)
        {
            bool ShelterKey = false; //(CurrentLocation.GetIsWatchCell((int)people.Cord.X, (int)people.Cord.Y));

            StackPanel menu = new StackPanel();
            Canvas.SetLeft(menu, pixelSizeGamePole + pixelSizeGamePole * (people.Cord.Y + 1 - LeftUpCorner.Y));
            Canvas.SetTop(menu, pixelSizeGamePole + pixelSizeGamePole * (people.Cord.X - LeftUpCorner.X));

            //Button Information = new Button()
            //{
            //    Content = "Информация",
            //    Opacity = 0.6,
            //};
            //Information.Click += MenuPersonInformation_Click;
            //Information.Tag = people;

            Button Dialog = new Button()
            {
                Content = "Поговорить",
                Opacity = 0.6,
                FontSize = 10,
            };
            Dialog.Click += MenuPersonDialog_Click;
            Dialog.Tag = people;

            //Button Check = new Button()
            //{
            //    Content = "Обыскать",
            //    Opacity = 0.6,
            //};
            //Check.Click += MenuPersonCheck_Click;
            //Check.Tag = people;

            double dx = Math.Abs(people.Cord.X - player.Cord.X);
            double dy = Math.Abs(people.Cord.Y - player.Cord.Y);
            bool Near = (dx <= 2 && dy <= 1) || (dx <= 1 && dy <= 2);
            if (ShelterKey || !Near)
            {
                //Check.IsEnabled = false;
                Dialog.IsEnabled = false;
            }
            if (people.HealthInf() <= 0 || people.FractionInf() == NPSGroup.Box)
            {
                //menu.Children.Add(Check);
            }
            else if (!player.FriendFranction.Contains(people.FractionInf()))
            {
                //menu.Children.Add(Information);
            }
            else
            {
                //menu.Children.Add(Information);
                //menu.Children.Add(new Separator());
                menu.Children.Add(Dialog);
            }
            SystemObj.Add(menu);

        }
        private void MenuDoorOpen_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Skelet door = (Skelet)button.Tag;
            CurrentLocation.RemoveSkeletWithCell(door);
        }
        //private void MenuPersonCheck_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = (Button)sender;
        //    Skelet people = (Skelet)button.Tag;

        //    foreach (Item item in people.InventoryList.ReferenceItem.Keys)
        //    {
        //        for (int i = 0; i < people.InventoryList.ReferenceItem[item].Count; i++)
        //        {
        //            AddItem(item);
        //        }
        //    }
        //    player.Money += people.Money;

        //    CurrentLocation.RemoveLivesWithCell(people);

        //    TimerAnimation_Tick(null, null);
        //}
        private void MenuPersonDialog_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Skelet people = (Skelet)button.Tag;

            DialogWindow dialog = new DialogWindow(player, people);
            timerReloadAnimation.IsEnabled = false;
            dialog.ShowDialog();
            SystemObj = new List<UIElement>();
            timerReloadAnimation.IsEnabled = true;
        }
        //private void MenuPersonInformation_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = (Button)sender;
        //    Skelet people = (Skelet)button.Tag;
        //}
        private void ItemBut_Click(object sender, RoutedEventArgs e)
        {
            Button item = (Button)sender;
            
            //foreach(Item item1 in player.InventoryList)
            //{
            //    if (item.Tag.ToString() == item1.SystemName)
            //    {
            //        item1.Using(player);
            //        if (item1 is Gun)
            //        {
            //            player.TakeGun((Gun)item1);
            //        }
            //        player.InventoryList.Remove(player.InventoryList.Find(x => x.SystemName == item.Tag.ToString()));
            //        break;
            //    }
            //}

            ReloadPlayerInformation();
        }

        //-------------------------------------------------------------------------------InOtherWindow

        public void TakeRewardTaskKillNaemnik()
        {
        }
        public void TakeTaskKillNaemnik()
        {
        }
        public void TakeTaskSearchGun()
        {
        }
        public void TakeRewardTaskSearchGun()
        {
        }
        public bool BuyThing(Item item)
        {
            if (player.Money < item.Cost) return false;
            else
            {
                player.Money -= item.Cost;
                player.InventoryList.Add(item);
                return true;
            }
        }
        public bool SellThing(Item item)
        {
            return true;
            //Item it = player.InventoryList.Find(X => X.SystemName == item.SystemName);
            //if (it == null) return false;
            //else
            //{
            //    player.Money += it.Cost;
            //    player.InventoryList.Remove(it);
            //    return true;
            //}            
        }
        //-------------------------------------------------------------------------------Window
        private void EndGame()
        {
            FinalWindow finalWindow = new FinalWindow();
            finalWindow.ShowDialog();

            this.Close();
        }
        private void PDA_Click(object sender, RoutedEventArgs e)
        {
            //PDAWindow pda = new PDAWindow(player);
            //pda.ShowDialog();
        }
        //private void ArmorPlayer_Click(object sender, RoutedEventArgs e)
        //{
        //    ToSeeEnemy = !ToSeeEnemy;
        //    ToSeePlayer = false;
        //}
        private void GunPlayer_Click(object sender, RoutedEventArgs e)
        {
            ToSeePlayer = !ToSeePlayer;
            ToSeeEnemy = false;
        }
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            //MenuWindow menu = new MenuWindow();
            //menu.Owner = this;
            //menu.MenuInGame();
            //menu.ShowDialog();
        }


    }
}
