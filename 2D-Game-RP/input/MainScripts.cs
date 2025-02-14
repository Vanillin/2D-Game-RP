namespace TwoD_Game_RP
{
    internal class MainScripts
    {
        public static void TimerAnalyze(MainWindow window)
        {
            foreach (var task in window.player.Tasks.GetUsingTask())
            {
                switch (task.SystemName)
                {
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
                    case "findDetailDrawwell": break;
                    case "fixDrawwell": break;
                    case "fixWithoutDetailDrawwell": break;
                    case "takeGunInQuestKillScorpion": break;
                    case "killOneScorpion": break;
                    case "kill1People": break;
                    case "killAllPeople": break;
                    case "go2Ep": break;
                    case "go2EpWithGun": break;
                    case "go2Epkill1": break;
                    case "go2EpkillAll": break;

                    case "test": break;
                    case "talkStartQuestDrawwell": break;
                    case "talkStartQuestKillScorpion": break;
                    case "talkEndQuestKillScorpion": break;

                    //trigger
                    case "start1Ep": window.player.Tasks.ComplitedTask("start1Ep"); break;
                    case "final0": Final(window); break;
                    case "final1": Final(window); break;
                    case "final2": Final(window); break;
                    case "final3": Final(window); break;

                    //triggerimp
                    case "talkEndQuestDrawwell": break;
                    case "triggerGo2EpWithGun": break;
                    case "triggerkill1People": break;
                    case "triggerkillAllPeople": break;

                    //default
                    default: throw new CustomException($"Task with SystemName {task.SystemName} not find");
                }
            }
        }
        private static void Final(MainWindow window)
        {
            //финал игры. Написать что продолжение следует, спасибо за прохождение первой главы. Написать что свои идеи и наработки писать туда сюда
        }
    }
}
