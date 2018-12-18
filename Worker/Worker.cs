using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ThreadWorker.Arguments;
using ThreadWorker.Code;

namespace ThreadWorker
{
    /// <summary>
    /// A class for running task containers.
    /// </summary>
    public sealed class Worker
    {
        /// <summary>
        /// Called upon completion.
        /// </summary>
        public event EventHandler<WorkerArgs> Finish;
        /// <summary>
        /// Called before a new task. Gives similar information as Status.
        /// </summary>
        public event EventHandler<WorkerStatusArgs> Next;
        /// <summary>
        /// Called during the execution of a task, for information.
        /// </summary>
        public event EventHandler<WorkerStatusArgs> Status;
        /// <summary>
        /// Called when an error occurs in the thread.
        /// </summary>
        public event EventHandler<WorkerExceptionArgs> Exception;
        /// <summary>
        /// Called during a pause.
        /// </summary>
        public event EventHandler<WorkerArgs> Wait;
        /// <summary>
        /// Called at startup.
        /// </summary>
        public event EventHandler<WorkerArgs> Start;
        /// <summary>
        /// Called when stopped.
        /// </summary>
        public event EventHandler<WorkerArgs> Abort;

        /// <summary>
        /// Loops the thread and keeps it hanging. Through method PauseCycle.
        /// </summary>
        public bool Pause
        {
            get;
            set;
        }
        /// <summary>
        /// Returns the elapsed time in milliseconds.
        /// </summary>
        public TimeSpan Elapsed
        {
            get => TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);
        }
        /// <summary>
        /// Indicates if the thread is running.
        /// </summary>
        public bool IsRunning
        {
            get;
            private set;
        }
        /// <summary>
        /// Indicates if the thread has finished running.
        /// </summary>
        public bool Complete
        {
            get;
            private set;
        }
        /// <summary>
        /// Token for recording between tasks.
        /// </summary>
        public Token Token
        {
            get;
            set;
        }

        private readonly Stopwatch stopwatch;
        private WorkContainer currentContainer;
        private DateTime currentContainerDateTime;
        private readonly List<WorkContainer> containers;
        private Thread thread;
        private bool aborted;
        private int totalProgress;
        private int workProgress;
        private int taskIndex;

        /// <summary>
        /// Ctor
        /// </summary>
        public Worker()
        {
            taskIndex = 0;
            Token = new Token();
            containers = new List<WorkContainer>();
            stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="token">Token for recording between tasks.</param>
        public Worker(Token token)
            : this()
        {
            this.Token = token;
        }

        /// <summary>
        /// Pass containers with a name and a task.
        /// </summary>
        /// <param name="containers"></param>
        public void Works(params WorkContainer[] containers)
        {
            this.containers.AddRange(containers);
        }

        /// <summary>
        /// Starts container execution.
        /// </summary>
        public void Run()
        {
            taskIndex = 0;
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

        /// <summary>
        /// Stops the execution of containers.
        /// </summary>
        public void Stop()
        {
            aborted = true;
            if(thread.IsAlive)
                thread.Abort();
        }

        /// <summary>
        /// If the thread is running, you can wait for it to complete. Thread.Join()
        /// </summary>
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
                    TaskProgress = workProgress,
                    Title = currentContainer.Title,
                    Started = currentContainerDateTime,
                    Token = Token
                });
            while (Pause) ;
            if (inPause)
                Wait?.Invoke(this, new WorkerWaitArgs
                {
                    TotalProgress = totalProgress,
                    TaskProgress = workProgress,
                    Title = currentContainer.Title,
                    Started = currentContainerDateTime,
                    Token = Token
                });
            stopwatch.Start();
        }

        internal void ChangeProgress(double progress)
        {
            int total = (int)(100 * ((float)taskIndex / containers.Count));
            totalProgress = total + (int)(100 * (progress * (1f / containers.Count)));
            workProgress = (int)(100 * progress);
            Status?.Invoke(this, new WorkerStatusArgs
            {
                TotalProgress = totalProgress,
                TaskProgress = workProgress,
                Title = currentContainer.Title,
                Started = currentContainerDateTime,
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
                        taskIndex = count;
                        currentContainerDateTime = DateTime.Now;
                        totalProgress = (int)(100 * ((float)taskIndex / containers.Count));
                        workProgress = 0;
                        Next?.Invoke(this, new WorkerStatusArgs
                        {
                            Title = container.Title,
                            Started = currentContainerDateTime,
                            TotalProgress = totalProgress,
                            TaskProgress = workProgress,
                            Token = Token
                        });
                        Status?.Invoke(this, new WorkerStatusArgs
                        {
                            Title = container.Title,
                            Started = currentContainerDateTime,
                            TotalProgress = totalProgress,
                            TaskProgress = workProgress,
                            Token = Token
                        });
                        PauseCycle();
                        container.Action?.Invoke(new WorkManager(this), new TokenManager(Token));

                        totalProgress = (int)(100 * ((float)(taskIndex + 1) / containers.Count));
                        workProgress = 100;
                        Status?.Invoke(this, new WorkerStatusArgs
                        {
                            Title = container.Title,
                            Started = DateTime.Now,
                            TotalProgress = totalProgress,
                            TaskProgress = workProgress,
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
                            $"Exception at token index:{taskIndex}",ex),
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
