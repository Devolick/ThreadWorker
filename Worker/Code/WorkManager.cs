using System;

namespace ThreadWorker.Code
{
    /// <summary>
    /// Worker sender shell.
    /// </summary>
    public class WorkManager
    {
        /// <summary>
        /// Returns the elapsed time in milliseconds.
        /// </summary>
        public TimeSpan Elapsed
        {
            get => worker.Elapsed;
        }
        /// <summary>
        /// Indicates if the thread is running.
        /// </summary>
        public bool IsRunning
        {
            get => worker.IsRunning;
        }
        /// <summary>
        /// Indicates if the thread has finished running.
        /// </summary>
        public bool Complete
        {
            get => worker.Complete;
        }
        /// <summary>
        /// Flexible class for data entry.
        /// </summary>
        public Token Token
        {
            get => worker.Token;
        }

        private readonly Worker worker;

        internal WorkManager(Worker worker)
        {
            this.worker = worker;
        }

        /// <summary>
        /// Loops task when pause is called.
        /// </summary>
        public void PauseCycle()
        {
            worker.PauseCycle();
        }

        /// <summary>
        /// Used to calculate the progress of the current task.
        /// </summary>
        public void ChangeProgress(uint step, uint count)
        {
            if (count == 0)
                throw new DivideByZeroException("Cannot divide when count parameter is zero.");

            worker.ChangeProgress(step / (float)count);
        }

    }
}
