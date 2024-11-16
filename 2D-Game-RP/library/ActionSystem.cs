using System;
using System.Collections.Generic;

namespace TwoD_Game_RP
{
    /*
     

    using System;
using System.Collections.Generic;

namespace Maze_runner_Library
{
    internal interface IActions 
    {
        bool TakeAction(Subject sub, Graf graf);
        List<IActions> ConvertToLocalActions(Subject sub, Graf graf);
    };

    /// <summary>
    /// Действие неактивности (прозрачности)
    /// </summary>
    internal class ActionClear : IActions
    {
        public int Count { get; set; }
        public ActionClear()
        {
            Count = 1;
        }
        public ActionClear(int count)
        {
            Count = count;
        }
        public bool TakeAction(Subject sub, Graf graf)
        {
            sub.IsEmpty = true;
            return false;
        }

        public List<IActions> ConvertToLocalActions(Subject sub, Graf graf)
        {
            List<IActions> actions = new List<IActions>();
            for (int i = 0; i < Count; i++)
                actions.Add(new ActionClear());
            return actions;
        }
    }
    
    /// <summary>
    /// Действие поворота
    /// </summary>
    internal class ActionWatch : IActions
    {
        public double Angle { get; set; }
        public ActionWatch(double angle)
        {
            if (angle < 0) angle += 360;
            else if (angle >= 360) angle -= 360;
            Angle = angle;
        }

        public bool TakeAction(Subject sub, Graf graf)
        {
            sub.Angle = Angle;
            sub.ReloadAnimation();
            return false;
        }

        public List<IActions> ConvertToLocalActions(Subject sub, Graf graf)
        {
            List<IActions> actions = new List<IActions>();
            List<ActionWatch> pathWatch = ActionsHelper.CreatePathWatchToAngle(Angle, sub.Angle);
            foreach (var w in pathWatch)
                actions.Add(w);
            return actions;
        }
    }
    /// <summary>
    /// Действие телепортации
    /// </summary>
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

        public bool TakeAction(Subject sub, Graf graf)
        {
            if (graf.Vertexes[PointOnGraf].Busy == true && !sub.PointOnGraf.Equals(PointOnGraf))
            {
                sub.Actions.ClearLocalActions();
                return true;
            }
            graf.Vertexes[sub.PointOnGraf].Busy = false;
            sub.PointOnGraf = PointOnGraf;
            graf.Vertexes[sub.PointOnGraf].Busy = true;
            sub.GamePoint = GamePoint;
            sub.Angle = Angle;

            sub.NextPicture();
            return false;
        }

        public List<IActions> ConvertToLocalActions(Subject sub, Graf graf)
        {
            return new List<IActions>() {this};
        }
    }
    /// <summary>
    /// Действие атаки
    /// </summary>
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

        public bool TakeAction(Subject sub, Graf graf)
        {
            Enemy.MinusHealth(Damage);
            return true;
        }
        public List<IActions> ConvertToLocalActions(Subject sub, Graf graf)
        {
            List<IActions> actions = new List<IActions>();
            actions.Add(new SystemActionChangeSystemPicture(new AnimatedPicCell(new string[] { "Image/system/aiming1.png", "Image/system/aiming2.png", "Image/system/aiming3.png" }, 0)));
            actions.Add(new ActionWait());
            for (int i = 0; i < CountAiming - 1; i++)
                actions.Add(new ActionSystemPicture());

            actions.Add(new SystemActionChangeSystemPicture(new StaticPicCell("Image/system/puli.png", 0)));
            actions.Add(this);
            actions.Add(new ActionSystemPicture());
            return actions;
        }
    }
    /// <summary>
    /// Системное действие
    /// </summary>
    internal class ActionSystemPicture : IActions
    {
        public ActionSystemPicture() { }

        public bool TakeAction(Subject sub, Graf graf)
        {
            sub.NextPicture();
            return false;
        }
        public List<IActions> ConvertToLocalActions(Subject sub, Graf graf)
        {
            throw new Exception("Is internal class, bug");
        }
    }
    /// <summary>
    /// Системное действие
    /// </summary>
    internal class SystemActionChangeSystemPicture : IActions
    {
        public IPictureCell picture { get; set; }
        public SystemActionChangeSystemPicture(IPictureCell pictureCell)
        {
            picture = pictureCell;
        }

        public bool TakeAction(Subject sub, Graf graf)
        {
            sub.AddSystemPictures(picture);
            return true;
        }
        public List<IActions> ConvertToLocalActions(Subject sub, Graf graf)
        {
            throw new Exception("Is internal class, bug");
        }
    }
}
    */

    public class ActionDelete : IAction
    {
        public bool IsCycle => false;
        public bool IsSystem => true;
        public void CompleteAction(Skelet skelet, Location location)
        {
            skelet.RemoveGlobalAction();
        }
        public IEnumerable<IAction> CreateActions(Skelet skelet, Location location)
        {
            throw new Exception("Its not Global Action");
        }
        public bool IsCanComplete(Skelet skelet, Location location)
        {
            return true;
        }
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
            location.MoveLivesWithCell(skelet, GamePoint);
        }
        public IEnumerable<IAction> CreateActions(Skelet skelet, Location location)
        {
            List<IAction> retur = new List<IAction>();
            var path = location.GrafLocToMove.SearchWidthWithoutSomePoint(skelet.Cord, GamePoint, location.IsBusy);
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
