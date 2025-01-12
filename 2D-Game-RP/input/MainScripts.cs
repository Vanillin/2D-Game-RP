using System;
using System.Windows;

namespace TwoD_Game_RP
{
    internal class MainScripts
    {
        private static void ScriptTime(MainWindow window)
        {
            MessageBox.Show("Вы подождали до вечера. С работы пришли жильцы.");

            //var maksim = new Maksim(new GamePoint(17, 19), '0');
            //maksim.EnqueueDownGlobalAction(new ActionWait(5, true));
            //maksim.EnqueueDownGlobalAction(new ActionMove(new GamePoint(16, 23), true));
            //maksim.EnqueueDownGlobalAction(new ActionWait(1, true));
            //maksim.EnqueueDownGlobalAction(new ActionMove(new GamePoint(17, 19), true));
            //window.CurrentLocation.AddFirstLayerWithCell(maksim);

            foreach (var skelet in window.CurrentLocation.GetLives())
            {
                if (skelet is Door && skelet.Cord.CompareTo((14, 15)) == 0)
                {
                    ((Door)skelet).IsLock = false;
                    break;
                }
            }

            window.player.Tasks.ComplitedTask("scriptTime");
        }
        public static void TimerAnalyze(MainWindow window)
        {
            foreach (var task in window.player.Tasks.GetUsingTask())
            {
                if (task.SystemName == "trainingButton")
                {
                    MessageBox.Show("Управление: \nКнопками: WASD для ходьбы \nМышкой: ЛКМ для ходьбы, ПКМ для взаимодействия с чем-либо");
                    window.player.Tasks.ComplitedTask("trainingButton");
                }

                if (task.SystemName == "scriptstart")
                {
                    window.player.Tasks.ComplitedTask("scriptstart");
                    //window.MenuPersonDialog_Click(new Agency(new GamePoint(0, 0), '0'));
                }

                if (task.SystemName == "findKey" && window.player.ContainsInBackpack(new Key()))
                {
                    window.player.Tasks.ComplitedTask("findKey");
                }

                if (task.SystemName == "findBloodPaper" && window.player.ContainsInBackpack(new BloodPaper()))
                {
                    window.player.Tasks.ComplitedTask("findBloodPaper");
                }

                if (task.SystemName == "scriptTime")
                {
                    ScriptTime(window);
                }

                if (task.SystemName == "scriptcantalkVanya")
                {
                    window.CanTalkVanya = true;
                    window.player.Tasks.ComplitedTask("scriptcantalkVanya");
                }

                if (task.SystemName == "scriptFinal")
                {
                    MessageBox.Show("Работа здесь была закончена, но не дело. А значит для вас ещё найдётся работка. \nКонец демоверсии, спасибо за игру :3");

                    window.player.Tasks.ComplitedTask("scriptFinal");
                    window.Close();
                }
            }
        }
    }
}
