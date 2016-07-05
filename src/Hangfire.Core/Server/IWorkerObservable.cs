using System;

namespace Hangfire.Server
{
    public interface IWorkerObservable
    {
        void Subscribe(IWorkerObserver observer);
    }
}