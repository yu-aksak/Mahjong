using System;

namespace Core.ActionQueueSystem
{
    public class WaitableAction : IWaitableAction
    {
        private Action action;

        public WaitableAction(Action action)
        {
            this.action = action;
        }
    
        public void Begin()
        {
            action?.Invoke();
        }

        public event Action Ended;

        public void End()
        {
            Ended();
        }
    }
}
