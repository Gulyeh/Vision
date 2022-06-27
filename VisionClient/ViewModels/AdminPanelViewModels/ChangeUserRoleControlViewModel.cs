using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class ChangeUserRoleControlViewModel : BindableBase, IActiveAware
    {
        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private bool isButtonEnabled = true;
        public bool IsButtonEnabled
        {
            get { return isButtonEnabled; }
            set { SetProperty(ref isButtonEnabled, value); }
        }

        private bool isActive;
        public bool IsActive 
        { 
            get => isActive;
            set
            {
                SetProperty(ref isActive, value, RaiseIsActiveChanged);
                if (isActive) GetRoles();
            }
        }

        private Guid UserId { get; set; }

        private readonly IAccountRepository accountRepository;
        public event EventHandler? IsActiveChanged;
        protected virtual void RaiseIsActiveChanged() => IsActiveChanged?.Invoke(this, EventArgs.Empty);

        public ObservableCollection<string> UserRoles { get; set; } = new();
        public DelegateCommand<string> ExecuteCommand { get; }

        public ChangeUserRoleControlViewModel(IEventAggregator eventAggregator, ITextEventHelper openedControl, IAccountRepository accountRepository)
        {
            ExecuteCommand = new DelegateCommand<string>(ChangeRole);
            this.accountRepository = accountRepository;

            eventAggregator.GetEvent<SendEvent<(DetailedUserModel, string)>>().Subscribe(x =>
            {
                UserId = x.Item1.UserId;
            }, ThreadOption.PublisherThread, false, x => x.Item2.Equals("ChangeUserRole"));

            eventAggregator.GetEvent<SendEvent<DetailedUserModel>>().Subscribe(x =>
            {
                ErrorText = string.Empty;
                UserId = x.UserId;
            });

            openedControl.Notify("ChangeUserRole");
        }

        private async void GetRoles()
        {
            ErrorText = string.Empty;
            IsButtonEnabled = false;
            try
            {
                UserRoles.Clear();
                var list = await accountRepository.GetRoles();
                UserRoles.AddRange(list);

                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                ErrorText = "Could not obtain available roles";
                IsButtonEnabled = true;
            }
        }

        private async void ChangeRole(string role)
        {
            ErrorText = string.Empty;
            if (string.IsNullOrWhiteSpace(role) || UserId == Guid.Empty) return;
            IsButtonEnabled = false;

            try
            {
                ErrorText = await accountRepository.ChangeUserRole(UserId, role);
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }
    }
}
