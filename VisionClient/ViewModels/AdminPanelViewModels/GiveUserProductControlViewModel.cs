using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class GiveUserProductControlViewModel : BindableBase
    {
        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private bool isButtonEnabled = true;
        public bool IsButtonEnabled
        {
            get { return isButtonEnabled; }
            set { SetProperty(ref isButtonEnabled, value); }
        }

        private GiveProductDto productModel = new();
        public GiveProductDto ProductModel
        {
            get { return productModel; }
            set { SetProperty(ref productModel, value); }
        }

        private readonly IDialogService dialogService;
        private readonly IGamesRepository gamesRepository;

        public ObservableCollection<ProductsModel> GameProductsList { get; set; } = new();
        public DelegateCommand ExecuteCommand { get; }
        public DelegateCommand GetGameProductsCommand { get; }
        public IStaticData StaticData { get; }

        public GiveUserProductControlViewModel(IEventAggregator eventAggregator, ITextEventHelper textEventHelper, IDialogService dialogService, IGamesRepository gamesRepository, IStaticData staticData)
        {
            ExecuteCommand = new DelegateCommand(GiveUserProduct);
            GetGameProductsCommand = new DelegateCommand(GetGameProducts);
            this.dialogService = dialogService;
            this.gamesRepository = gamesRepository;
            StaticData = staticData;
            eventAggregator.GetEvent<SendEvent<(DetailedUserModel, string)>>().Subscribe(x =>
            {
                ProductModel.UserId = x.Item1.UserId;
            }, ThreadOption.PublisherThread, false, x => x.Item2.Equals("GiveUserProduct"));

            eventAggregator.GetEvent<SendEvent<DetailedUserModel>>().Subscribe(x =>
            {
                GameProductsList.Clear();
                ErrorText = string.Empty;
                ProductModel = new();

                ProductModel.UserId = x.UserId;
            });

            textEventHelper.Notify("GiveUserProduct");
        }

        private async void GetGameProducts()
        {
            ErrorText = string.Empty;
            if (ProductModel.GameId == Guid.Empty) return;
            IsButtonEnabled = false;

            try
            {
                ProductModel.ProductId = Guid.Empty;
                GameProductsList.Clear();
                var list = await gamesRepository.GetProducts(ProductModel.GameId);
                if (list.Products.Any()) GameProductsList.AddRange(list.Products);

                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }

        private async void GiveUserProduct()
        {
            ErrorText = string.Empty;
            if (!ProductModel.Validation() || !OpenDialog("Give access", "Are you sure, you want to give user access to this product?")) return;
            IsButtonEnabled = false;

            try
            {
                ErrorText = await gamesRepository.GiveUserProduct(ProductModel);
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }

        private bool OpenDialog(string title, string message)
        {
            bool result = false;
            dialogService.ShowDialog("ConfirmControl", new DialogParameters
            {
                { "title", title },
                { "message", message }
            }, x =>
            {
                result = x.Result switch
                {
                    ButtonResult.OK => true,
                    ButtonResult.Cancel => false,
                    _ => false,
                };
            });

            return result;
        }
    }
}
