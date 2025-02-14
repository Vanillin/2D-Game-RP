using System;
using System.Collections.Generic;

namespace TwoD_Game_RP
{
    public interface IAction
    {
        bool IsCycle { get; }
        bool IsSystem { get; }
        bool IsCanComplete(SystemSkelet skelet, Location location);
        void CompleteAction(SystemSkelet skelet, Location location);
        IEnumerable<IAction> CreateActions(SystemSkelet skelet, Location location);
    }
    internal class ActionSelfDelete : IAction
    {
        public bool IsCycle { get; }
        public bool IsSystem => true;
        public void CompleteAction(SystemSkelet skelet, Location location)
        {
            location.RemoveFirstLayerWithCell(skelet);
        }
        public IEnumerable<IAction> CreateActions(SystemSkelet skelet, Location location)
        {
            return new List<IAction>() { this };
        }
        public bool IsCanComplete(SystemSkelet skelet, Location location) => true;
    }
    internal class ActionAttack : IAction
    {
        public bool IsCycle { get; }
        public bool IsSystem => false;
        public AliveSkelet EnemySkelet { get; }
        public ActionAttack(AliveSkelet enemySkelet, bool iscycle)
        {
            IsCycle = iscycle;
            EnemySkelet = enemySkelet;
        }
        public void CompleteAction(SystemSkelet skelet, Location location)
        {
            if (!(skelet is AliveSkelet)) throw new CustomException("Skelet not a class Skelet");
            var rotate = -(int)Math.Truncate(Math.Atan2(-EnemySkelet.GPoint.X + skelet.GPoint.X, EnemySkelet.GPoint.Y - skelet.GPoint.Y) * (180 / Math.PI)) + 90;
            var shootSkelet = new SimpleElement("Shoot", (skelet as AliveSkelet).GetGunInHand().PictureAttack, skelet.GPoint, true);
            shootSkelet.Rotate = rotate;
            shootSkelet.EnqueueUpGlobalAction(new ActionWait(1, false));
            shootSkelet.EnqueueUpGlobalAction(new ActionSelfDelete());

            //var hitSkelet = new SystemSket(EnemySkelet.Cord, 0, "Hit", false);
            //hitSkelet.EnqueueUpGlobalAction(new ActionWait(1, false));
            //hitSkelet.EnqueueUpGlobalAction(new ActionSelfDelete());

            //location.AddSecondLayerWithCell(hitSkelet);

            location.AddSecondLayerWithCell(shootSkelet);
            EnemySkelet.MinusHealth((skelet as AliveSkelet).GetGunInHand().Damage);
        }
        public IEnumerable<IAction> CreateActions(SystemSkelet skelet, Location location)
        {
            if (!(skelet is AliveSkelet)) throw new CustomException("Skelet not a class Skelet");
            List<IAction> actions = new List<IAction>();
            var lenPathVisible = location.CreateLenghtPathVisible(skelet.GPoint, EnemySkelet.GPoint);
            if (lenPathVisible >= (skelet as AliveSkelet).GetGunInHand().Radius)
            {
                var path = location.CreatePath_OutOfPoint(skelet.GPoint, EnemySkelet.GPoint, location.IsBusy);
                actions.Add(new ActionMove(path[0], false));
            }
            actions.Add(new ActionAttack(EnemySkelet, false));
            actions.Add(new ActionDelete());
            return actions;
        }
        public bool IsCanComplete(SystemSkelet skelet, Location location)
        {
            if (!(skelet is AliveSkelet)) throw new CustomException("Skelet not a class Skelet");
            var lenPathVisible = location.CreateLenghtPathVisible(skelet.GPoint, EnemySkelet.GPoint);
            if (lenPathVisible <= (skelet as AliveSkelet).GetGunInHand().Radius)
                return true;
            return false;
        }
    }
    internal class ActionDelete : IAction
    {
        public bool IsCycle => false;
        public bool IsSystem => true;
        public void CompleteAction(SystemSkelet skelet, Location location)
        {
            skelet.RemoveGlobalAction();
        }
        public IEnumerable<IAction> CreateActions(SystemSkelet skelet, Location location)
        {
            throw new CustomException("Its not Global Action");
        }
        public bool IsCanComplete(SystemSkelet skelet, Location location) => true;
    }
    internal class ActionWait : IAction
    {
        public int Count { get; set; }
        public bool IsCycle { get; }
        public bool IsSystem => false;

        public ActionWait(int count, bool isCycle)
        {
            Count = count;
            IsCycle = isCycle;
        }
        public bool IsCanComplete(SystemSkelet skelet, Location location) => true;
        public void CompleteAction(SystemSkelet skelet, Location location)
        {
            //waiting (empty)
        }
        public IEnumerable<IAction> CreateActions(SystemSkelet skelet, Location location)
        {
            List<IAction> actions = new List<IAction>();
            for (int i = 0; i < Count; i++)
            {
                actions.Add(new ActionWait(0, false));
            }
            actions.Add(new ActionDelete());
            return actions;
        }
    }
    internal class ActionMove : IAction
    {
        public GamePoint GamePoint { get; set; }
        public bool IsSystem => false;
        public bool IsCycle { get; }

        public ActionMove(GamePoint point, bool isCycle)
        {
            GamePoint = point;
            IsCycle = isCycle;
        }
        public void CompleteAction(SystemSkelet skelet, Location location)
        {
            location.MoveFirstLayerWithCell(skelet, GamePoint);
        }
        public IEnumerable<IAction> CreateActions(SystemSkelet skelet, Location location)
        {
            List<IAction> retur = new List<IAction>();
            List<GamePoint> path;
            if (skelet is AliveSkelet)
                path = location.CreatePath_OutOfPoint(skelet.GPoint, GamePoint, location.IsBusy);
            else
                path = location.CreatePath_OutOfPoint(skelet.GPoint, GamePoint, new CustomSortedEnum<GamePoint>());
            if (path != null)
                foreach (var v in path)
                {
                    retur.Add(new ActionMove(v, false));
                }
            retur.Add(new ActionDelete());
            return retur;
        }
        public bool IsCanComplete(SystemSkelet skelet, Location location)
        {
            //  SystemSket can go in Skelet
            if (skelet is AliveSkelet)
                return !location.IsCellBusy((int)GamePoint.X, (int)GamePoint.Y);
            else return true;
        }
    }
    internal class ActionTeleport : IAction
    {
        public GamePoint GamePoint { get; set; }
        public bool IsSystem => false;
        public bool IsCycle { get; }

        public ActionTeleport(GamePoint point, bool isCycle)
        {
            GamePoint = point;
            IsCycle = isCycle;
        }
        public void CompleteAction(SystemSkelet skelet, Location location)
        {
            location.MoveFirstLayerWithCell(skelet, GamePoint);
        }
        public IEnumerable<IAction> CreateActions(SystemSkelet skelet, Location location)
        {
            return new List<IAction>() { this };
        }
        public bool IsCanComplete(SystemSkelet skelet, Location location)
        {
            //  SystemSket can go in Skelet
            if (skelet is AliveSkelet)
                return !location.IsCellBusy((int)GamePoint.X, (int)GamePoint.Y);
            else return true;
        }
    }
}