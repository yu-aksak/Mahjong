using System;

namespace Core.ActionQueueSystem
{
    public interface IWaitableAction
    {
        void Begin();
        event Action Ended;
    }
}
