using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.ViewModels.DialogsViewModels;
using VisionClient.Views.Dialogs;

namespace VisionClient.Extensions
{
    internal static class RegisterDialogExtensions
    {
        public static void RegisterDialogs(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<DeleteAccountControl, DeleteAccountControlViewModel>();
            containerRegistry.RegisterDialog<LogoutControl, LogoutControlViewModel>();
            containerRegistry.RegisterDialog<ApplyCouponControl, ApplyCouponControlViewModel>();
            containerRegistry.RegisterDialog<ChangePhotoControl, ChangePhotoControlViewModel>();
            containerRegistry.RegisterDialog<ConfirmControl, ConfirmControlViewModel>();
            containerRegistry.RegisterDialog<ChangeUserProfileControl, ChangeUserProfileControlViewModel>();
            containerRegistry.RegisterDialog<EditMessageControl, EditMessageControlViewModel>();
            containerRegistry.RegisterDialog<ImagePreviewControl, ImagePreviewControlViewModel>();
            containerRegistry.RegisterDialog<BuyMoreControl, BuyMoreControlViewModel>();
        }
    }
}
