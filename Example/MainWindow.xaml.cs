using System.Threading;
using System.Windows;
using ThreadWorker;
using ThreadWorker.Code;

namespace Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Worker worker;

        public MainWindow()
        {
            InitializeComponent();
            //Pass token in ctor to Worker if your work got exception.
            worker = new ThreadWorker.Worker();
            worker.Works(
                new WorkContainer("Job1", Job1),
                new WorkContainer("Job2", Job2),
                new WorkContainer("Job3", Job3));
            worker.Start += Worker_Start;
            worker.Next += Worker_Next;
            worker.Status += Worker_Status;
            worker.Wait += Worker_Wait;
            worker.Finish += Worker_Finish;
            worker.Exception += Worker_Exception;
            worker.Abort += Worker_Abort;
        }

        private void Worker_Abort(object sender, ThreadWorker.Arguments.WorkerArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                workButton.Content = "Work!";
            });
        }

        private void Worker_Exception(object sender, ThreadWorker.Arguments.WorkerExceptionArgs e)
        {
            //You can save you Token here! 
        }

        private void Worker_Finish(object sender, ThreadWorker.Arguments.WorkerArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                workButton.Content = "Work!";
            });
        }

        private void Worker_Wait(object sender, ThreadWorker.Arguments.WorkerArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (worker.Pause)
                    pauseButton.Content = "Pause...";
                else
                    pauseButton.Content = "Paused!";
            });
        }

        private void Worker_Status(object sender, ThreadWorker.Arguments.WorkerStatusArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                taskProgress.Content = $"{e.Title}:{e.TaskProgress}%";
                totalProgress.Content = $"Total:{e.TotalProgress}%";
                
                workProgress.Value = e.TotalProgress;
            });
        }

        private void Worker_Next(object sender, ThreadWorker.Arguments.WorkerStatusArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                taskProgress.Content = $"{e.Title}:{e.TaskProgress}%";
                totalProgress.Content = $"Total:{e.TotalProgress}%";
            });
        }

        private void Worker_Start(object sender, ThreadWorker.Arguments.WorkerArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                workButton.Content = "Working...";
            });
        }

        private void Job1(WorkManager workManager)
        {
            for (uint i = 0; i < 10; i++)
            {
                workManager.PauseCycle();
                workManager.ChangeProgress(i + 1, 10);
                Thread.Sleep(250);
            }

            Thread.Sleep(2000);
        }
        private void Job2(WorkManager workManager)
        {
            for (uint i = 0; i < 10; i++)
            {
                workManager.PauseCycle();
                workManager.ChangeProgress(i + 1, 10);
                Thread.Sleep(250);
            }
            Thread.Sleep(2000);
        }
        private void Job3(WorkManager workManager)
        {
            for (uint i = 0; i < 10; i++)
            {
                workManager.PauseCycle();
                workManager.ChangeProgress(i + 1, 10);
                Thread.Sleep(250);
            }
            Thread.Sleep(2000);
        }

        private void Work_Click(object sender, RoutedEventArgs e)
        {
            if (!worker.IsRunning)
                worker.Run();
            else
                worker.Stop();

            pauseButton.Content = "Pause...";
            workProgress.Value = 0;
        }
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            worker.Pause = !worker.Pause;
        }
    }
}
