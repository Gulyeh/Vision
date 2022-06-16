using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VisionClient.Core.Enums;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.SignalR;

namespace VisionClient.ViewModels
{
    internal class PurchaseControlViewModel : BindableBase
    {
        private PurchaseModel product = new();
        public PurchaseModel Product
        {
            get { return product; }
            set { SetProperty(ref product, value); }
        }

        private PaymentMethod selectedPayment = new();
        public PaymentMethod SelectedPayment
        {
            get { return selectedPayment; }
            set { SetProperty(ref selectedPayment, value); }
        }

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private string code = string.Empty;
        public string Code
        {
            get { return code; }
            set { SetProperty(ref code, value); }
        }

        private string? couponCodeVerified = null;
        public string? CouponCodeVerified
        {
            get => couponCodeVerified;
            set => SetProperty(ref couponCodeVerified, value);
        }

        private readonly IRegionManager regionManager;
        private readonly IPaymentRepository paymentRepository;
        private readonly IOrderService_Hubs orderService_Hubs;
        private readonly IDialogService dialogService;
        private readonly ICouponRepository couponRepository;

        public ObservableCollection<PaymentMethod> PaymentMethodsList { get; }
        public DelegateCommand GoBackCommand { get; }
        public DelegateCommand ApplyCodeCommand { get; }
        public DelegateCommand ExecutePayment { get; }


        public PurchaseControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,
            IPaymentRepository paymentRepository, IOrderService_Hubs orderService_Hubs, IDialogService dialogService, ICouponRepository couponRepository)
        {
            PaymentMethodsList = new ObservableCollection<PaymentMethod>();

            eventAggregator.GetEvent<SendEvent<PurchaseModel>>().Subscribe(async (x) =>
            {
                await GetPayments();
                Product = x;
            });

            GoBackCommand = new DelegateCommand(GoBackward);
            ApplyCodeCommand = new DelegateCommand(CheckCoupon);
            ExecutePayment = new DelegateCommand(Execute);
            this.regionManager = regionManager;
            this.paymentRepository = paymentRepository;
            this.orderService_Hubs = orderService_Hubs;
            this.dialogService = dialogService;
            this.couponRepository = couponRepository;
        }

        private async Task GetPayments()
        {
            PaymentMethodsList.Clear();
            var paymentMethods = await paymentRepository.GetPaymentMethods();
            foreach (var method in paymentMethods) PaymentMethodsList.Add(method);
        }

        private void GoBackward()
        {
            CouponCodeVerified = null;
            ErrorText = string.Empty;
            regionManager.RequestNavigate("LibraryContentRegion", "GamesControl");
        }

        private async void Execute()
        {
            try
            {
                await orderService_Hubs.CreateHubConnection(Product.Id, SelectedPayment.Id, Product.OrderType, Product.GameId, CouponCodeVerified);
                dialogService.ShowDialog("PurchaseProgressControl", null, async r =>
                {
                    switch (r.Result)
                    {
                        case ButtonResult.OK:
                            await orderService_Hubs.Disconnect();
                            GoBackward();
                            break;
                        case ButtonResult.Cancel:
                            await orderService_Hubs.Disconnect();
                            break;
                        default:
                            break;
                    }
                });
            }
            catch (Exception) { }
        }

        private async void CheckCoupon()
        {
            try
            {
                CouponCodeVerified = string.Empty;
                (var couponData, var error) = await couponRepository.VerifyCoupon(code, CodeTypes.Discount);
                if (!string.IsNullOrWhiteSpace(error) && string.IsNullOrWhiteSpace(couponData.Coupon))
                {
                    ErrorText = error;
                    CouponCodeVerified = null;
                    return;
                }

                if (couponData.Signature.Equals("Price")) Product.Price -= int.Parse(couponData.CodeValue);
                else if (couponData.Signature.Equals("Procentage")) Product.Price -= Product.Price * decimal.Parse(couponData.CodeValue) / 100;

                CouponCodeVerified = couponData.Coupon;
                var signature = couponData.Signature.Equals("Price") ? "$" : "%";
                ErrorText = $"{couponData.CodeValue}{signature} discount has been applied";
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                CouponCodeVerified = null;
            }
        }
    }
}
