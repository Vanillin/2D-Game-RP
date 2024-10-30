using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;
using TwoD_Game_RP;

namespace TwoD_Game_RP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double pixelIX;
        public double pixelIY;
        const double pixelIOtstX = 50;
        const double pixelIOtstY = 20;

        Point LeftUpCorner;
        int sizeGamePoleH;
        int sizeGamePoleW;
        double pixelSize;

        public Player player;

        bool ToSeePlayer = false;
        bool ToSeeEnemy = false;

        private static readonly int CountTimeAnimation = 500;
        private DispatcherTimer timerReloadAnimation = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(CountTimeAnimation)
        };
        private void ChangeSizeGamePole(int height, int wight, Point player)
        {
            if (height % 2 == 0 && height!=CurrentLocation.Height) throw new Exception("Размеры поля должны быть нечетными!");
            if (wight % 2 == 0 && wight != CurrentLocation.Width) throw new Exception("Размеры поля должны быть нечетными!");
            sizeGamePoleH = height;
            sizeGamePoleW = wight;
            pixelSize = Math.Min(Map.ActualHeight / (sizeGamePoleH + 2), Map.ActualWidth / (sizeGamePoleW + 2));
            LeftUpCorner = new Point(player.X - (height - 1) / 2, player.Y - (wight - 1) / 2);
            if (LeftUpCorner.X < 0) LeftUpCorner.X = 0;
            if (LeftUpCorner.Y < 0) LeftUpCorner.Y = 0;
            if (LeftUpCorner.X > CurrentLocation.Height - sizeGamePoleH) LeftUpCorner.X = CurrentLocation.Height - sizeGamePoleH;
            if (LeftUpCorner.Y > CurrentLocation.Width - sizeGamePoleW) LeftUpCorner.Y = CurrentLocation.Width - sizeGamePoleW;
        }
        public MainWindow()
        {
            //Information.Serialization();

            InitializeComponent();
            Time = 0;
            SystemObj = new List<UIElement>();
            timerReloadAnimation.Tick += TimerAnimation_Tick;
            timerReloadAnimation.IsEnabled = true;

            int x = 12;
            int y = 8;

            AddInformationPlayer("a", PlayerGender.Man, x, y, new Toz34(), new KurtkaStalker(), 1300, new List<Item>() { { new AidFirstKid() }, { new AidFirstKid() } });
            player.Tasks.Add(Information.FindTask("ГлавныйКвест"));
            player.Tasks.Add(Information.FindTask("СпроситьСталкеров"));

            GoToLocation("Garden");
            TimerAnimation_Tick(null, null);

            pixelIX = (int.Parse(InventoryPlayer.Height.ToString()) - pixelIOtstX) / (player.InventoryList.MaxSizeH);
            pixelIY = (int.Parse(InventoryPlayer.Width.ToString()) - pixelIOtstY) / (player.InventoryList.MaxSizeW);
        }
        public MainWindow(string PlayerName, PlayerGender Gender)
        {
            //Information.Serialization();

            InitializeComponent();
            Time = 0;
            SystemObj = new List<UIElement>();
            timerReloadAnimation.Tick += TimerAnimation_Tick;
            timerReloadAnimation.IsEnabled = true;

            int x = 12;
            int y = 8;

            AddInformationPlayer(PlayerName, Gender, x, y, new Toz34(), new KurtkaStalker(), 1300, new List<Item>() { { new AidFirstKid() }, { new AidFirstKid() } });
            player.Tasks.Add(Information.FindTask("ГлавныйКвест"));
            player.Tasks.Add(Information.FindTask("СпроситьСталкеров"));

            GoToLocation("Garden");
            TimerAnimation_Tick(null, null);

            pixelIX = (int.Parse(InventoryPlayer.Height.ToString()) - pixelIOtstX) / (player.InventoryList.MaxSizeH);
            pixelIY = (int.Parse(InventoryPlayer.Width.ToString()) - pixelIOtstY) / (player.InventoryList.MaxSizeW);
        }
        private void AddInformationPlayer(string PlayerName, PlayerGender gender, int x, int y, Gun gun, Cloth cloth, int money, List<Item> inventory)
        {
            player = new Player(PlayerName, gender, new Point(x, y), gun, cloth, money, inventory,
                new List<NPSGroup>() { { NPSGroup.Stalker }, { NPSGroup.Box } });
            PicturePlayer.Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{player.SystemName}.png"), UriKind.Relative));
        }

        private int Time;
        private List<UIElement> SystemObj;
        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            NamePlayer.Content = $"{Time}";  
            Time++;  

            DoActionAll();
            //if (ToSeePlayer)
            //if (ToSeeEnemy)
            if (ToSeePlayer)
            {
                int oblsee = 7;
                ChangeSizeGamePole(oblsee*2+1, oblsee * 2 + 1, player.Cord);
                var OblSee = CurrentLocation.GrafLocToWatch.SearchSeeInCircleWithBlocks(player.Cord, oblsee, 
                    Math.Max((int)player.Cord.X- oblsee, 0), Math.Max((int)player.Cord.Y - oblsee, 0),
                    Math.Min((int)player.Cord.X + oblsee + 1, CurrentLocation.Height), Math.Min((int)player.Cord.Y + oblsee + 1, CurrentLocation.Width));
                //var OblSee = CurrentLocation.GrafLocToWatch.SearchSeeWithBlocks(player.Cord, 7, 0, 0, CurrentLocation.Height, CurrentLocation.Width);
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
                CurrentLocation.DisplayToPointsWithBorder(OblSee, LeftUpCorner, Map, pixelSize, SystemObj);
                //foreach (var v in OblSee)
                //{
                //    CurrentLocation.RemoveCell(SystemRamka, (int)v.X, (int)v.Y);
                //    //сделать потом более грамотное (выборочное) удаление и добавление новых системных изображений
                //}
            }
            else
            {
                ChangeSizeGamePole(CurrentLocation.Height, CurrentLocation.Width, player.Cord);
                CurrentLocation.Display(Map, pixelSize, SystemObj);
            }

            //selectLevel.TakeNextPictureLevel();
        }
        private void DoActionAll()
        {
            Location current = CurrentLocation;
            foreach (var v in current.Lives)
            {
                if (v.PeekGlobalAction() == null) continue;
                if (v.PeekAction() == null) v.CreateActions(v.PeekGlobalAction().CreateActions(v, current.GrafLocToMove));
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

                CurrentLocation.Lives.Add(player);
                CurrentLocation.AddCell(player.picture, 1, (int)player.Cord.X, (int)player.Cord.Y);

                //Point p = new Point(12, 10);

                //StalkerSmall stalker = new StalkerSmall("a", "a", null, NPSIntellect.StandAgressive, p, 0, new List<Item>(), new List<NPSGroup>() { { NPSGroup.Stalker }, { NPSGroup.Box } });
                //CurrentLocation.AddCell(stalker.picture, 1, (int)stalker.Cord.X, (int)stalker.Cord.Y);
                //CurrentLocation.Lives.Add(stalker);
                //stalker.EnqueueDownGlobalAction(new ActionMove(new Point(12, 11), true));
                //stalker.EnqueueDownGlobalAction(new ActionMove(new Point(12, 10), true));

                //Point p2 = new Point(12, 6);

                //StalkerSmall stalker2 = new StalkerSmall("a", "a", null, NPSIntellect.StandAgressive, p2, 0, new List<Item>(), new List<NPSGroup>() { { NPSGroup.Stalker }, { NPSGroup.Box } });
                //CurrentLocation.AddCell(stalker2.picture, 1, (int)stalker2.Cord.X, (int)stalker2.Cord.Y);
                //CurrentLocation.Lives.Add(stalker2);
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
            FractionPlayer.Content = $"Фракция: {player.FractionString()}";
            HealthPlayer.MaxHeight = 100;
            HealthPlayer.Value = player.HealthInf();
            MoneyPlayer.Content = $"Деньги: {player.Money}";

            Image imageArmorPlayer = new Image()
            {
                Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"],
                    $"{player.ClothIng().SystemName}.png"), UriKind.Relative)),
            };
            Image imageGunPlayer = new Image()
            {
                Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"],
                    $"{player.GunInf().SystemName}.png"), UriKind.Relative)),
            };
            ArmorPlayer.Content = imageArmorPlayer;
            GunPlayer.Content = imageGunPlayer;
            CreateAndReloadInventory();
        }
        private void CreateAndReloadInventory()
        {
            //int indi = 0; int indj = 0;
            //InventoryPlayer.Children.Clear();

            //foreach (Item item in player.InventoryList.ReferenceItem.Keys)
            //{
            //    foreach (Point point in player.InventoryList.ReferenceItem[item])
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
        private void MakeMovePerson(Skelet Person, Point point)
        {
            //Location current = CurrentLocation;
            //current.RemoveCell(Person.picture, (int)Person.Cord.X, (int)Person.Cord.Y);
            //current.AddCell(Person.picture, 1, (int)Person.Cord.X, (int)Person.Cord.Y);
            //Person.Cord = new Point(point.X, point.Y);

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
            //    Person.Going(new Point(x, y), Locations.Find(X => X.SystemName == NameCurrentLocation));
            //}
            //else if (Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y] == "anomaly")
            //{
            //    Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] = Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[cordX, cordY];
            //    Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[cordX, cordY] = null;
            //    Person.Going(new Point(x, y), Locations.Find(X => X.SystemName == NameCurrentLocation));
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
            foreach (Skelet enemy in current.Lives)
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

        private void Map_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SystemObj = new List<UIElement>();
            Point pointMouse = e.GetPosition(Map);
            (int W, int H) = ((int)Math.Truncate(pointMouse.X / pixelSize) - 1, (int)Math.Truncate(pointMouse.Y / pixelSize) - 1);
            (W, H) = (W + (int)LeftUpCorner.Y, H + (int)LeftUpCorner.X);
            if (W < 0 || W >= CurrentLocation.Width || H < 0 || H >= CurrentLocation.Height) return;
            if (!CurrentLocation.GetIsBlockCell(H, W))
            {
                player.EnqueueUpGlobalAction(new ActionMove(new Point(H, W), false));

                //случай перехода на новую локацию или застревания где-либо
            }
        }
        private void Map_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SystemObj = new List<UIElement>();
            Point pointMouse = e.GetPosition(Map);
            (int W, int H) = ((int)Math.Truncate(pointMouse.X / pixelSize) - 1, (int)Math.Truncate(pointMouse.Y / pixelSize) - 1);
            (W, H) = (W + (int)LeftUpCorner.Y, H + (int)LeftUpCorner.X);
            if (W < 0 || W >= CurrentLocation.Width || H < 0 || H >= CurrentLocation.Height) return;
            Skelet people = CurrentLocation.Lives.Find(x => x.Cord == new Point(H, W));
            if (people != null && !(people is Player))
            {
                ClickOnSkelet(people);
            }
        }
        private void ClickOnSkelet(Skelet people)
        {
            bool ShelterKey = false; //(CurrentLocation.GetIsWatchCell((int)people.Cord.X, (int)people.Cord.Y));

            StackPanel menu = new StackPanel();
            Canvas.SetLeft(menu, pixelSize + pixelSize * (people.Cord.Y + 1));
            Canvas.SetTop(menu, pixelSize + pixelSize * people.Cord.X);

            Button Information = new Button()
            {
                Content = "Информация",
                Opacity = 0.6,
            };
            Information.Click += MenuPersonInformation_Click;
            Information.Tag = people;

            Button Dialog = new Button()
            {
                Content = "Поговорить",
                Opacity = 0.6,
            };
            Dialog.Click += MenuPersonDialog_Click;
            Dialog.Tag = people;

            Button Check = new Button()
            {
                Content = "Обыскать",
                Opacity = 0.6,
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
            if (people.HealthInf() <= 0 || people.FractionInf() == NPSGroup.Box)
            {
                menu.Children.Add(Check);
            }
            else if (!player.FriendFranction.Contains(people.FractionInf()))
            {
                menu.Children.Add(Information);
            }
            else
            {
                menu.Children.Add(Information);
                menu.Children.Add(new Separator());
                menu.Children.Add(Dialog);
            }
            SystemObj.Add(menu);

        }
        private void MenuPersonCheck_Click(object sender, RoutedEventArgs e) 
        {
            Button button = (Button)sender;
            Skelet people = (Skelet)button.Tag;

            foreach (Item item in people.InventoryList.ReferenceItem.Keys)
            {
                for (int i = 0; i < people.InventoryList.ReferenceItem[item].Count; i++)
                {
                    AddItem(item);
                }
            }
            player.Money += people.Money;

            CurrentLocation.RemoveCell(people.picture, (int)people.Cord.X, (int)people.Cord.Y);
            CurrentLocation.Lives.Remove(people);

            TimerAnimation_Tick(null, null);
        }
        private void MenuPersonDialog_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Skelet people = (Skelet)button.Tag;

            //DialogWindow dialog = new DialogWindow(player, people, this);
            //dialog.ShowDialog();
        }
        private void MenuPersonInformation_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Skelet people = (Skelet)button.Tag;
        }
        private void ItemBut_Click(object sender, RoutedEventArgs e)
        {
            Button item = (Button)sender;
            if (item.Tag.ToString() == new KvestGun().SystemName ||
                item.Tag.ToString() == new MutantSkin().SystemName ||
                item.Tag.ToString() == new TailDog().SystemName || 
                item.Tag.ToString() == new ArtZabiiPuzir().SystemName)
            {
                return;
            }

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
            PDAWindow pda = new PDAWindow(player);
            pda.ShowDialog();
        }
        private void ArmorPlayer_Click(object sender, RoutedEventArgs e)
        {
            ToSeeEnemy = !ToSeeEnemy;
            ToSeePlayer = false;
        }
        private void GunPlayer_Click(object sender, RoutedEventArgs e)
        {
            ToSeePlayer = !ToSeePlayer;
            ToSeeEnemy = false;
        }
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menu = new MenuWindow();
            menu.Owner = this;
            menu.MenuInGame();
            menu.ShowDialog();
        }

    }
}
