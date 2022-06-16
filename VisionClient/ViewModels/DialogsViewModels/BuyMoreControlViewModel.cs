using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using VisionClient.Core.Enums;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Helpers;
using VisionClient.SignalR;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class BuyMoreControlViewModel : DialogHelper
    {
        private PaymentMethod selectedPayment = new();
        public PaymentMethod SelectedPayment
        {
            get { return selectedPayment; }
            set { SetProperty(ref selectedPayment, value); }
        }

        private CoinPackageModel selectedCoin = new();
        public CoinPackageModel SelectedCoin
        {
            get { return selectedCoin; }
            set { SetProperty(ref selectedCoin, value); }
        }

        private string code = string.Empty;
        public string Code
        {
            get { return code; }
            set { SetProperty(ref code, value); }
        }

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private string? couponCodeVerified = null;
        public string? CouponCodeVerified
        {
            get => couponCodeVerified;
            set => SetProperty(ref couponCodeVerified, value);
        }

        public ObservableCollection<CoinPackageModel> CoinsList { get; }
        public ObservableCollection<PaymentMethod> PaymentMethodsList { get; }
        public DelegateCommand ApplyCodeCommand { get; }

        private readonly ICurrencyRepository currencyRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IOrderService_Hubs orderService_Hubs;
        private readonly IDialogService dialogService;
        private readonly ICouponRepository couponRepository;

        public BuyMoreControlViewModel(IEventAggregator eventAggregator, ICurrencyRepository currencyRepository,
            IPaymentRepository paymentRepository, IOrderService_Hubs orderService_Hubs, IDialogService dialogService, ICouponRepository couponRepository) : base(eventAggregator)
        {
            CoinsList = new ObservableCollection<CoinPackageModel>();
            PaymentMethodsList = new ObservableCollection<PaymentMethod>();
            ApplyCodeCommand = new DelegateCommand(CheckCoupon);
            this.currencyRepository = currencyRepository;
            this.paymentRepository = paymentRepository;
            this.orderService_Hubs = orderService_Hubs;
            this.dialogService = dialogService;
            this.couponRepository = couponRepository;
        }

        public async void GetPackages()
        {
            try
            {
                ErrorText = string.Empty;

                var packages = await currencyRepository.GetPackages();
                foreach (var package in packages) CoinsList.Add(package);

                var paymentMethods = await paymentRepository.GetPaymentMethods();
                foreach (var method in paymentMethods) PaymentMethodsList.Add(method);
            }
            catch (Exception)
            {
                ErrorText = "Error while loading data";
            }
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

                foreach (var item in CoinsList)
                {
                    if (couponData.Signature.Equals("Price")) item.Price -= int.Parse(couponData.CodeValue);
                    else if (couponData.Signature.Equals("Procentage")) item.Price -= item.Price * decimal.Parse(couponData.CodeValue) / 100;
                }

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

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            GetPackages();
        }

        protected override async void Execute(object? data)
        {
            try
            {
                await orderService_Hubs.CreateHubConnection(SelectedCoin.Id, SelectedPayment.Id, OrderType.Currency, Guid.Empty, CouponCodeVerified);
                dialogService.ShowDialog("PurchaseProgressControl", null, async r =>
                {
                    switch (r.Result)
                    {
                        case ButtonResult.OK:
                            await orderService_Hubs.Disconnect();
                            RaiseRequestClose(new DialogResult(ButtonResult.OK));
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
    }
}
