using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Models;
using VisionClient.Helpers;

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

        private CoinPackage selectedCoin = new();
        public CoinPackage SelectedCoin
        {
            get { return selectedCoin; }
            set { SetProperty(ref selectedCoin, value); }
        }

        private string? code;
        public string? Code
        {
            get { return code; }
            set { SetProperty(ref code, value); }
        }

        public ObservableCollection<CoinPackage> CoinsList { get; set; }
        public ObservableCollection<PaymentMethod> PaymentMethodsList { get; set; }
        public BuyMoreControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            CoinsList = new ObservableCollection<CoinPackage>();
            PaymentMethodsList = new ObservableCollection<PaymentMethod>();
            var coin = new CoinPackage()
            {
                Id = 1,
                Title = "4000 Visions",
                Amount = 4000,
                Price = 15.99M,
                Discount = 50,
                IsAvailable = true
            };
            coin.Discounted();

            var payment = new PaymentMethod()
            {
                PaymentName = "Stripe",
                PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/ba/Stripe_Logo%2C_revised_2016.svg/2560px-Stripe_Logo%2C_revised_2016.svg.png",
                IsAvailable = true
            };

            PaymentMethodsList.Add(payment);

            CoinsList.Add(coin);
            CoinsList.Add(coin);
            CoinsList.Add(coin);
            CoinsList.Add(coin);
            CoinsList.Add(coin);
            CoinsList.Add(coin);
            CoinsList.Add(coin);
        }

        public override void Execute(object? data)
        {
        
        }
    }
}
