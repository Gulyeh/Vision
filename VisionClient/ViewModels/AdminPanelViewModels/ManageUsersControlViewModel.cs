using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class ManageUsersControlViewModel : BindableBase
    {
        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private string userId = string.Empty;
        public string UserId
        {
            get { return userId; }
            set { SetProperty(ref userId, value); }
        }

        private Visibility loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set { SetProperty(ref loadingVisibility, value); }
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly IUsersRepository usersRepository;

        private DetailedUserModel TempUser { get; set; } = new();
        public ObservableCollection<DetailedUserModel> UsersList { get; set; } = new();
        public DelegateCommand<DetailedUserModel> ShowDetailsCommand { get; }
        public DelegateCommand GetUsersCommand { get; }

        public ManageUsersControlViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IUsersRepository usersRepository, ITextEventHelper panelOpened)
        {
            ShowDetailsCommand = new DelegateCommand<DetailedUserModel>(ShowDetails);
            GetUsersCommand = new DelegateCommand(GetUsers);

            panelOpened.NotifyOpened += (s, e) =>
            {
                eventAggregator.GetEvent<SendEvent<(DetailedUserModel, string)>>().Publish((TempUser, e.Text));
            };

            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.usersRepository = usersRepository;
        }

        private void ShowDetails(DetailedUserModel data)
        {
            regionManager.RequestNavigate("AdminPanelRegion", "EditUsersControl");
            TempUser = data;
            eventAggregator.GetEvent<SendEvent<DetailedUserModel>>().Publish(data);
        }

        private async void GetUsers()
        {
            ErrorText = string.Empty;
            if (string.IsNullOrWhiteSpace(UserId)) return;
            LoadingVisibility = Visibility.Visible;

            try
            {
                UsersList.Clear();
                var list = await usersRepository.GetDetailedUsers(UserId);
                UsersList.AddRange(list);
                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                LoadingVisibility = Visibility.Collapsed;
                ErrorText = "Something went wrong";
            }
        }
    }
}
