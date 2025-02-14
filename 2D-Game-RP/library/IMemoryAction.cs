using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{
    internal interface IMemoryAction
    {
        bool IsEmptyAction();
        bool DoAction(SystemSkelet skelet, Location location);
        void RemoveGlobalAction();
        void EnqueueUpGlobalAction(IAction action);
        void EnqueueDownGlobalAction(IAction action);
        void ClearGlobalActions();
        void ClearActions();
    }
    internal class MemoryAction : IMemoryAction
    {
        internal CustomBilateralQueue<IAction> _globalActions;
        internal CustomBilateralQueue<IAction> _actions;

        public MemoryAction()
        {
            _globalActions = new CustomBilateralQueue<IAction>();
            _actions = new CustomBilateralQueue<IAction>();
        }

        public bool IsEmptyAction()
        {
            if (PeekGlobalAction() != null) return false;
            if (PeekAction() != null) return false;
            return true;
        }
        public virtual bool DoAction(SystemSkelet skelet, Location location)
        {
            if (PeekGlobalAction() == null) return false;
            if (PeekAction() == null) CreateActions(PeekGlobalAction().CreateActions(skelet, location));

            if (!PeekAction().IsCanComplete(skelet, location))
            {
                ClearActions();
                CreateActions(PeekGlobalAction().CreateActions(skelet, location));
            }
            if (!PeekAction().IsCanComplete(skelet, location)) return false;

            PeekAction().CompleteAction(skelet, location);
            RemoveAction();
            while (PeekAction() != null && PeekAction().IsSystem)
            {
                PeekAction().CompleteAction(skelet, location);
                RemoveAction();
            }
            return true;
        }
        public void ClearGlobalActions() => _globalActions.Clear();
        public void ClearActions() => _actions.Clear();
        public void EnqueueUpGlobalAction(IAction action) => _globalActions.EnqueueInFront(action);
        public void EnqueueDownGlobalAction(IAction action) => _globalActions.EnqueueInBack(action);
        public void RemoveGlobalAction()
        {
            var v = _globalActions.Dequeue();
            if (v.IsCycle)
            {
                _globalActions.EnqueueInBack(v);
            }
        }
        private void RemoveAction() => _actions.RemoveUp();
        private void CreateActions(IEnumerable<IAction> actions)
        {
            foreach (var v in actions)
            {
                _actions.EnqueueInBack(v);
            }
        }
        private IAction PeekGlobalAction()
        {
            if (_globalActions.Count == 0) return null;
            return _globalActions.Peek();
        }
        private IAction PeekAction()
        {
            if (_actions.Count == 0) return null;
            return _actions.Peek();
        }
    }
}
