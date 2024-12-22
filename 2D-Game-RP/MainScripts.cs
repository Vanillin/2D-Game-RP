using System.Windows;

namespace TwoD_Game_RP
{
    internal class MainScripts
    {
        public static void ScriptTime(MainWindow window)
        {
            MessageBox.Show("Вы подождали до вечера. С работы пришли жильцы.");
            window.TimeLabel.Content = "Время: 19**/06/13 ~21 час";

            var maksim = new Maksim(new GamePoint(17, 19), '0');
            maksim.EnqueueDownGlobalAction(new ActionWait(5, true));
            maksim.EnqueueDownGlobalAction(new ActionMove(new GamePoint(16, 23), true));
            maksim.EnqueueDownGlobalAction(new ActionWait(1, true));
            maksim.EnqueueDownGlobalAction(new ActionMove(new GamePoint(17, 19), true));
            window.CurrentLocation.AddLivesWithCell(maksim);

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
        public static void ScriptFinal(MainWindow window)
        {
            MessageBox.Show("Работа здесь была закончена, но не дело. А значит для вас ещё найдётся работка. \nКонец демоверсии, спасибо за игру :3");

            window.player.Tasks.ComplitedTask("scriptFinal");

            window.Close();
        }
        public static void ScriptCanTalkVanya(MainWindow window)
        {
            window.CanTalkVanya = true;

            window.player.Tasks.ComplitedTask("scriptcantalkVanya");
        }
    }
}
