using System.Collections.Generic;
using System.Windows;

namespace TwoD_Game_RP
{
    internal class MainScripts
    {
        static bool IsSpawnDrawwellDialog = false;
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
                    case "takeGunInQuestKillScorpion": break;

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
                    case "talkEndQuestDrawwell": break;

                    //trigger
                    case "checkActualMechanik": break;
                    case "checkActualSniper": break;
                    case "start1Ep": window.MenuPersonDialog_Click(window.player); break;
                    case "final0": Final(window, "1"); break;
                    case "final1": Final(window, "2"); break;
                    case "final2": Final(window, "3"); break;
                    case "final3": Final(window, "4"); break;

                    //triggerimp
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
            window.Close();
        }
    }
}
