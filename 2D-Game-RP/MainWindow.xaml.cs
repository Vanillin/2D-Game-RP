﻿using System;
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

namespace Game_STALKER_Exclusion_Zone
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rand = new Random(DateTime.Now.Millisecond);
        const int sizeX = 19;
        const int sizeY = 31;
        public double pixelSX;
        public double pixelSY;
        const int inventX = 4;
        const int inventY = 7;
        public double pixelIX;
        public double pixelIY;
        const double pixelIOtstX = 50;
        const double pixelIOtstY = 20;

        public List<Location> Locations = new List<Location>();
        public Player player;
        public string NameCurrentLocation = "";

        List<string> RandomNamePerson = new Information().NameRandom;
        List<string> RandomSecondNamePerson = new Information().NicknameRandom;

        Task[] TasksInGame = new Information().TasksInGame;

        public Style ButtonCheck = new Style();
        public Style ButtonNormal = new Style();
        bool ToSeePlayer = false;
        bool ToSeeEnemy = false;

        private static readonly int CountTimeAnimation = 200;
        private DispatcherTimer timerReloadAnimation = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(CountTimeAnimation)
        };

        public Task FindTask(string Systemname)
        {
            for (int i = 0; i < TasksInGame.Length; i++)
            {
                if (TasksInGame[i].SystemName == Systemname)
                {
                    return TasksInGame[i];
                }
            }
            throw new Exception("Не найдено задание по системному имени");
        }
        public MainWindow()
        {
            Time = 0;
            timerReloadAnimation.Tick += TimerAnimation_Tick;
            timerReloadAnimation.IsEnabled = true;

            ButtonNormal.Setters.Add(new Setter { Property = Button.OpacityProperty, Value = 0.01 });
            ButtonNormal.Setters.Add(new Setter { Property = Button.BackgroundProperty, Value = Brushes.White });
            //ButtonCheck.Setters.Add(new Setter { Property = Button.OpacityProperty, Value = 0.15 });
            //ButtonCheck.Setters.Add(new Setter { Property = Button.BackgroundProperty, Value = Brushes.Red });

            InitializeComponent();

            pixelSX = int.Parse(Map.Height.ToString()) / (sizeX + 3);
            pixelSY = int.Parse(Map.Width.ToString()) / (sizeY + 2);
            pixelIX = (int.Parse(InventoryPlayer.Height.ToString()) - pixelIOtstX) / (inventX);
            pixelIY = (int.Parse(InventoryPlayer.Width.ToString()) - pixelIOtstY) / (inventY);
            Icon = new BitmapImage(new Uri( Path.Combine(ConfigurationManager.AppSettings["Textures"], $"icon.png"), UriKind.Relative));
        }

        private int Time;
        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            NamePlayer.Content = $"{Time}";  
            Time++;  

            DoActionAll();
            CreateLocation(Locations.Find(x => x.SystemName == NameCurrentLocation));

            //MakeMovePersonIntellect();
            //if (selectLevel != null)
            //    selectLevel.TakeNextPictureLevel();

        }
        private void DoActionAll()
        {
            Location current = Locations.Find(X => X.SystemName == NameCurrentLocation);
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

        /*
         
        private static readonly int CountTimePlayer = 25;
        private static IPictureCell deadFloor = new StaticPicCell("Image/system/deadFloor.png", 0);
        private static IPictureCell step = new StaticPicCell("Image/system/step.png", 0);
        private static IPictureCell goal = new StaticPicCell("Image/system/goal.png", 0);
        private static IPictureCell final = new StaticPicCell("Image/system/final.png", 0);
                
        private GamePoint PointView;
        private CanvasToDisplay canvas;
        private string nameLastLevel;
        private GameLevel selectLevel;
        private Player Player = null;
        private int HeigthPole;
        private int WightPole;
        public MainWindow()
        {
            InitializeComponent();
            SizeCanvasChanged();
            selectLevel = null;
            canvas = new CanvasToDisplay(CanvasPole);

            timerReloadAnimation.Tick += TimerAnimation_Tick;
            timerReloadAnimation.IsEnabled = true;
        }
        
        private void DoActionPlayer()
        {
            if (selectLevel == null) return;
            selectLevel.DoActionSubject(Player);

            if (selectLevel.Transitions.ContainsKey(Player.PointOnGraf))
            {
                FinishLevel(selectLevel.Transitions[Player.PointOnGraf]);
            }
            canvas.ChangePointWatch((Player.Point.H, Player.Point.W));
        }        
         */
        public void Init(string PlayerName, PlayerGender Gender)
        {
            AddInformationPlayer(PlayerName, Gender, 7, 2, new Toz34(), new KurtkaStalker(), 1300, new List<Item>() { { new AidFirstKid() }, { new AidFirstKid() } });
            player.Tasks.Add(FindTask("ГлавныйКвест"));
            player.Tasks.Add(FindTask("СпроситьСталкеров"));

            ReadInformationLocation("hab");

            TimerAnimation_Tick(null, null);
            //CreateLocation(Locations.Find(x => x.SystemName == NameCurrentLocation));
            //ReloadPlayerInformation();
        }

        private void ReadInformationLocation(string name) 
        {
            //if (Locations.Exists(x => x.SystemName == NameCurrentLocation))
            //    Locations.Find(x => x.SystemName == NameCurrentLocation).Signs[(int)player.Cord.X, (int)player.Cord.Y] = null;
            //сброс персонажа при переходе

            string LastLocation = NameCurrentLocation;
            NameCurrentLocation = name;

            if (!Locations.Exists(x => x.SystemName == name))
            {
                if (name == "hab") //создание информации о локации
                {
                    Locations.Add(Information.GetLocationHub());
                }
                else if (name == "zavod")
                {
                    Locations.Add(Information.GetLocationZavod());
                }
                else if (name == "boloto")
                {
                    Locations.Add(Information.GetLocationBoloto());
                }
                else if (name == "electr")
                {
                    Locations.Add(Information.GetLocationBoloto());
                }

                //if (name == "zavod") //создание переходов и установка персонажа
                //{
                //    for (int i = 3; i < 6; i++)
                //    {
                //        signs[i, 0] = "trans-hab";
                //        air[i, 0] = "х";
                //    }
                //    player.Cord = new Point(4, 1);
                //}
                //if (name == "boloto")
                //{
                //    for (int i = 14; i < 17; i++)
                //    {
                //        signs[18, i] = "trans-hab";
                //        air[18, i] = "х";
                //    }
                //    player.Cord = new Point(17, 15);
                //}
                //if (name == "electr")
                //{
                //    for (int i = 6; i < 9; i++)
                //    {
                //        signs[0, i] = "trans-hab";
                //        air[0, i] = "х";
                //    }
                //    player.Cord = new Point(1, 7);
                //}
                
                AddPlayer((int)player.Cord.X, (int)player.Cord.Y);

                StalkerSmall stalker = new StalkerSmall(RandomNamePerson[0], RandomSecondNamePerson[0], new Ak74ukorot(), NPSIntellect.StandAgressive, new Point(0, 0), 0,
                    new List<Item>(), new List<NPSGroup>() { { NPSGroup.Stalker }, { NPSGroup.Box } });
                Locations.Find(X => X.SystemName == NameCurrentLocation).AddCell(stalker.picture, 1, 0, 0);
                Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Add(stalker);
                stalker.EnqueueDownGlobalAction(new ActionMove(new Point(0, 7), true));
                stalker.EnqueueDownGlobalAction(new ActionMove(new Point(0, 0), true));
                return;
            }

            // переходы на старые локации
            if (LastLocation == "boloto" && name == "hab")
                MakeMovePerson(player, new Point(1, 17));
            //player.Cord = new Point(1, 17);
            else if (LastLocation == "zavod" && name == "hab")
                MakeMovePerson(player, new Point(9, 29));
            //player.Cord = new Point(9, 29);
            else if (LastLocation == "electr" && name == "hab")
                MakeMovePerson(player, new Point(17, 10));
            //player.Cord = new Point(17, 10);
            else if (LastLocation == "hab" && name == "zavod")
                MakeMovePerson(player, new Point(4, 1));
            //player.Cord = new Point(4, 1);
            else if (LastLocation == "hab" && name == "boloto")
                MakeMovePerson(player, new Point(17, 15));
            //player.Cord = new Point(17, 15);
            else if (LastLocation == "hab" && name == "electr")
                MakeMovePerson(player, new Point(1, 7));
            //player.Cord = new Point(1, 7);

            //Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[(int)player.Cord.X, (int)player.Cord.Y] = "player";
        }
        //-------------------------------------------------------------------------------Add
        private void AddInformationPlayer(string PlayerName, PlayerGender gender, int x, int y, Gun gun, Cloth cloth, int money, List<Item> inventory)
        {
            player = new Player(PlayerName, gender, new Point(x, y), gun, cloth, money, inventory,
                new List<NPSGroup>() { { NPSGroup.Stalker }, { NPSGroup.Box } });
            PicturePlayer.Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{player.SystemName}.png"), UriKind.Relative));
        }
        private void AddPlayer(int x, int y)
        {
            Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Add(player);

            Locations.Find(X => X.SystemName == NameCurrentLocation).AddCell(player.picture, 1, x, y);
            //Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] = "player";

            player.OblSee = Locations.Find(X => X.SystemName == NameCurrentLocation).GrafLocToWatch.SearchSee(player.Cord, 9);
        }
        private void AddBox(string Name, string SystemName, int x, int y, int money, List<Item> listitems)
        {
            SkeletBox Box = new SkeletBox(Name, SystemName, new Point(x, y), money, listitems);
            //Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] =
            //    $"item{Name}{Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Count + 1}";
            Locations.Find(X => X.SystemName == NameCurrentLocation).AddCell(Box.picture, 1, x, y);
            Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Add(Box);
        }
        private void AddStalkerSmall(int x, int y, Gun gun, NPSIntellect intel, List<Item> inventory)
        {
            int numb1 = rand.Next(0, RandomNamePerson.Count - 1);
            int numb2 = rand.Next(0, RandomSecondNamePerson.Count - 1);
            StalkerSmall stalker = new StalkerSmall(RandomNamePerson[numb1], RandomSecondNamePerson[numb2], null, intel, new Point(x, y), 0,
                inventory, new List<NPSGroup>() { { NPSGroup.Stalker }, { NPSGroup.Box } });
            //Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] =
            //    $"stalker{Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Count + 1}";
            Locations.Find(X => X.SystemName == NameCurrentLocation).AddCell(stalker.picture, 1, x, y);
            Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Add(stalker);

            stalker.TakeGun(gun, Locations.Find(X => X.SystemName == NameCurrentLocation));
            stalker.OblSee = Locations.Find(X => X.SystemName == NameCurrentLocation).GrafLocToWatch.SearchSee(stalker.Cord, 9);
        }
        private void AddStalkerMedium(int x, int y, Gun gun, NPSIntellect intel, List<Item> inventory)
        {
            int numb1 = rand.Next(0, RandomNamePerson.Count - 1);
            int numb2 = rand.Next(0, RandomSecondNamePerson.Count - 1);
            StalkerMedium stalker = new StalkerMedium(RandomNamePerson[numb1], RandomSecondNamePerson[numb2], null, intel, new Point(x, y), 0,
                inventory, new List<NPSGroup>() { { NPSGroup.Stalker }, { NPSGroup.Box } });
            //Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] =
            //    $"stalker{Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Count + 1}";
            Locations.Find(X => X.SystemName == NameCurrentLocation).AddCell(stalker.picture, 1, x, y);
            Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Add(stalker);

            stalker.TakeGun(gun, Locations.Find(X => X.SystemName == NameCurrentLocation));
            stalker.OblSee = Locations.Find(X => X.SystemName == NameCurrentLocation).GrafLocToWatch.SearchSee(stalker.Cord, 9);
        }
        private void AddNaemnikMedium(int x, int y, Gun gun, NPSIntellect intel, List<Item> inventory)
        {
            int numb1 = rand.Next(0, RandomNamePerson.Count - 1);
            int numb2 = rand.Next(0, RandomSecondNamePerson.Count - 1);
            NaemnikMedium naemnik = new NaemnikMedium(RandomNamePerson[numb1], RandomSecondNamePerson[numb2], null, intel, new Point(x, y), 0,
                inventory, new List<NPSGroup>() { { NPSGroup.Naemnik }, { NPSGroup.Box } });
            //Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] =
            //    $"naemnik{Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Count + 1}";
            Locations.Find(X => X.SystemName == NameCurrentLocation).AddCell(naemnik.picture, 1, x, y);
            Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Add(naemnik);

            naemnik.TakeGun(gun, Locations.Find(X => X.SystemName == NameCurrentLocation));
            naemnik.OblSee = Locations.Find(X => X.SystemName == NameCurrentLocation).GrafLocToWatch.SearchSee(naemnik.Cord, 9);
        }
        private void AddNaemnikHard(int x, int y, Gun gun, NPSIntellect intel, List<Item> inventory)
        {
            int numb1 = rand.Next(0, RandomNamePerson.Count - 1);
            int numb2 = rand.Next(0, RandomSecondNamePerson.Count - 1);
            NaemnikHard naemnik = new NaemnikHard(RandomNamePerson[numb1], RandomSecondNamePerson[numb2], null, intel, new Point(x, y), 0,
                inventory, new List<NPSGroup>() { { NPSGroup.Naemnik }, { NPSGroup.Box } });
            //Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] =
            //    $"naemnik{Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Count + 1}";
            Locations.Find(X => X.SystemName == NameCurrentLocation).AddCell(naemnik.picture, 1, x, y);
            Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Add(naemnik);

            naemnik.TakeGun(gun, Locations.Find(X => X.SystemName == NameCurrentLocation));
            naemnik.OblSee = Locations.Find(X => X.SystemName == NameCurrentLocation).GrafLocToWatch.SearchSee(naemnik.Cord, 9);
        }
        private void AddMutantSobaka(int x, int y, List<Item> inventory)
        {
            MutantSobaka mutant = new MutantSobaka(new Point(x, y),
                inventory, new List<NPSGroup>() { { NPSGroup.Mutant }, { NPSGroup.Box } });
            //Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] =
            //    $"mutant{Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Count + 1}";
            Locations.Find(X => X.SystemName == NameCurrentLocation).AddCell(mutant.picture, 1, x, y);
            Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Add(mutant);

            mutant.OblSee = Locations.Find(X => X.SystemName == NameCurrentLocation).GrafLocToWatch.SearchSee(mutant.Cord, 11);
        }
        private void AddMutantCrovosos(int x, int y, List<Item> inventory)
        {
            MutantCrovosos mutant = new MutantCrovosos(new Point(x, y),
                inventory, new List<NPSGroup>() { { NPSGroup.Mutant }, { NPSGroup.Box } });
            //Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] =
            //    $"mutant{Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Count + 1}";
            Locations.Find(X => X.SystemName == NameCurrentLocation).AddCell(mutant.picture, 1, x, y);
            Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Add(mutant);

            mutant.OblSee = Locations.Find(X => X.SystemName == NameCurrentLocation).GrafLocToWatch.SearchSee(mutant.Cord, 11);
        }
        private void AddDealerBoris(int x, int y, Gun gun, NPSIntellect intel)
        {
            DealerBoris dealer = new DealerBoris(null, intel, new Point(x, y),
                new List<NPSGroup>() { { NPSGroup.Stalker }, { NPSGroup.Box } });
            //Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] =
            //    $"dealer{Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Count + 1}";
            Locations.Find(X => X.SystemName == NameCurrentLocation).AddCell(dealer.picture, 1, x, y);
            Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Add(dealer);

            dealer.TakeGun(gun, Locations.Find(X => X.SystemName == NameCurrentLocation));
            dealer.OblSee = Locations.Find(X => X.SystemName == NameCurrentLocation).GrafLocToWatch.SearchSee(dealer.Cord, 9);
        }
        private void AddStalkerZelen(int x, int y, Gun gun, NPSIntellect intel)
        {
            StalkerZelen zelen = new StalkerZelen(null, intel, new Point(x, y),
                new List<NPSGroup>() { { NPSGroup.Stalker }, { NPSGroup.Box } });
            //Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[x, y] =
            //    $"dealer{Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Count + 1}";
            Locations.Find(X => X.SystemName == NameCurrentLocation).AddCell(zelen.picture, 1, x, y);
            Locations.Find(X => X.SystemName == NameCurrentLocation).Lives.Add(zelen);

            zelen.TakeGun(gun, Locations.Find(X => X.SystemName == NameCurrentLocation));
            zelen.OblSee = Locations.Find(X => X.SystemName == NameCurrentLocation).GrafLocToWatch.SearchSee(zelen.Cord, 9);
        }
        private void AddItem(Item item)
        {
            player.InventoryList.Add(item);
        }

        //-------------------------------------------------------------------------------Reload Create
        
        private void CreateLocation(Location location)
        {
            Map.Children.Clear();
            for (int i = 0; i < location.Height; i++)
            {
                for (int j = 0; j < location.Width; j++)
                {
                    foreach (var v in location.GetListLocationCell(i, j))
                    {
                        Image image = new Image()
                        {
                            Source = new BitmapImage(new Uri(v.Picture(), UriKind.Relative)),
                            Width = pixelSY,
                            Height = pixelSX,
                        };
                        Canvas.SetLeft(image, pixelSY + pixelSY * j);
                        Canvas.SetTop(image, pixelSX + pixelSX * i);
                        Map.Children.Add(image);
                    }
                }
            }

            return;

            //if (ToSeePlayer)
            //{
            //    List<Point> around = Locations.Find(x => x.SystemName == NameCurrentLocation).Lives.Find(x => x.Cord == player.Cord).OblAttack;
            //    if (around.Count == 0)
            //    {
            //        NamePlayer.Content = $"Имя: {Locations.Find(x => x.SystemName == NameCurrentLocation).SignsLives[(int)player.Cord.X, (int)player.Cord.Y]}";
            //    }
            //    foreach (Point point in around)
            //    {
            //        LayerButtons[(int)point.X, (int)point.Y].Background = Brushes.Red;
            //        LayerButtons[(int)point.X, (int)point.Y].Opacity += 0.11;
            //    }
            //}
            //if (ToSeeEnemy)
            //{
            //    foreach (Skelet enemy in Locations.Find(x => x.SystemName == NameCurrentLocation).Lives)
            //    {
            //        if (!(enemy is Player) && enemy.IsAlive == true)
            //        {
            //            List<Point> around = Locations.Find(x => x.SystemName == NameCurrentLocation).Lives.Find(x => x.Cord == enemy.Cord).OblSee;
            //            foreach (Point point in around)
            //            {
            //                LayerButtons[(int)point.X, (int)point.Y].Background = Brushes.Red;
            //                LayerButtons[(int)point.X, (int)point.Y].Opacity += 0.10;
            //            }
            //        }
            //    }
            //}
        }        

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
            if (player.Tasks.Contains(FindTask("ГлавныйКвест")))
            {
                foreach (Item item in player.InventoryList)
                {
                    if (item is ArtZabiiPuzir)
                    {
                        player.Tasks.Remove(FindTask("ГлавныйКвест"));
                        player.CompliteTasks.Add(FindTask("ГлавныйКвест"));
                        player.Tasks.Remove(FindTask("НайтиАртефакт"));
                        player.CompliteTasks.Add(FindTask("НайтиАртефакт"));
                        player.Tasks.Add(FindTask("Финал"));

                        for (int i = 5; i < 8; i++)
                        {
                            //Locations.Find(x => x.SystemName == "hab").Signs[i, 0] = "trans-final";
                            //Locations.Find(x => x.SystemName == "hab").Air[i, 0] = "х";
                        }

                        AddMutantCrovosos(7, 21, new List<Item>() { new MutantSkin() });
                    }
                }
            }
            if (player.Tasks.Contains(FindTask("НайтиФамильноеРужьё")))
            {
                foreach (Item item in player.InventoryList)
                {
                    if (item is KvestGun)
                    {
                        player.Tasks.Remove(FindTask("НайтиФамильноеРужьё"));
                        player.CompliteTasks.Add(FindTask("НайтиФамильноеРужьё"));
                        player.Tasks.Add(FindTask("ВыполненКвестНайтиФамильноеРужьё"));
                    }
                }
            }

            int indi = 0; int indj = 0;
            InventoryPlayer.Children.Clear();

            foreach (Item item in player.InventoryList)
            {
                Button itemBut = new Button();
                itemBut.Tag = item.SystemName;
                itemBut.Click += ItemBut_Click;
                itemBut.Width = pixelIY;
                itemBut.Height = pixelIX;
                Canvas.SetLeft(itemBut, pixelIOtstY / 2 + pixelIY * indj);
                Canvas.SetTop(itemBut, pixelIOtstX / 2 + pixelIX * indi);

                Image image = new Image()
                {
                    Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"],
                    $"{item.SystemName}.png"), UriKind.Relative)),
                };
                itemBut.Content = image;

                InventoryPlayer.Children.Add(itemBut);

                indj++;
                if (indj >= inventY)
                {
                    indj = 0; indi++;
                }
            }

            while (indi < inventX)
            {
                Button itemEmp = new Button();
                itemEmp.Width = pixelIY;
                itemEmp.Height = pixelIX;
                Canvas.SetLeft(itemEmp, pixelIOtstY / 2 + pixelIY * indj);
                Canvas.SetTop(itemEmp, pixelIOtstX / 2 + pixelIX * indi);

                Image imageBut = new Image()
                {
                    Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"],
                    "emptyitem.png"), UriKind.Relative)),
                };
                itemEmp.Content = imageBut;

                InventoryPlayer.Children.Add(itemEmp);

                indj++;
                if (indj >= inventY)
                {
                    indj = 0; indi++;
                }
            }
        }

        //-------------------------------------------------------------------------------Move
        private void MakeMovePerson(Skelet Person, Point point)
        {
            Location current = Locations.Find(x => x.SystemName == NameCurrentLocation);
            current.GetListLocationCell((int)Person.Cord.X, (int)Person.Cord.Y).RemoveLocationCell(Person.picture);
            current.GetListLocationCell((int)point.X, (int)point.Y).AddLocationCell(Person.picture, 1);
            Person.Cord = new Point(point.X, point.Y);

            return;

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
            //    if (player.Tasks.Contains(FindTask("ПереходЗавод")) &&
            //        Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y].Split('-')[1] == "zavod")
            //    {
            //        player.Tasks.Remove(FindTask("ПереходЗавод"));
            //        player.CompliteTasks.Add(FindTask("ПереходЗавод"));
            //        player.Tasks.Add(FindTask("УбитьНаёмниковЗавод"));
            //    }
            //    else if (player.Tasks.Contains(FindTask("ПереходТопи")) &&
            //        Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y].Split('-')[1] == "boloto")
            //    {
            //        player.Tasks.Remove(FindTask("ПереходТопи"));
            //        player.CompliteTasks.Add(FindTask("ПереходТопи"));
            //        player.Tasks.Add(FindTask("НайтиАртефакт"));
            //    }
            //    else if (player.Tasks.Contains(FindTask("ПереходАномалии")) &&
            //        Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y].Split('-')[1] == "electr")
            //    {
            //        player.Tasks.Remove(FindTask("ПереходАномалии"));
            //        player.CompliteTasks.Add(FindTask("ПереходАномалии"));
            //        player.Tasks.Add(FindTask("НайтиФамильноеРужьё"));
            //    }

            //    Locations.Find(X => X.SystemName == NameCurrentLocation).SignsLives[(int)Person.Cord.X, (int)Person.Cord.Y] = null;

            //    if (Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y].Split('-')[1] == "final")
            //    {
            //        EndGame();
            //    }

            //    MoveToLocation(Person, Locations.Find(X => X.SystemName == NameCurrentLocation).Signs[x, y].Split('-')[1]);
            //}
        }
        private void MoveToLocation(Skelet Person, string nameLoc)
        {
            if (!(Person is Player))
            {
                return;
            }
            ReadInformationLocation(nameLoc);
            //CreateLayersLocation();
        }
        private void MakeMovePersonIntellect()
        {
            foreach (Skelet Person in Locations.Find(x => x.SystemName == NameCurrentLocation).Lives)
            {
                if (Person.Intellect == NPSIntellect.Non || Person is Player || !Person.IsAlive)
                {
                    continue;
                }

                Skelet Enemy = null;
                if (Person.LastSeeEnemy == null || Person.LastSeeEnemy.IsAlive == false )//|| 
                //    Locations.Find(x => x.SystemName == NameCurrentLocation).Signs[(int)Person.LastSeeEnemy.Cord.X, (int)Person.LastSeeEnemy.Cord.Y] == "shelter")
                {
                    Enemy = CheckPlayer(Person);
                    Person.LastSeeEnemy = Enemy;
                }
                else
                {
                    Enemy = Person.LastSeeEnemy;
                }

                if (Enemy is null)
                {
                    if (Person.Intellect == NPSIntellect.RandomPassive || Person.Intellect == NPSIntellect.RandomAgressive)
                        IntellectDoRandom(Person);
                    continue;
                }

                if (Person.Intellect == NPSIntellect.StandPassive || Person.Intellect == NPSIntellect.RandomPassive)
                {
                    IntellectDoPassive(Person, Enemy);
                }
                else if (Person.Intellect == NPSIntellect.StandAgressive || Person.Intellect == NPSIntellect.RandomAgressive)
                {
                    IntellectDoAgressive(Person, Enemy);
                }
            }
            ReloadPlayerInformation();
        }
        private Point? CreatePath(Skelet person, Point point)
        {
            int cordX = (int)point.X;
            int cordY = (int)point.Y;
            Location current = Locations.Find(x => x.SystemName == NameCurrentLocation);

            List<Vertex[]> deleted = new List<Vertex[]>(); //псевдоудаление из графа (перекидывание в хранилище)
            foreach (Skelet pers in Locations.Find(x => x.SystemName == NameCurrentLocation).Lives)
            {
                if (pers is SkeletBox || !pers.IsAlive || pers == person)
                {
                    continue;
                }
                foreach (Vertex v in current.GrafLocToMove.Vertexes.Find(X => X.Cord == pers.Cord).Near)
                {
                    deleted.Add(new Vertex[2] { v, current.GrafLocToMove.Vertexes.Find(X => X.Cord == pers.Cord) });
                    v.Near.Remove(current.GrafLocToMove.Vertexes.Find(X => X.Cord == pers.Cord));
                }
                deleted.Add(new Vertex[2] { null, current.GrafLocToMove.Vertexes.Find(X => X.Cord == pers.Cord) });
                current.GrafLocToMove.Vertexes.Remove(current.GrafLocToMove.Vertexes.Find(X => X.Cord == pers.Cord));
            }
            List<Point> points = null;

            if (current.GrafLocToMove.Vertexes.Find(X => X.Cord == point) != null)
            {
                points = current.GrafLocToMove.SearchWidth(player.Cord, point);
            }

            foreach (var del in deleted) //возвращение удаленного (позволяет сохранять ссылки)
            {
                if (del[0] == null)
                {
                    current.GrafLocToMove.Vertexes.Add(del[1]);
                }
            }
            foreach (var del in deleted)
            {
                if (del[0] != null)
                {
                    current.GrafLocToMove.Vertexes.Find(X => X.Cord == del[0].Cord).Near.Add(del[1]);
                }
            };

            if (points == null)
            {
                return null;
            }
            return points.First();
        }
        private Point CreatePath(Skelet person, Skelet enemy)
        {
            int cordX = (int)enemy.Cord.X;
            int cordY = (int)enemy.Cord.Y;
            Location current = Locations.Find(x => x.SystemName == NameCurrentLocation);
            Point retur = new Point();

            List<Vertex[]> deleted = new List<Vertex[]>(); //псевдоудаление из графа (перекидывание в хранилище)
            foreach (Skelet pers in Locations.Find(x => x.SystemName == NameCurrentLocation).Lives)
            {
                if (pers is SkeletBox || !pers.IsAlive || pers == person || pers == enemy)
                {
                    continue;
                }
                foreach (Vertex v in current.GrafLocToMove.Vertexes.Find(X => X.Cord == pers.Cord).Near)
                {
                    deleted.Add(new Vertex[2] { v, current.GrafLocToMove.Vertexes.Find(X => X.Cord == pers.Cord) });
                    v.Near.Remove(current.GrafLocToMove.Vertexes.Find(X => X.Cord == pers.Cord));
                }
                deleted.Add(new Vertex[2] { null, current.GrafLocToMove.Vertexes.Find(X => X.Cord == pers.Cord) });
                current.GrafLocToMove.Vertexes.Remove(current.GrafLocToMove.Vertexes.Find(X => X.Cord == pers.Cord));
            }

            List<Point> points = current.GrafLocToMove.SearchWidth(person.Cord, enemy.Cord);

            foreach (var del in deleted) //возвращение удаленного (позволяет сохранять ссылки)
            {
                if (del[0] == null)
                {
                    current.GrafLocToMove.Vertexes.Add(del[1]);
                }
            }
            foreach (var del in deleted)
            {
                if (del[0] != null)
                {
                    current.GrafLocToMove.Vertexes.Find(X => X.Cord == del[0].Cord).Near.Add(del[1]);
                }
            };

            if (points == null)
            {
                return retur;
            }
            return points.First();
        }
        //-------------------------------------------------------------------------------Intellect
        private Skelet CheckPlayer(Skelet Person)
        {
            int leng = int.MaxValue;
            Skelet personEnemy = null;
            Location current = Locations.Find(x => x.SystemName == NameCurrentLocation);
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

        private void IntellectDoRandom(Skelet Person)
        {
            Point Last = Person.LastGoingPoint;
            if (Last == new Point(-1,-1) || Last == Person.Cord) //||
               // Locations.Find(x => x.SystemName == NameCurrentLocation).SignsLives[(int)Last.X, (int)Last.Y] != null)
            {
                List<Point> RandomPoints = new List<Point>();
                {                    
                    for (int i = -3; i < 4; i++)
                    {
                        if (Person.Cord.X + i < 0 || Person.Cord.X + i >= sizeX)
                        {
                            continue;
                        }
                        for (int j = -3; j < 4; j++)
                        {
                            if (Person.Cord.Y + j < 0 || Person.Cord.Y + j >= sizeY)
                            {
                                continue;
                            }

                            int X = (int)Person.Cord.X + i;
                            int Y = (int)Person.Cord.Y + j;

                            //bool trans = (Locations.Find(x => x.SystemName == NameCurrentLocation).Signs[X, Y] != null
                            //    && Locations.Find(x => x.SystemName == NameCurrentLocation).Signs[X, Y].Split('-')[0] == "trans");

                            //if ((Locations.Find(x => x.SystemName == NameCurrentLocation).SignsLives[X, Y] == null) &&
                            //    (Locations.Find(x => x.SystemName == NameCurrentLocation).Signs[X, Y] != "block") &&
                            //    (Locations.Find(x => x.SystemName == NameCurrentLocation).Signs[X, Y] != "window") && !trans)
                            //{
                            //    RandomPoints.Add(new Point(X, Y));
                            //}
                        }
                    }
                }
                Point RandPoint = RandomPoints[rand.Next(0, RandomPoints.Count)];
                Person.LastGoingPoint = RandPoint;
            }

            if (Person.LastGoingPoint == null)
            {
                return;
            }

            MakeMovePerson(Person, Locations.Find(x => x.SystemName == NameCurrentLocation).GrafLocToMove.SearchWidth(Person.Cord, Person.LastGoingPoint).First());
        }
        private void IntellectDoPassive(Skelet Person, Skelet Enemy)
        {
            Location location = Locations.Find(x => x.SystemName == NameCurrentLocation);
            bool viewEnemy = Enemy.OblAttack.Contains(Person.Cord);
            bool viewPerson = Person.OblAttack.Contains(Enemy.Cord);
            if (viewPerson)
            {
                if (viewEnemy) //отходишь если рядом со слепой зоной
                {
                    foreach (Vertex vertex in location.GrafLocToMove)
                    {
                        if (vertex.Cord == Person.Cord)
                        {
                            foreach (Vertex vert in vertex.Near)
                            {
                                if (!Enemy.OblAttack.Contains(vert.Cord))
                                {
                                    MakeMovePerson(Person, vert.Cord);
                                    return;
                                }
                            }
                            break;
                        }
                    }
                }
                Enemy.Damaging(Person.GunInf().Damage); //иначе атакуешь
            }
            else
            {
                if (viewEnemy) //отходишь от него подальше
                {
                    foreach (Vertex vertex in location.GrafLocToMove)
                    {
                        if (vertex.Cord == Person.Cord)
                        {
                            int leng = 0;
                            Point point = new Point();
                            foreach (Vertex vert in vertex.Near)
                            {
                                int LenEnemy = location.GrafLocToMove.SearchWidth(Enemy.Cord, vert.Cord).Count;
                                if (LenEnemy > leng)
                                {
                                    point = vert.Cord;
                                    leng = LenEnemy;
                                }
                            }

                            MakeMovePerson(Person, point);
                            return;
                        }
                    }
                }

                Point going = CreatePath(Person, Enemy); //подходишь
                MakeMovePerson(Person, going);
            }
        }
        private void IntellectDoAgressive(Skelet Person, Skelet Enemy)
        {
            Location location = Locations.Find(x => x.SystemName == NameCurrentLocation);
            bool viewPerson = Person.OblAttack.Contains(Enemy.Cord);
            if (viewPerson)
            {
                Enemy.Damaging(Person.GunInf().Damage); //атакуешь
            }
            else
            {
                Point going = CreatePath(Person, Enemy); //подходишь
                MakeMovePerson(Person, going);
            }
        }

        //-------------------------------------------------------------------------------Click

        private void Map_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            /////////////////=====================Событие клика левой кнопки, ловим корды и обрабатываем индексы и вызываем ClickRightButtonMap
        }
        private void ClickRightButtonMap(object sender, RoutedEventArgs e) 
        {
            //Menu save = null;
            //foreach (var v in Map.Children)
            //{
            //    if (v is Menu)
            //    {
            //        save = (Menu)v;
            //        break;
            //    }
            //}
            //if (save != null)
            //{
            //    Map.Children.Remove(save);
            //}

            Button button = (Button)sender;
            string[] cord = button.Tag.ToString().Split();
            int cordX = int.Parse(cord[0]);
            int cordY = int.Parse(cord[1]);
            Skelet people = Locations.Find(x => x.SystemName == NameCurrentLocation).Lives.Find(x => x.Cord == new Point(cordX, cordY));

            if (people is Player || people == null)
            {
                return;
            }
            bool ShelterKey = (Locations.Find(X => X.SystemName == NameCurrentLocation).GrafLocToWatch.Vertexes.Find(X => X.Cord == people.Cord) == null ||
                Locations.Find(X => X.SystemName == NameCurrentLocation).GrafLocToWatch.Vertexes.Find(X => X.Cord == player.Cord) == null);

            StackPanel menu = new StackPanel();
            Canvas.SetLeft(menu, pixelSY + pixelSY * (cordY + 1));
            Canvas.SetTop(menu, pixelSX + pixelSX * cordX);

            if (people != null)
            {
                Button Information = new Button()
                {
                    Content = "Информация",
                    Opacity = 0.6,
                };
                Information.Click += MenuPersonInformation_Click;
                Information.Tag = button.Tag;

                Button Attack = new Button()
                {
                    Content = "Атаковать",
                    Opacity = 0.6,
                };
                Attack.Click += MenuPersonAttack_Click;
                Attack.Tag = button.Tag;

                Button Dialog = new Button()
                {
                    Content = "Поговорить",
                    Opacity = 0.6,
                };
                Dialog.Click += MenuPersonDialog_Click;
                Dialog.Tag = button.Tag;

                Button Check = new Button()
                {
                    Content = "Обыскать",
                    Opacity = 0.6,
                };
                Check.Click += MenuPersonCheck_Click;
                Check.Tag = button.Tag;

                if (ShelterKey || Locations.Find(x => x.SystemName == NameCurrentLocation).GrafLocToMove.SearchWidth(player.Cord, people.Cord).Count > 2)
                {
                    Check.IsEnabled = false;
                    Dialog.IsEnabled = false;
                }
                if (ShelterKey || Locations.Find(x => x.SystemName == NameCurrentLocation).GrafLocToWatch.SearchWidth(player.Cord, people.Cord).Count > player.GunInf().Radius)
                {
                    Attack.IsEnabled = false;
                }

                if (people.HealthInf() <= 0 || people.FractionInf() == NPSGroup.Box)
                {
                    menu.Children.Add(Check);
                }
                else if (!player.FriendFranction.Contains(people.FractionInf()))
                {
                    menu.Children.Add(Information);
                    menu.Children.Add(Attack);
                }
                else
                {
                    menu.Children.Add(Information);
                    menu.Children.Add(new Separator());
                    menu.Children.Add(Attack);
                    menu.Children.Add(new Separator());
                    menu.Children.Add(Dialog);
                }
            }
            Map.Children.Add(menu);

        }

        private void MenuPersonCheck_Click(object sender, RoutedEventArgs e) 
        {
            Button button = (Button)sender;
            string[] cord = button.Tag.ToString().Split();
            int cordX = int.Parse(cord[0]);
            int cordY = int.Parse(cord[1]);
            Location current = Locations.Find(x => x.SystemName == NameCurrentLocation);

            Skelet people = current.Lives.FindAll(x => x.Cord == new Point(cordX, cordY))[0];
            foreach (Item item in people.InventoryList)
            {
                AddItem(item);
            }
            player.Money += people.Money;

            //current.SignsLives[cordX, cordY] = null;
            current.Lives.Remove(people);

            MakeMovePersonIntellect();
            //CreateLayerPerson(current);
            //DisplayLocationOnMap();
            TimerAnimation_Tick(null, null);
            //CreateLocation(current);
            //ReloadPlayerInformation();
        }

        private void MenuPersonDialog_Click(object sender, RoutedEventArgs e) 
        {
            Button button = (Button)sender;
            string[] cord = button.Tag.ToString().Split();
            int cordX = int.Parse(cord[0]);
            int cordY = int.Parse(cord[1]);
            Skelet people = Locations.Find(x => x.SystemName == NameCurrentLocation).Lives.Find(x => x.Cord == new Point(cordX, cordY));

            DialogWindow dialog = new DialogWindow(player, people, this);
            //dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void MenuPersonAttack_Click(object sender, RoutedEventArgs e) 
        {
            Button button = (Button)sender;
            string[] cord = button.Tag.ToString().Split();
            int cordX = int.Parse(cord[0]);
            int cordY = int.Parse(cord[1]);
            Skelet people = Locations.Find(x => x.SystemName == NameCurrentLocation).Lives.Find(x => x.Cord == new Point(cordX, cordY));

            people.Damaging(player.GunInf().Damage);

            if (people.HealthInf() <= 0)
            {
                player.Kills++;

                if (player.Tasks.Contains(FindTask("УбитьНаёмниковЗавод")) && !Locations.Find(x => x.SystemName == "zavod").Lives.Exists(x => x.FractionInf() == NPSGroup.Naemnik && x.IsAlive == true))
                {
                    player.Tasks.Remove(FindTask("УбитьНаёмниковЗавод"));
                    player.CompliteTasks.Add(FindTask("УбитьНаёмниковЗавод"));
                    player.Tasks.Add(FindTask("ВыполненКвестУбитьНаёмниковЗавод"));
                }
            }

            MakeMovePersonIntellect();
            //CreateLayerPerson(Locations.Find(x => x.SystemName == NameCurrentLocation));
            //DisplayLocationOnMap();
            TimerAnimation_Tick(null, null);
            //CreateLocation(Locations.Find(x => x.SystemName == NameCurrentLocation));
        }
        private void MenuPersonInformation_Click(object sender, RoutedEventArgs e) 
        {
            Button button = (Button)sender;
            string[] cord = button.Tag.ToString().Split();
            int cordX = int.Parse(cord[0]);
            int cordY = int.Parse(cord[1]);
            Skelet people = Locations.Find(x => x.SystemName == NameCurrentLocation).Lives.Find(x => x.Cord == new Point(cordX, cordY));
        }
        private void Map_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point pointMouse = e.GetPosition(Map);
            (int W, int H) = ((int)Math.Truncate(pointMouse.X / pixelSX)-1, (int)Math.Truncate(pointMouse.Y / pixelSY)-1);
            Location current = Locations.Find(x => x.SystemName == NameCurrentLocation);
            if (!current.GetListLocationCell(H, W).IsBlock)
            {
                player.EnqueueUpGlobalAction(new ActionMove(new Point(H, W), false));

                //случай перехода на новую локацию или застревания где-либо
            }
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

            foreach(Item item1 in player.InventoryList)
            {
                if (item.Tag.ToString() == item1.SystemName)
                {
                    item1.Using(player);
                    if (item1 is Gun)
                    {
                        player.TakeGun((Gun)item1, Locations.Find(x => x.SystemName == NameCurrentLocation));
                    }
                    player.InventoryList.Remove(player.InventoryList.Find(x => x.SystemName == item.Tag.ToString()));
                    break;
                }
            }            

            ReloadPlayerInformation();
        }

        //-------------------------------------------------------------------------------InOtherWindow

        public void TakeRewardTaskKillNaemnik()
        {
            player.Tasks.Remove(FindTask("ВыполненКвестУбитьНаёмниковЗавод"));
            player.CompliteTasks.Add(FindTask("ВыполненКвестУбитьНаёмниковЗавод"));
            player.Tasks.Add(FindTask("ПереходТопи"));
            for (int i = 16; i < 19; i++)
            {
                //Locations.Find(x => x.SystemName == "hab").Signs[0, i] = "trans-boloto";
                //Locations.Find(x => x.SystemName == "hab").Air[0, i] = "х";
            }
            //CreateLayerEarth(Locations.Find(x => x.SystemName == "hab"));
            //CreateLayerAir(Locations.Find(x => x.SystemName == "hab"));
        }
        public void TakeTaskKillNaemnik()
        {
            player.Tasks.Remove(FindTask("СпроситьСталкеров"));
            player.CompliteTasks.Add(FindTask("СпроситьСталкеров"));
            player.Tasks.Add(FindTask("ПереходЗавод"));
            for (int i = 8; i < 11; i++)
            {
                //Locations.Find(x => x.SystemName == "hab").Signs[i, 30] = "trans-zavod";
                //Locations.Find(x => x.SystemName == "hab").Air[i, 30] = "х";
            }
            //CreateLayerEarth(Locations.Find(x => x.SystemName == "hab"));
            //CreateLayerAir(Locations.Find(x => x.SystemName == "hab"));
        }

        public void TakeTaskSearchGun()
        {
            player.Tasks.Add(FindTask("ПереходАномалии"));
            for (int i = 9; i < 12; i++)
            {
                //Locations.Find(x => x.SystemName == "hab").Signs[18, i] = "trans-electr";
                //Locations.Find(x => x.SystemName == "hab").Air[18, i] = "х";
            }
            //CreateLayerEarth(Locations.Find(x => x.SystemName == "hab"));
            //CreateLayerAir(Locations.Find(x => x.SystemName == "hab"));
        }
        public void TakeRewardTaskSearchGun()
        {
            player.Tasks.Remove(FindTask("ВыполненКвестНайтиФамильноеРужьё"));
            player.CompliteTasks.Add(FindTask("ВыполненКвестНайтиФамильноеРужьё"));
            player.InventoryList.Remove(player.InventoryList.Find(x => x.SystemName == "KvestGun"));

            player.Money += 450;
            AddItem(new Bread());
            AddItem(new Stew());
            AddItem(new AidFirstKid());
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
            Item it = player.InventoryList.Find(X => X.SystemName == item.SystemName);
            if (it == null) return false;
            else
            {
                player.Money += it.Cost;
                player.InventoryList.Remove(it);
                return true;
            }            
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

            //CreateLocation(Locations.Find(x => x.SystemName == NameCurrentLocation));
        }
        private void GunPlayer_Click(object sender, RoutedEventArgs e)
        {
            ToSeePlayer = !ToSeePlayer;
            ToSeeEnemy = false;

            //CreateLocation(Locations.Find(x => x.SystemName == NameCurrentLocation));
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