using System;
using System.Collections.Generic;

namespace TwoD_Game_RP
{
    internal interface IMemorySmartAction : IMemoryAction
    {    }

    internal class MemoryAttackAction : MemoryAction, IMemorySmartAction
    {
        private bool DoAction(EnemySkelet skelet, Location location)
        {
            if (_globalActions.Count != 0 && _globalActions.Peek() is ActionAttack)
                return base.DoAction(skelet, location);
            
            int lenWatch = skelet.LenWatch;
            var cord = skelet.GPoint;
            var OblWatch = location.GetWatchCirlce(cord, lenWatch - 0.1,
                Math.Max((int)cord.X - lenWatch, 0), Math.Max((int)cord.Y - lenWatch, 0),
                Math.Min((int)cord.X + lenWatch + 1, location.Height), Math.Min((int)cord.Y + lenWatch + 1, location.Width));
            var list = OblWatch.ToList();
            foreach (SystemSkelet enemyskelet in location.GetLives())
            {
                if (enemyskelet is AliveSkelet && OblWatch.Contains(enemyskelet.GPoint))
                    if (!skelet.FriendFranction.Contains((enemyskelet as AliveSkelet).Fraction))
                    {
                        EnqueueUpGlobalAction(new ActionAttack(enemyskelet as AliveSkelet, false));
                        break;
                    }
            }
            return base.DoAction(skelet, location);
        }
        public override bool DoAction(SystemSkelet skelet, Location location)
        {
            if (skelet is EnemySkelet)
                return DoAction(skelet as EnemySkelet, location);
            return base.DoAction(skelet, location);
        }
    }
}
