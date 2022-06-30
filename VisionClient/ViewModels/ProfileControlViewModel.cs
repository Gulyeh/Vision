using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using VisionClient.Core;

namespace VisionClient.ViewModels
{
    internal class ProfileControlViewModel : BindableBase
    {
        public DelegateCommand<string> SettingsCommand { get; }
        public IStaticData StaticData { get; }

        private readonly IDialogService dialogService;

        public ProfileControlViewModel(IDialogService dialogService, IStaticData staticData)
        {
            SettingsCommand = new DelegateCommand<string>(OpenSettings);
            this.dialogService = dialogService;
            StaticData = staticData;
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
                        { "content",  StaticData.UserData.Username }
                    }, null);
                    break;
                case "Description":
                    dialogService.ShowDialog("ChangeUserProfileControl", new DialogParameters()
                    {
                        { "message", "Description" },
                        { "title", "Change Description" },
                        { "content",  StaticData.UserData.Description}
                    }, null);
                    break;
                case "Avatar":
                    dialogService.ShowDialog("ChangePhotoControl", new DialogParameters()
                    {
                        { "title", "Change Avatar" },
                        { "ImageSource",  StaticData.UserData.PhotoUrl }
                    }, null);
                    break;
                default:
                    break;
            }
        }
    }
}
