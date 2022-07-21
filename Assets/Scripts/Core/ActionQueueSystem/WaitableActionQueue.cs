using System.Collections.Generic;

namespace Core.ActionQueueSystem
{
    public static class WaitableActionQueue
    {
        private static readonly Queue<IWaitableAction> actions;
        private static WaitableAction plug;

        static WaitableActionQueue()
        {
            actions = new Queue<IWaitableAction>();
            Start();
        }
    
        public static void Add(IWaitableAction waitableAction)
        {
            actions.Enqueue(waitableAction);
            plug?.End();
        }

        private static void Start()
        {
            if (actions.Count == 0)
            {
                plug = new WaitableAction(null);
                plug.Ended += ResetPlug;
                actions.Enqueue(plug);
            }
        
            var action = actions.Peek();
            action.Ended += Start;
            action.Begin();
            actions.Dequeue();
        }

        private static void ResetPlug()
        {
            plug = null;
        }
    }
}
