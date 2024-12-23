using System.Collections.Generic;

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

    internal class ActionDelete : IAction
    {
        public bool IsCycle => false;
        public bool IsSystem => true;
        public void CompleteAction(Skelet skelet, Location location)
        {
            skelet.RemoveGlobalAction();
        }
        public IEnumerable<IAction> CreateActions(Skelet skelet, Location location)
        {
            throw new CustomException("Its not Global Action");
        }
        public bool IsCanComplete(Skelet skelet, Location location)
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
        public bool IsCanComplete(Skelet skelet, Location location)
        {
            return true;
        }
        public void CompleteAction(Skelet skelet, Location location)
        {
            //waiting
        }
        public IEnumerable<IAction> CreateActions(Skelet skelet, Location location)
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
        public void CompleteAction(Skelet skelet, Location location)
        {
            location.MoveFirstLayerWithCell(skelet, GamePoint);
        }
        public IEnumerable<IAction> CreateActions(Skelet skelet, Location location)
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
        public bool IsCanComplete(Skelet skelet, Location location)
        {
            return !location.IsCellBusy((int)GamePoint.X, (int)GamePoint.Y);
        }
    }

    public interface IAction
    {
        bool IsCycle { get; }
        bool IsSystem { get; }
        bool IsCanComplete(Skelet skelet, Location location);
        void CompleteAction(Skelet skelet, Location location);
        IEnumerable<IAction> CreateActions(Skelet skelet, Location location);
    }
}