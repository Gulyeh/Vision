using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Events;
using VisionClient.Core.Helpers.Aggregators;
using VisionClient.Core.Models;

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

        private readonly IRegionManager regionManager;
        public ObservableCollection<PaymentMethod> PaymentMethodsList { get; set; }
        public DelegateCommand GoBackCommand { get; set; }

        public PurchaseControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<SendEvent<GameDetailsToPurchase>>().Subscribe(x =>
            {

            });

            PaymentMethodsList = new ObservableCollection<PaymentMethod>();
            GoBackCommand = new DelegateCommand(GoBackward);
            this.regionManager = regionManager;

            var product1 = new PurchaseModel()
            {
                Id = 1,
                Title = "Swords of Legends Online",
                Discount = 50,
                Price = 50,
                PhotoUrl = "https://mmos.com/wp-content/uploads/2021/04/swords-of-legends-online-character-summoner-banner.jpg",
                Details = new List<string>
                {
                    "Golden Wings",
                    "Trust Mode",
                    "50000 Gold",
                    "20000 Silver",
                    "Flying Mount",
                    "Supreme Outfit"
                }
            };
            product1.CreateDiscountText();

            Product = product1;

            var payment = new PaymentMethod()
            {
                PaymentName = "Stripe",
                PhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/ba/Stripe_Logo%2C_revised_2016.svg/2560px-Stripe_Logo%2C_revised_2016.svg.png",
                IsAvailable = true
            };

            PaymentMethodsList.Add(payment);
        }

        private void GoBackward()
        {
            
            regionManager.RequestNavigate("LibraryContentRegion", "GamesControl");
        }

    }
}
