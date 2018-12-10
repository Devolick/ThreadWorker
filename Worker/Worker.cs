using Newtonsoft.Json;
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
        public double Elapsed
        {
            get => stopwatch.ElapsedMilliseconds;
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
        public Token Token
        {
            get;
            set;
        }

        private Stopwatch stopwatch;
        private WorkContainer currentContainer;
        private DateTime currentContainerDateTime;
        private List<WorkContainer> containers;
        private Thread thread;
        private bool aborted;
        private int totalProgress;
        private int workProgress;

        public Worker()
        {
            Token = new Token();
            containers = new List<WorkContainer>();
            stopwatch = new Stopwatch();
        }

        public Worker(Token token)
            : this()
        {
            this.Token = token;
        }

        public void Works(params WorkContainer[] containers)
        {
            this.containers.AddRange(containers);
        }

        public void Run()
        {
            IsRunning = false;
            Complete = false;
            Pause = false;
            aborted = false;
            if (thread == null || !thread.IsAlive)
            {
                thread = new Thread(DoWork)
                {
                    IsBackground = false
                };
                thread.Start();
            }
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
            bool inPause = Pause;
            if (Pause)
                Wait?.Invoke(this, new WorkerWaitArgs
                {
                    TotalProgress = totalProgress,
                    WorkProgress = workProgress,
                    Title = currentContainer.Title,
                    DateTime = currentContainerDateTime,
                    Token = Token
                });
            while (Pause) ;
            if (inPause)
                Wait?.Invoke(this, new WorkerWaitArgs
                {
                    TotalProgress = totalProgress,
                    WorkProgress = workProgress,
                    Title = currentContainer.Title,
                    DateTime = currentContainerDateTime,
                    Token = Token
                });
            stopwatch.Start();
        }

        internal void ChangeProgress(double progress)
        {
            int total = (int)(100 * ((float)Token.TaskIndex / containers.Count));
            totalProgress = total + (int)(100 * (progress * (1f / containers.Count)));
            workProgress = (int)(100 * progress);
            Status?.Invoke(this, new WorkerStatusArgs
            {
                TotalProgress = totalProgress,
                WorkProgress = workProgress,
                Title = currentContainer.Title,
                DateTime = currentContainerDateTime,
                Token = Token
            });
        }

        private void DoWork()
        {
            try
            {
                stopwatch.Restart();
                IsRunning = true;

                if (containers.Count > 0)
                {
                    int count = 0;
                    foreach (WorkContainer container in containers)
                    {
                        currentContainer = container;
                        Token.TaskIndex = count;
                        currentContainerDateTime = DateTime.Now;
                        totalProgress = (int)(100 * ((float)Token.TaskIndex / containers.Count));
                        workProgress = 0;
                        Next?.Invoke(this, new WorkerStatusArgs
                        {
                            Title = container.Title,
                            DateTime = currentContainerDateTime,
                            TotalProgress = totalProgress,
                            WorkProgress = workProgress,
                            Token = Token
                        });
                        Status?.Invoke(this, new WorkerStatusArgs
                        {
                            Title = container.Title,
                            DateTime = currentContainerDateTime,
                            TotalProgress = totalProgress,
                            WorkProgress = workProgress,
                            Token = Token
                        });
                        PauseCycle();
                        container.Action?.Invoke(new WorkManager(this), new TokenManager(Token));

                        totalProgress = (int)(100 * ((float)(Token.TaskIndex + 1) / containers.Count));
                        workProgress = 100;
                        Status?.Invoke(this, new WorkerStatusArgs
                        {
                            Title = container.Title,
                            DateTime = DateTime.Now,
                            TotalProgress = totalProgress,
                            WorkProgress = workProgress,
                            Token = Token
                        });

                        ++count;
                    }
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
                        Exception = new Exception(
                            $"Exception at token index:{Token.TaskIndex}",ex),
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
