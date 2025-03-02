using System.Collections.Generic;
using System.Windows;

namespace TwoD_Game_RP
{
    internal class MainScripts
    {
        static bool IsSpawnDrawwellDialog = false;
        static bool IsSpawnTransitEosha = false;
        static bool IsCreateManual = false;
        static List<string> ComplitedToEvent = new List<string>();
        public static void EventKillEnyone(Location location, AliveSkelet who, AliveSkelet whom)
        {
            if (location.SystemName == "Mine" && who is PlayerSkelet)
            {
                if (whom.Fraction == NPSGroup.Monster && !ComplitedToEvent.Contains("killOneScorpion"))
                {
                    ComplitedToEvent.Add("killOneScorpion");
                }
            }
            if (location.SystemName == "Eosha" && whom is Grandpa)
            {
                ComplitedToEvent.Add("kill1People");
            }
            if (location.SystemName == "Eosha" && whom is Girl)
            {
                ComplitedToEvent.Add("killAllPeople");
            }
        }
        public static void EventAddItemInBackpack(Item item)
        {
            if (item.Equals(new DatailDrawwell()) && !ComplitedToEvent.Contains("findDetailDrawwell"))
            {
                ComplitedToEvent.Add("findDetailDrawwell");
            }
            if (item.Equals(new SmallToz()) && !ComplitedToEvent.Contains("takeGunInQuestKillScorpion"))
            {
                ComplitedToEvent.Add("takeGunInQuestKillScorpion");
            }
        }
        public static void TimerAnalyze(MainWindow window)
        {
            foreach (var task in window.player.Tasks.GetUsingTask())
            {
                if (ComplitedToEvent.Contains(task.SystemName))
                {
                    window.player.Tasks.ComplitedTask(task.SystemName);
                    continue;
                }
                switch (task.SystemName)
                {
                    case "test": break;

                    //eventKill
                    case "killOneScorpion": break;
                    case "kill1People": break;
                    case "killAllPeople": break;
                    case "findDetailDrawwell": break;
                    case "takeGunInQuestKillScorpion":
                        {
                            if (!IsSpawnTransitEosha)
                            {
                                IsSpawnTransitEosha = true;
                                List<(string, GamePoint)> transit = new List<(string, GamePoint)>()
                                {
                                    ("Mine", new GamePoint(10,0)),
                                    ("Mine", new GamePoint(11,0)),
                                    ("Mine", new GamePoint(12,0)),
                                    ("Mine", new GamePoint(13,0)),
                                    ("Mine", new GamePoint(14,0)),
                                    ("Mine", new GamePoint(15,0)),
                                    ("UnderEosha", new GamePoint(3,13)),
                                };
                                window.Locations.Find(x => x.SystemName == "Eosha").AddTransitPoint(transit);
                            }
                            break;
                        }

                    //task
                    case "spawnPerecati":
                        {
                            if (window.player.GPoint.Equals((10, 23)))
                            {
                                var perecati = new Perecati(new GamePoint(12, 30), 0);
                                perecati.EnqueueDownGlobalAction(new ActionMove(new GamePoint(12, 3), true));
                                perecati.EnqueueDownGlobalAction(new ActionMove(new GamePoint(12, 30), true));
                                window.Locations.Find(x => x.SystemName == "Eosha").AddFirstLayerWithCell(perecati);

                                window.player.Tasks.ComplitedTask("spawnPerecati");
                            }
                            break;
                        }
                    case "go2Ep":
                        {
                            if (!IsCreateManual)
                            {
                                IsCreateManual = true;
                                string manual = "Обучение: \n" +
                                    "В игре есть 4 режима активностей: Движение, Взаимодействие, Атака, Изучение. \n" +
                                    "Их переключение происходит при помощи кнопок в нижнем левом углу. Их активация происходит при нажатии левой кнопки мыши. \n" +
                                    "Посмотреть все взаимодействия с объектом можно при помощи правой кнопки мыши. \n" +
                                    "Передвижение происходит при помощи левой кнопки мыши в режиме передвижение или с помощью WASD. \n";
                                MessageBox.Show(manual);
                            }
                            if (window.CurrentLocation.SystemName == "Eosha" && window.player.GPoint.Y == 31)
                            {
                                if (window.player.GPoint.X <= 13 && window.player.GPoint.X >= 10)
                                {
                                    window.player.Tasks.ComplitedTask("go2Ep");
                                }
                            }
                            break;
                        }
                    case "go2EpWithGun":
                        {
                            if (window.CurrentLocation.SystemName == "Eosha" && window.player.GPoint.Y == 31)
                            {
                                if (window.player.GPoint.X <= 13 && window.player.GPoint.X >= 10)
                                {
                                    window.player.Tasks.ComplitedTask("go2EpWithGun");
                                }
                            }
                            break;
                        }
                    case "go2Epkill1":
                        {
                            if (window.CurrentLocation.SystemName == "Eosha" && window.player.GPoint.Y == 31)
                            {
                                if (window.player.GPoint.X <= 13 && window.player.GPoint.X >= 10)
                                {
                                    window.player.Tasks.ComplitedTask("go2Epkill1");
                                }
                            }
                            break;
                        }
                    case "go2EpkillAll":
                        {
                            if (window.CurrentLocation.SystemName == "Eosha" && window.player.GPoint.Y == 31)
                            {
                                if (window.player.GPoint.X <= 13 && window.player.GPoint.X >= 10)
                                {
                                    window.player.Tasks.ComplitedTask("go2EpkillAll");
                                }
                            }
                            break;
                        }

                    //dialogs
                    case "fixDrawwell": break;
                    case "fixWithoutDetailDrawwell":
                        if (!IsSpawnDrawwellDialog)
                        {
                            window.Locations.Find(x => x.SystemName == "Eosha").AddFirstLayerWithCell(new Skelet("fixdrawwell", new GamePoint(9, 14), true, 0, 0, 1, NPSGroup.Box, null, "", ""));
                            IsSpawnDrawwellDialog = true;
                        }
                        break;
                    case "talkStartQuestDrawwell":
                        {
                            if (!IsSpawnDrawwellDialog)
                            {
                                window.Locations.Find(x => x.SystemName == "Eosha").AddFirstLayerWithCell(new Skelet("fixdrawwell", new GamePoint(9, 14), true, 0, 0, 1, NPSGroup.Box, null, "", ""));
                                IsSpawnDrawwellDialog = true;
                            }
                            break;
                        }
                    case "talkStartQuestKillScorpion": break;
                    case "talkEndQuestKillScorpion": break;

                    //trigger
                    case "checkActualMechanik": break;
                    case "checkActualSniper": break;
                    case "start1Ep": window.MenuPersonDialog_Click(window.player); break;
                    case "final0": Final(window, "Вы покинули родной Сан Пабло. Неизвестно вернётесь ли вы сюда."); break;
                    case "final1": Final(window, "Вы покинули родной Сан Пабло. Неизвестно вернётесь ли вы сюда. На вашей спине висит ружьё, которое даёт вам увереность."); break;
                    case "final2": Final(window, "Вы покинули родной Сан Пабло, добив её своеручно. Вы никогда больше сюда не вернётесь. Вероника это запомнила."); break;
                    case "final3": Final(window, "Вы покинули родной Сан Пабло. Что-то вам подсказывает, что вы больше не увидете его жителей."); break;

                    //triggerimp
                    case "talkEndQuestDrawwell": break;
                    case "triggerGo2EpWithGun": window.player.Tasks.ComplitedTask("triggerGo2EpWithGun"); break;
                    case "triggerkill1People": window.player.Tasks.ComplitedTask("triggerkill1People"); break;
                    case "triggerkillAllPeople": window.player.Tasks.ComplitedTask("triggerkillAllPeople"); break;

                    //default
                    default: throw new CustomException($"Task with SystemName {task.SystemName} not find");
                }
            }
        }
        private static void Final(MainWindow window, string message)
        {
            MessageBox.Show(message);
            MessageBox.Show("Спасибо за прохождение первой главы. В скором времени выйдет вторая. Если у вас есть мысли, как улучшить игру, или вы нашли ошибку - напишите нам (vk.com/evan_from_yav)");
            window.Close();
        }
    }
}
