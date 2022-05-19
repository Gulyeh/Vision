using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VisionClient.Helpers;
using VisionClient.Views;
using VisionClient.Views.Login.Dialogs;

namespace VisionClient.ViewModels
{
    internal class LoginControlViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;

        public DelegateCommand RegisterCommand { get; set; }
        public DelegateCommand<UserControl> LoginCommand { get; set; }
        public LoginControlViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            RegisterCommand = new DelegateCommand(SwitchToRegister);
            LoginCommand = new DelegateCommand<UserControl>(ExecuteLogin);
        }

        private void ExecuteLogin(UserControl loginControl)
        {
            var bs = new LoadingBootstrapper();
            bs.Run();
            CloseParentWindowHelper.Close(loginControl as UserControl);
            //dialogService.ShowDialog(nameof(TFAControl));
        }

        private void SwitchToRegister()
        {
            regionManager.RequestNavigate("LoginContent", "RegisterControl");
        }
    }
}
