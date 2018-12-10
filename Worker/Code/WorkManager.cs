using System;

namespace ThreadWorker.Code
{
    public class WorkManager
    {
        public bool Pause
        {
            get;
            set;
        }
        public TimeSpan Elapsed
        {
            get => TimeSpan.FromMilliseconds(worker.Elapsed);
        }
        public bool IsRunning
        {
            get => worker.IsRunning;
        }
        public bool Complete
        {
            get => worker.Complete;
        }
        public Token Token
        {
            get => worker.Token;
        }

        private Worker worker;

        internal WorkManager(Worker worker)
        {
            this.worker = worker;
        }

        public void PauseCycle()
        {
            worker.PauseCycle();
        }

        public void ChangeProgress(uint step, uint count)
        {
            if (count == 0)
                throw new DivideByZeroException("Cannot divide when count parameter is zero.");

            worker.ChangeProgress(step / (float)count);
        }

    }
}
