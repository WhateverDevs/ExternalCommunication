using System.Threading;
using WhateverDevs.Core.Runtime.Common;

namespace WhateverDevs.ExternalCommunication.Runtime
{
    /// <summary>
    ///     ThreadedJob heritage from Simusafe
    /// </summary>
    public class ThreadedJob : Loggable<ThreadedJob>
    {
        private object handle = new object();

        private Thread thread;

        public virtual void Start()
        {
            thread = new Thread(Run);
            thread.Start();
        }

        public virtual void Abort()
        {
            if (thread != null) thread.Abort();
        }

        protected virtual void ThreadFunction()
        {
        }

        protected virtual void OnFinished()
        {
        }

        private void Run() => ThreadFunction();
    }
}