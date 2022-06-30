using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace VisionClient.ViewModels
{
    internal class SettingsControlViewModel : BindableBase
    {
        public DelegateCommand<string> SettingsContentCommand { get; }
        public DelegateCommand<string> ButtonPressedCommand { get; }
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;

        public SettingsControlViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            SettingsContentCommand = new DelegateCommand<string>(SelectContent);
            ButtonPressedCommand = new DelegateCommand<string>(ShowDialog);
            regionManager.RegisterViewWithRegion("SettingsRegion", "ProfileControl");
            this.regionManager = regionManager;
            this.dialogService = dialogService;
        }

        private void ShowDialog(string parameter)
        {
            dialogService.ShowDialog(parameter);
        }

        private void SelectContent(string name)
        {
            regionManager.RequestNavigate("SettingsRegion", name);
        }
    }
}
