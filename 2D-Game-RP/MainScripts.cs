using System;
using System.Windows;
using System.Windows.Threading;

namespace TwoD_Game_RP
{
    internal class MainScripts
    {
        public static void ScriptTime(MainWindow window)
        {
            MessageBox.Show("Вы подождали до вечера. С работы пришли жильцы.");

            window.CurrentLocation.AddLivesWithCell(new Maksim(new GamePoint(17, 21), '0'));

            window.player.Tasks.ComplitedTask("scriptTime");
        }
        public static void ScriptFinal(MainWindow window)
        {
            MessageBox.Show("Работа здесь была закончена, но не дело. А значит для вас ещё найдётся работка");

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
