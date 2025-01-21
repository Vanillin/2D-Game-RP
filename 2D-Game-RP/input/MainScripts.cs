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
                //if (task.SystemName == "trainingButton")
                //{
                //    MessageBox.Show("Управление: \nКнопками: WASD для ходьбы \nМышкой: ЛКМ для ходьбы, ПКМ для взаимодействия с чем-либо");
                //    window.player.Tasks.ComplitedTask("trainingButton");
                //}
            }
        }
    }
}
