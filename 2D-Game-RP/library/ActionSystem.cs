﻿using System;
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
    */
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
        public bool IsCanComplete(SystemSkelet skelet, Location location)
        {
            return true;
        }
    }
    public class ActionAttack : IAction
    {
        public bool IsCycle { get; }
        public bool IsSystem => false;
        public Skelet EnemySkelet { get; }
        public ActionAttack(Skelet enemySkelet, bool iscycle)
        {
            IsCycle = iscycle;
            EnemySkelet = enemySkelet;
        }
        public void CompleteAction(SystemSkelet skelet, Location location)
        {
            if (!(skelet is Skelet)) throw new CustomException("Skelet not a class Skelet");
            var rotate = -(int)Math.Truncate(Math.Atan2(-EnemySkelet.Cord.X + skelet.Cord.X, EnemySkelet.Cord.Y - skelet.Cord.Y) * (180 / Math.PI)) + 90;
            var shootSkelet = new SystemSkelet(skelet.Cord, rotate, "Shoot", true, (skelet as Skelet).GetGunInHand().PictureAttack);
            shootSkelet.EnqueueUpGlobalAction(new ActionWait(1, false));
            shootSkelet.EnqueueUpGlobalAction(new ActionSelfDelete());

            //var hitSkelet = new SystemSket(EnemySkelet.Cord, 0, "Hit", false);
            //hitSkelet.EnqueueUpGlobalAction(new ActionWait(1, false));
            //hitSkelet.EnqueueUpGlobalAction(new ActionSelfDelete());

            location.AddSecondLayerWithCell(shootSkelet);
            //location.AddSecondLayerWithCell(hitSkelet);
            EnemySkelet.MinusHealth((skelet as Skelet).GetGunInHand().Damage);
        }
        public IEnumerable<IAction> CreateActions(SystemSkelet skelet, Location location)
        {
            if (!(skelet is Skelet)) throw new CustomException("Skelet not a class Skelet");
            List<IAction> actions = new List<IAction>();
            var lenPathVisible = location.CreateLenghtPathVisible(skelet.Cord, EnemySkelet.Cord);
            if (lenPathVisible >= (skelet as Skelet).GetGunInHand().Radius)
            {
                var path = location.CreatePath_OutOfPoint(skelet.Cord, EnemySkelet.Cord, location.IsBusy);
                actions.Add(new ActionMove(path[0], false));
            }
            actions.Add(new ActionAttack(EnemySkelet, false));
            actions.Add(new ActionDelete());
            return actions;
        }
        public bool IsCanComplete(SystemSkelet skelet, Location location)
        {
            if (!(skelet is Skelet)) throw new CustomException("Skelet not a class Skelet");
            var lenPathVisible = location.CreateLenghtPathVisible(skelet.Cord, EnemySkelet.Cord);
            if (lenPathVisible <= (skelet as Skelet).GetGunInHand().Radius)
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
        public bool IsCanComplete(SystemSkelet skelet, Location location)
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
        public bool IsCanComplete(SystemSkelet skelet, Location location) => true;
        public void CompleteAction(SystemSkelet skelet, Location location)
        {
            //waiting
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
        public void CompleteAction(SystemSkelet skelet, Location location)
        {
            location.MoveFirstLayerWithCell(skelet, GamePoint);
        }
        public IEnumerable<IAction> CreateActions(SystemSkelet skelet, Location location)
        {
            List<IAction> retur = new List<IAction>();
            List<GamePoint> path;
            if (skelet is Skelet)
                path = location.CreatePath_OutOfPoint(skelet.Cord, GamePoint, location.IsBusy);
            else
                path = location.CreatePath_OutOfPoint(skelet.Cord, GamePoint, new CustomSortedEnum<GamePoint>());
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
            if (skelet is Skelet)
                return !location.IsCellBusy((int)GamePoint.X, (int)GamePoint.Y);
            else return true;
        }
    }
    public class ActionTeleport : IAction
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
            if (skelet is Skelet)
                return !location.IsCellBusy((int)GamePoint.X, (int)GamePoint.Y);
            else return true;
        }
    }
    public interface IAction
    {
        bool IsCycle { get; }
        bool IsSystem { get; }
        bool IsCanComplete(SystemSkelet skelet, Location location);
        void CompleteAction(SystemSkelet skelet, Location location);
        IEnumerable<IAction> CreateActions(SystemSkelet skelet, Location location);
    }
}