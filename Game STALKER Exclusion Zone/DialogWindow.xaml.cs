using InputLibraryForStalkerEZ;
using LibraryForStalkerEZ;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Game_STALKER_Exclusion_Zone
{
    /// <summary>
    /// Логика взаимодействия для DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        //public Dictionary<string, string> DialogQuestion = new Dictionary<string, string>
        //{
        //    {"ВопросОРаботе", "Работа есть?"},
        //    {"ЧтоНового", "Что в мире нового?"},
        //    {"Торговля", "Могу посмотреть товары?"},
        //    {"ВопросОРаботеДа", "Да, берусь."},
        //    {"ВопросОРаботеНет", "Не, это не для меня."},
        //    {"Аптечка", "Аптечку (350р)"},
        //    {"АК", "Калаш (2300р)"},
        //    {"Обрез", "Обрез (1500р)"},
        //    {"НичегоТорговля", "Ничего не надо."},
        //    {"ЗаданиеВыполнено", "Завод теперь чист. Наёмники тебя больше не потревожат."},
        //    {"ВопросПодработкаПоиск", "Тебе нужна какая-то другая помощь?"},
        //    {"ВопросПодработкаДа", "Я согласен."},
        //    {"ВопросПодработкаНет", "В другой раз."},
        //    {"ЗаданиеПоискВыполнено", "Нашёл твоё ружьё."}
        //};
        
        public Phrase[] PhraseVersion = new Information().PhraseInGame;
        public Dictionary<string, string> Phrase = new Dictionary<string, string>();

        Random rand = new Random(DateTime.Now.Millisecond);
        Player Player;
        Skelet Person;

        public DialogWindow(Player Player, Skelet Person, MainWindow mainWindow)
        {
            foreach (Phrase phrase in PhraseVersion)
            {
                Phrase.Add(phrase.Index, phrase.Dialog);
            }

            this.Player = Player;
            this.Person = Person;
            this.Owner = mainWindow;
            InitializeComponent();
            Icon = new BitmapImage(new Uri("gamedata/textures/icon.png", UriKind.Relative));
            CreatePersones();
            StartDialog();
        }
        private void StartDialog()
        {
            int rn = rand.Next(0, 4);
            //AddDialog("...", "l");
            switch (rn)
            {
                case 0: AddDialog(Phrase["Привет11"], "r"); AddDialog(Phrase["Привет12"], "l"); break;
                case 1: AddDialog(Phrase["Привет21"], "r"); AddDialog(Phrase["Привет22"], "l"); break;
                case 2: AddDialog(Phrase["Привет31"], "r"); AddDialog(Phrase["Привет32"], "l"); break;
                case 3: AddDialog(Phrase["Привет41"], "r"); AddDialog(Phrase["Привет42"], "l"); break;
            }
            ReturnDialog();
        }
        private void ReturnDialog()
        {
            if (Person is DealerBoris)
            {
                AddButton("Торговля1");
                AddButton("Продажа1");

                if (Player.Tasks.Contains(((MainWindow)this.Owner).FindTask("СпроситьСталкеров")))
                    AddButton("ГлавныйКвест1");
                if (Player.Tasks.Contains(((MainWindow)this.Owner).FindTask("ВыполненКвестУбитьНаёмниковЗавод"))) //------------ВыполненКвестУбитьНаёмниковЗавод
                    AddButton("ГлавныйКвест7");
            }
            if (Person is StalkerZelen)
            {
                AddButton("ГлавныйКвестСталкер1");
                AddButton("ЧтоНового1");

                if (!Player.Tasks.Contains(((MainWindow)this.Owner).FindTask("ПереходАномалии")) &&
                    !Player.CompliteTasks.Contains(((MainWindow)this.Owner).FindTask("ПереходАномалии")))
                    AddButton("КвестПоискРужья1");
                if (Player.Tasks.Contains(((MainWindow)this.Owner).FindTask("ВыполненКвестНайтиФамильноеРужьё")))
                    AddButton("КвестПоискРужья5");

            }
            if (Person is StalkerSmall || Person is StalkerMedium)
            {
                AddButton("ГлавныйКвестСталкер1");
                AddButton("ЧтоНового1");
                AddButton("ТорговляСталкер1");
            }
        }
        private void CreatePersones()
        {
            FractionPerson.Content = Person.FractionString();
            NamePerson.Content = Person.Name;
            PicturePerson.Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{Person.SystemName}.png"), UriKind.Relative));
            FractionPlayer.Content = Player.FractionString();
            NamePlayer.Content = Player.Name;
            PicturePlayer.Source = new BitmapImage(new Uri(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{Player.SystemName}.png"), UriKind.Relative));
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
            Button b = (Button)sender;
            AddDialog(b.Content.ToString(), "r");

            switch (b.Tag)
            {
                case "ЧтоНового1":
                    {
                        int rn = rand.Next(0, 4);
                        //AddDialog("...", "l");
                        switch (rn)
                        {
                            case 0: AddDialog(Phrase["ЧтоНового21"], "l"); break;
                            case 1: AddDialog(Phrase["ЧтоНового22"], "l"); break;
                            case 2: AddDialog(Phrase["ЧтоНового23"], "l"); break;
                            case 3: AddDialog(Phrase["ЧтоНового24"], "l"); break;
                        }
                        ReturnDialog();
                        break;
                    }
                case "ГлавныйКвестСталкер1":
                    {
                        int rn = rand.Next(0, 3);
                        //AddDialog("...", "l");
                        switch (rn)
                        {
                            case 0: AddDialog(Phrase["ГлавныйКвестСталкер21"], "l"); break;
                            case 1: AddDialog(Phrase["ГлавныйКвестСталкер22"], "l"); break;
                            case 2: AddDialog(Phrase["ГлавныйКвестСталкер23"], "l"); break;
                        }
                        ReturnDialog();
                        break;
                    }
                #region ГЛАВНЫЙ КВЕСТ
                case "ГлавныйКвест1":
                    {
                        AddDialog(Phrase["ГлавныйКвест2"], "l");
                        AddButton("ГлавныйКвест3");
                        break;
                    }
                case "ГлавныйКвест3":
                    {
                        AddDialog(Phrase["ГлавныйКвест4"], "l");
                        AddButton("ГлавныйКвест51");
                        AddButton("ГлавныйКвест52");
                        break;
                    }
                case "ГлавныйКвест52":
                    {
                        ReturnDialog();
                        break;
                    }
                case "ГлавныйКвест51":
                    {
                        AddDialog(Phrase["ГлавныйКвест6"], "l"); 
                        ((MainWindow)this.Owner).TakeTaskKillNaemnik();
                        ReturnDialog();
                        break;
                    }
                case "ГлавныйКвест7":
                    {
                        AddDialog(Phrase["ГлавныйКвест8"], "l");
                        ((MainWindow)this.Owner).TakeRewardTaskKillNaemnik();
                        ReturnDialog();
                        break;
                    }
                #endregion

                #region КВЕСТ ПОИСК РУЖЬЯ
                case "КвестПоискРужья1":
                    {
                        AddDialog(Phrase["КвестПоискРужья2"], "l");
                        AddButton("КвестПоискРужья31");
                        AddButton("КвестПоискРужья32");
                        break;
                    }
                case "КвестПоискРужья32":
                    {
                        ReturnDialog();
                        break;
                    }
                case "КвестПоискРужья31":
                    {
                        AddDialog(Phrase["КвестПоискРужья4"], "l");
                        ((MainWindow)this.Owner).TakeTaskSearchGun();
                        ReturnDialog();
                        break;
                    }
                case "КвестПоискРужья5":
                    {
                        AddDialog(Phrase["КвестПоискРужья6"], "l");
                        ((MainWindow)this.Owner).TakeRewardTaskSearchGun();
                        ReturnDialog();
                        break;
                    }
                #endregion

                case "ТорговляСталкер1":
                    {
                        int rn = rand.Next(0, 2);
                        //AddDialog("...", "l");
                        switch (rn)
                        {
                            case 0: AddDialog(Phrase["ТорговляСталкер21"], "l"); break;
                            case 1: AddDialog(Phrase["ТорговляСталкер22"], "l"); break;
                        }
                        ReturnDialog();
                        break;
                    }

                #region ТОРГОВЛЯ ПРОДАЖА
                case "Торговля1":
                    {
                        AddDialog(Phrase["Торговля2"], "l");
                        AddButton("ТорговляАптечка");
                        AddButton("ТорговляАрмейскаяАптечка");
                        AddButton("ТорговляАК74");
                        AddButton("ТорговляХлеб");
                        AddButton("ТорговляМП5");
                        AddButton("ТорговляТоз34");
                        AddButton("ТорговляКонсерва");

                        AddButton("Торговля3");
                        break;
                    }
                case "Торговля3":
                    {
                        ReturnDialog();
                        break;
                    }
                case "Продажа1":
                    {
                        AddDialog(Phrase["Продажа2"], "l");
                        AddButton("ПродажаХвост");
                        AddButton("ПродажаШкура");

                        AddButton("Продажа3");
                        break;
                    }
                case "Продажа3":
                    {
                        ReturnDialog();
                        break;
                    }
                case "ТорговляАптечка":
                    {
                        if (!((MainWindow)this.Owner).BuyThing(new AidFirstKid())) AddDialog(Phrase["Торговля4"], "l");
                        else AddDialog(Phrase["Торговля5"], "l");
                        ReturnDialog(); break;
                    }
                case "ТорговляАрмейскаяАптечка":
                    {
                        if (!((MainWindow)this.Owner).BuyThing(new ArmyAidFirstKid())) AddDialog(Phrase["Торговля4"], "l");
                        else AddDialog(Phrase["Торговля5"], "l");
                        ReturnDialog(); break;
                    }
                case "ТорговляАК74":
                    {
                        if (!((MainWindow)this.Owner).BuyThing(new Ak74ukorot())) AddDialog(Phrase["Торговля4"], "l");
                        else AddDialog(Phrase["Торговля5"], "l");
                        ReturnDialog(); break;
                    }
                case "ТорговляХлеб":
                    {
                        if (!((MainWindow)this.Owner).BuyThing(new Bread())) AddDialog(Phrase["Торговля4"], "l");
                        else AddDialog(Phrase["Торговля5"], "l");
                        ReturnDialog(); break;
                    }
                case "ТорговляМП5":
                    {
                        if (!((MainWindow)this.Owner).BuyThing(new MP5())) AddDialog(Phrase["Торговля4"], "l");
                        else AddDialog(Phrase["Торговля5"], "l");
                        ReturnDialog(); break;
                    }
                case "ТорговляТоз34":
                    {
                        if (!((MainWindow)this.Owner).BuyThing(new Toz34())) AddDialog(Phrase["Торговля4"], "l");
                        else AddDialog(Phrase["Торговля5"], "l");
                        ReturnDialog(); break;
                    }
                case "ТорговляКонсерва":
                    {
                        if (!((MainWindow)this.Owner).BuyThing(new Stew())) AddDialog(Phrase["Торговля4"], "l");
                        else AddDialog(Phrase["Торговля5"], "l");
                        ReturnDialog(); break;
                    }
                case "ПродажаХвост":
                    {
                        if (!((MainWindow)this.Owner).SellThing(new TailDog())) AddDialog(Phrase["Продажа4"], "l");
                        else AddDialog(Phrase["Продажа5"], "l");
                        ReturnDialog(); break;
                    }
                case "ПродажаШкура":
                    {
                        if (!((MainWindow)this.Owner).SellThing(new MutantSkin())) AddDialog(Phrase["Продажа4"], "l");
                        else AddDialog(Phrase["Продажа5"], "l");
                        ReturnDialog(); break;
                    }
                    #endregion
            }
        }
        private void AddButton(string tag)
        {
            Button b = new Button()
            {
                Tag = tag,
                Content = Phrase[tag],
                FontSize = 20,
                Margin = new Thickness(0, 2, 0, 2),
            };
            b.Click += Question_Click;
            DialogWin.Children.Add(b);
        }
        private void AddDialog(string dialog, string side)
        {
            if (side == "r")
                DialogWin.Children.Add(new Label()
                {
                    Content = dialog,
                    FontSize = 20,
                    Background = new SolidColorBrush(Colors.DarkGray),
                    HorizontalAlignment = HorizontalAlignment.Right,
                });
            else
                DialogWin.Children.Add(new Label()
                {
                    Content = dialog,
                    FontSize = 20,
                    Background = new SolidColorBrush(Colors.LightGray),
                    HorizontalAlignment = HorizontalAlignment.Left,
                });
        }
    }
}
