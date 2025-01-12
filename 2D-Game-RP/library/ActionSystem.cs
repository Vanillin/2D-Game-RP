using System;
using System.Collections.Generic;
using System.Windows;

namespace TwoD_Game_RP
{
    /*
    internal class ActionClear : IActions //(прозрачности)
    {
        public int Count { get; set; }
        public ActionClear(int count)
        {
            Count = count;
        }
    }
    internal class ActionWatch : IActions
    {
        public double Angle { get; set; }
        public ActionWatch(double angle)
        {
            if (angle < 0) angle += 360;
            else if (angle >= 360) angle -= 360;
            Angle = angle;
        }
    }
    internal class ActionTeleport : IActions
    {
        public GamePoint GamePoint { get; set; }
        public GamePoint PointOnGraf { get; set; }
        public double Angle { get; set; }
        public ActionTeleport(GamePoint point, GamePoint pointOnGraf, double angle)
        {
            GamePoint = point;
            Angle = angle;
            PointOnGraf = pointOnGraf;
        }
    }
    internal class ActionAttack : IActions
    {
        public Creature Enemy { get; set; }
        public int CountAiming { get; set; }
        public int Damage { get; set; }
        public ActionAttack(Creature enemy, int countAiming, int damage)
        {
            Enemy = enemy;
            CountAiming = countAiming;
            Damage = damage;
        }
    }
    */
    internal class ActionSelfDelete : IAction
    {
        public bool IsCycle { get; }
        public bool IsSystem => true;
        public void CompleteAction(SystemSket skelet, Location location)
        {
            location.RemoveFirstLayerWithCell(skelet);
        }
        public IEnumerable<IAction> CreateActions(SystemSket skelet, Location location)
        {
            return new List<IAction>() { this };
        }
        public bool IsCanComplete(SystemSket skelet, Location location)
        {
            return true;
        }
    }
    public class ActionAttack : IAction
    {
        public bool IsCycle {get;}
        public bool IsSystem => false;
        public Skelet EnemySkelet { get; }
        public ActionAttack(Skelet enemySkelet, bool iscycle)
        {
            IsCycle = iscycle;
            EnemySkelet = enemySkelet;
        }
        public void CompleteAction(SystemSket skelet, Location location)
        {
            if (!(skelet is Skelet)) throw new CustomException("Skelet not a class Skelet");
            var rotate = -(int)Math.Truncate(Math.Atan2(-EnemySkelet.Cord.X + skelet.Cord.X, EnemySkelet.Cord.Y - skelet.Cord.Y) * (180 / Math.PI)) + 90;
            var shootSkelet = new SystemSket(skelet.Cord, rotate, "Shoot", false);
            shootSkelet.EnqueueUpGlobalAction(new ActionWait(1, false));
            shootSkelet.EnqueueUpGlobalAction(new ActionSelfDelete());

            //var hitSkelet = new SystemSket(EnemySkelet.Cord, 0, "Hit", false);
            //hitSkelet.EnqueueUpGlobalAction(new ActionWait(1, false));
            //hitSkelet.EnqueueUpGlobalAction(new ActionSelfDelete());

            location.AddSecondLayerWithCell(shootSkelet);
            //location.AddSecondLayerWithCell(hitSkelet);
            EnemySkelet.MinusHealth((skelet as Skelet).GetGunInHand().Damage);
        }
        public IEnumerable<IAction> CreateActions(SystemSket skelet, Location location)
        {
            if (!(skelet is Skelet)) throw new CustomException("Skelet not a class Skelet");
            List<IAction> actions = new List<IAction>();
            var path = location.CreatePath_OutOfPoint(skelet.Cord, EnemySkelet.Cord, location.IsBusy);
            if (path.Count > (skelet as Skelet).GetGunInHand().Radius)
                actions.Add(new ActionMove(path[0], false));
            actions.Add(new ActionAttack(EnemySkelet, false));
            actions.Add(new ActionDelete());
            return actions;
        }
        public bool IsCanComplete(SystemSket skelet, Location location)
        {
            if (!(skelet is Skelet)) throw new CustomException("Skelet not a class Skelet");
            return location.CreatePath_OutOfPoint(skelet.Cord, EnemySkelet.Cord, new CustomSortedEnum<GamePoint>()).Count <= (skelet as Skelet).GetGunInHand().Radius;
        }
    }
    internal class ActionDelete : IAction
    {
        public bool IsCycle => false;
        public bool IsSystem => true;
        public void CompleteAction(SystemSket skelet, Location location)
        {
            skelet.RemoveGlobalAction();
        }
        public IEnumerable<IAction> CreateActions(SystemSket skelet, Location location)
        {
            throw new CustomException("Its not Global Action");
        }
        public bool IsCanComplete(SystemSket skelet, Location location)
        {
            return true;
        }
    }
    public class ActionWait : IAction
    {
        public int Count { get; set; }
        public bool IsCycle { get; }
        public bool IsSystem => false;

        public ActionWait(int count, bool isCycle)
        {
            Count = count;
            IsCycle = isCycle;
        }
        public bool IsCanComplete(SystemSket skelet, Location location)
        {
            return true;
        }
        public void CompleteAction(SystemSket skelet, Location location)
        {
            //waiting
        }
        public IEnumerable<IAction> CreateActions(SystemSket skelet, Location location)
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
    public class ActionMove : IAction
    {
        public GamePoint GamePoint { get; set; }
        public bool IsSystem => false;
        public bool IsCycle { get; }

        public ActionMove(GamePoint point, bool isCycle)
        {
            GamePoint = point;
            IsCycle = isCycle;
        }
        public void CompleteAction(SystemSket skelet, Location location)
        {
            location.MoveFirstLayerWithCell(skelet, GamePoint);
        }
        public IEnumerable<IAction> CreateActions(SystemSket skelet, Location location)
        {
            List<IAction> retur = new List<IAction>();
            var path = location.CreatePath_OutOfPoint(skelet.Cord, GamePoint, location.IsBusy);
            if (path != null)
                foreach (var v in path)
                {
                    retur.Add(new ActionMove(v, false));
                }
            retur.Add(new ActionDelete());
            return retur;
        }
        public bool IsCanComplete(SystemSket skelet, Location location)
        {
            return !location.IsCellBusy((int)GamePoint.X, (int)GamePoint.Y);
        }
    }

    public interface IAction
    {
        bool IsCycle { get; }
        bool IsSystem { get; }
        bool IsCanComplete(SystemSket skelet, Location location);
        void CompleteAction(SystemSket skelet, Location location);
        IEnumerable<IAction> CreateActions(SystemSket skelet, Location location);
    }
}