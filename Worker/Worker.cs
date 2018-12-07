using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ThreadWorker.Arguments;
using ThreadWorker.Code;

namespace ThreadWorker
{
    public sealed class Worker
    {
        public event EventHandler<WorkerArgs> Finish;
        public event EventHandler<WorkerStatusArgs> Next;
        public event EventHandler<WorkerStatusArgs> Status;
        public event EventHandler<WorkerExceptionArgs> Exception;
        public event EventHandler<WorkerArgs> Wait;
        public event EventHandler<WorkerArgs> Start;
        public event EventHandler<WorkerArgs> Abort;

        public bool Pause
        {
            get;
            set;
        }
        public int Elapsed
        {
            get => (int)stopwatch.ElapsedMilliseconds;
        }
        public bool Complete
        {
            get;
            private set;
        }

        private Token token;
        private Stopwatch stopwatch;
        private List<WorkContainer> jobs;
        private Thread thread;
        private bool aborted;

        public Worker()
        {
            token = new Token();
            jobs = new List<WorkContainer>();
            stopwatch = new Stopwatch();
            thread = new Thread(DoWork)
            {
                IsBackground = false
            };
        }

        public Worker(Token token)
            : this()
        {
            this.token = token;
        }

        public void Jobs(params WorkContainer[] containers)
        {
            jobs.AddRange(containers);
        }

        public void Run()
        {
            aborted = false;
            if (!thread.IsAlive)
                thread.Start();
            Start?.Invoke(this, new WorkerArgs {
                Token = token
            });
        }

        public void Stop()
        {
            aborted = true;
            if(thread.IsAlive)
                thread.Abort();
        }

        public void Join()
        {
            if (thread.IsAlive)
                thread.Join();
        }

        internal void PauseCycle()
        {
            stopwatch.Stop();
            Wait?.Invoke(this, new WorkerArgs {
                Token = token
            });
            while (Pause) ;
            stopwatch.Start();
        }

        internal void ChangeProgress(double jobProgress)
        {
            Status?.Invoke(this, new WorkerStatusArgs
            {
                TotalProgress = (int)(100 * (jobProgress * (1f / jobs.Count))),
                JobProgress = (int)(100 * jobProgress)
            });
        }

        private void DoWork()
        {
            try
            {
                Complete = false;
                stopwatch.Restart();

                int count = 0;
                DateTime started;
                foreach (WorkContainer container in jobs)
                {
                    started = DateTime.Now;
                    int startTotalProgress = (int)(100 * ((float)count * jobs.Count));
                    Next?.Invoke(this, new WorkerStatusArgs
                    {
                        Title = container.Title,
                        Started = started,
                        TotalProgress = startTotalProgress,
                        JobProgress = 0
                    });
                    Status?.Invoke(this, new WorkerStatusArgs
                    {
                        Title = container.Title,
                        Started = started,
                        TotalProgress = startTotalProgress,
                        JobProgress = 0,
                        Token = token
                    });
                    PauseCycle();

                    token.JobIndex = count;
                    ++count;
                    container.Action?.Invoke(new WorkManager(this), token);

                    Status?.Invoke(this, new WorkerStatusArgs
                    {
                        Title = container.Title,
                        Started = started,
                        TotalProgress = (int)(100 * ((float)count * jobs.Count)),
                        JobProgress = 100,
                        Token = token
                    });
                    token.ProgressIndex = 0;
                    token.ProgressStep = 0;
                }

                stopwatch.Stop();
                Complete = true;
                Finish?.Invoke(this, new WorkerArgs {
                    Token = token
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                if (!aborted)
                    Exception?.Invoke(this, new WorkerExceptionArgs
                    {
                        Exception = ex,
                        Token = token
                    });
            }
            finally
            {
                if(aborted)
                    Abort?.Invoke(this, new WorkerArgs
                    {
                        Token = token
                    });
            }
        }
    }
}
