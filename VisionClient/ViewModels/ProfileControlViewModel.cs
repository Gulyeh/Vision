using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core;
using VisionClient.Core.Models;

namespace VisionClient.ViewModels
{
    internal class ProfileControlViewModel : BindableBase
    {
        private UserModel user = StaticData.UserData;
        public UserModel User
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }

        private string password = String.Empty;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        private string newEmailAddress = string.Empty;
        public string NewEmailAddress
        {
            get { return newEmailAddress; }
            set { SetProperty(ref newEmailAddress, value); }
        }

        public DelegateCommand SaveNewEmailCommand { get; set; }
        public DelegateCommand<string> SettingsCommand { get; set; }
        private readonly IDialogService dialogService;

        public ProfileControlViewModel(IDialogService dialogService)
        {
            SaveNewEmailCommand = new DelegateCommand(SaveNewEmail);
            SettingsCommand = new DelegateCommand<string>(OpenSettings);
            this.dialogService = dialogService;
        }

        private void OpenSettings(string parameter)
        {
            switch (parameter)
            {
                case "Username":
                    dialogService.ShowDialog("ChangeUserProfileControl", new DialogParameters()
                    {
                        { "message", "Username" },
                        { "title", "Change Username" },
                        { "content",  User.UserName }
                    }, null);
                    break;
                case "Description":
                    dialogService.ShowDialog("ChangeUserProfileControl", new DialogParameters()
                    {
                        { "message", "Description" },
                        { "title", "Change Description" },
                        { "content",  User.Description}
                    }, null);
                    break;
                case "Avatar":
                    dialogService.ShowDialog("ChangePhotoControl", new DialogParameters()
                    {
                        { "title", "Change Avatar" },
                        { "content",  User.PhotoUrl}
                    }, null);
                    break;
                default:
                    break;
            }
        }

        private void SaveNewEmail()
        {

        }
    }
}
