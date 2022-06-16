using Prism.Events;
using Prism.Services.Dialogs;
using System;
using VisionClient.Core.Enums;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Helpers;
using VisionClient.SignalR;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class ApplyCouponControlViewModel : DialogHelper
    {
        private string couponCode = string.Empty;
        public string CouponCode
        {
            get { return couponCode; }
            set { SetProperty(ref couponCode, value); }
        }

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private bool isEnabledButton = true;
        public bool IsEnabledButton
        {
            get { return isEnabledButton; }
            set { SetProperty(ref isEnabledButton, value); }
        }

        private CodeTypes CodeType { get; set; }
        private readonly ICouponRepository couponRepository;
        private readonly IUsersService_Hubs usersService_Hubs;

        public ApplyCouponControlViewModel(IEventAggregator eventAggregator, ICouponRepository couponRepository, IUsersService_Hubs usersService_Hubs) : base(eventAggregator)
        {
            this.couponRepository = couponRepository;
            this.usersService_Hubs = usersService_Hubs;
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            CodeType = parameters.GetValue<CodeTypes>("CodeType");
            usersService_Hubs.CouponTextEvent += CouponEventReceiver;
        }

        public override void OnDialogClosed()
        {
            base.OnDialogClosed();
            usersService_Hubs.CouponTextEvent -= CouponEventReceiver;
        }

        private void CouponEventReceiver(object? sender, CouponTextEventArgs e)
        {
            IsEnabledButton = true;
            if (!string.IsNullOrWhiteSpace(e.Text)) ErrorText = e.Text;
        }

        protected override async void Execute(object? data)
        {
            IsEnabledButton = false;
            ErrorText = string.Empty;

            try
            {
                var response = await couponRepository.ApplyCoupon(CouponCode, CodeType);
                if (!string.IsNullOrWhiteSpace(response))
                {
                    ErrorText = response;
                    IsEnabledButton = true;
                }
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                IsEnabledButton = true;
            }
        }
    }
}
