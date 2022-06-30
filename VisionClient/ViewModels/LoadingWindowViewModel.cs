using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisionClient.Core.Repository.IRepository;
using VisionClient.SignalR;

namespace VisionClient.ViewModels
{
    internal class LoadingWindowViewModel : BindableBase
    {
        private int loadingValue = 0;
        public int LoadingValue
        {
            get { return loadingValue; }
            set { SetProperty(ref loadingValue, value); }
        }

        private string progressText = string.Empty;
        public string ProgressText
        {
            get { return progressText; }
            set { SetProperty(ref progressText, value); }
        }

        private Visibility retryButtonVisibility = Visibility.Collapsed;
        public Visibility RetryButtonVisibility
        {
            get { return retryButtonVisibility; }
            set { SetProperty(ref retryButtonVisibility, value); }
        }

        private int InterruptedDownload = 0;
        public Window? TempWindow { get; set; } = null;
        public DelegateCommand RetryCommand { get; }
        public DelegateCommand ExitCommand { get; }
        private readonly IGamesRepository gamesRepository;
        private readonly IUsersService_Hubs usersService_Hubs;

        public LoadingWindowViewModel(IGamesRepository gamesRepository, IUsersService_Hubs usersService_Hubs)
        {
            this.gamesRepository = gamesRepository;
            this.usersService_Hubs = usersService_Hubs;
            RetryCommand = new DelegateCommand(RetryDownload);
            ExitCommand = new DelegateCommand(() => TempWindow?.Close());
            Initialization();
        }

        private void RetryDownload()
        {
            RetryButtonVisibility = Visibility.Collapsed;
            Initialization();
        }

        private async void Initialization()
        {
            if (InterruptedDownload == 3)
            {
                InterruptedDownload = 0;
                LoadingValue = 0;
                RetryButtonVisibility = Visibility.Visible;
                ProgressText = "Could not download data.\nPlease retry or exit";
                return;
            }

            try
            {
                ProgressText = "Getting user data...";
                await usersService_Hubs.CreateUsersConnection();
                await Task.Delay(1000);
                LoadingValue = 25;

                ProgressText = "Getting games data...";
                await gamesRepository.GetGames();
                await Task.Delay(1000);
                LoadingValue = 50;

                ProgressText = "Getting friends data...";
                await usersService_Hubs.CreateFriendsConnection();
                await Task.Delay(1000);
                LoadingValue = 75;

                ProgressText = "Loading interface...";
                var bs = new Bootstrapper();
                LoadingValue = 100;
                await Task.Delay(1000);

                bs.Run();
                TempWindow?.Close();
            }
            catch (Exception)
            {
                InterruptedDownload++;
                LoadingValue = 0;
                Initialization();
            }
        }
    }
}
