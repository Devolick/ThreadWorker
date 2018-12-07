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
        public bool IsRunning
        {
            get;
            private set;
        }
        public bool Complete
        {
            get;
            private set;
        }
        public Token Token { get; set; }

        private Stopwatch stopwatch;
        private List<WorkContainer> jobs;
        private Thread thread;
        private bool aborted;

        public Worker()
        {
            Token = new Token();
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
            this.Token = token;
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
                Token = Token
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
                Token = Token
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
                IsRunning = true;

                int count = 0;
                DateTime started;
                foreach (WorkContainer container in jobs)
                {
                    started = DateTime.Now;
                    int startTotalProgress = (int)(100 * ((float)count * jobs.Count));
                    Next?.Invoke(this, new WorkerStatusArgs
                    {
                        Title = container.Title,
                        DateTime = started,
                        TotalProgress = startTotalProgress,
                        JobProgress = 0
                    });
                    Status?.Invoke(this, new WorkerStatusArgs
                    {
                        Title = container.Title,
                        DateTime = started,
                        TotalProgress = startTotalProgress,
                        JobProgress = 0,
                        Token = Token
                    });
                    PauseCycle();

                    Token.JobIndex = count;
                    ++count;
                    container.Action?.Invoke(new WorkManager(this), Token);

                    Status?.Invoke(this, new WorkerStatusArgs
                    {
                        Title = container.Title,
                        DateTime = DateTime.Now,
                        TotalProgress = (int)(100 * ((float)count * jobs.Count)),
                        JobProgress = 100,
                        Token = Token
                    });
                    Token.ProgressIndex = 0;
                    Token.ProgressStep = 0;
                }
                stopwatch.Stop();
                IsRunning = false;
                Complete = true;
                Finish?.Invoke(this, new WorkerArgs {
                    Token = Token
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                IsRunning = false;
                if (!aborted)
                    Exception?.Invoke(this, new WorkerExceptionArgs
                    {
                        Exception = ex,
                        Token = Token
                    });
            }
            finally
            {
                if (aborted)
                    Abort?.Invoke(this, new WorkerArgs
                    {
                        Token = Token
                    });
                Token = new Token();
            }
        }
    }
}
