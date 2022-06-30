using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class AddCouponControlViewModel : BindableBase
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

        private AddCouponDto couponModel = new();
        public AddCouponDto CouponModel
        {
            get { return couponModel; }
            set { SetProperty(ref couponModel, value); }
        }

        private GameModel selectedGame = new();
        public GameModel SelectedGame
        {
            get { return selectedGame; }
            set
            {
                if (value is null)
                {
                    SetProperty(ref selectedGame, new());
                    return;
                }

                if (selectedGame != value && CouponModel.CodeType.Equals("Package")
                    && value.Id != Guid.Empty) GetGamePackages(value.Id);

                if (CouponModel.CodeType.Equals("Package")) CouponModel.GameId = value.Id;
                else if (CouponModel.CodeType.Equals("Game")) CouponModel.CodeValue = value.Id.ToString();

                SetProperty(ref selectedGame, value);
            }
        }

        private ProductsModel selectedPackage = new();
        public ProductsModel SelectedPackage
        {
            get { return selectedPackage; }
            set
            {
                if (value is null)
                {
                    SetProperty(ref selectedPackage, new());
                    return;
                }

                CouponModel.CodeValue = value.Id.ToString();
                SetProperty(ref selectedPackage, value);
            }
        }

        private Visibility loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set { SetProperty(ref loadingVisibility, value); }
        }

        public DelegateCommand ExecuteCommand { get; }
        public ObservableCollection<ProductsModel> ProductsList { get; set; } = new();
        public IStaticData StaticData { get; }

        private readonly IGamesRepository gamesRepository;
        private readonly ICouponRepository couponRepository;

        public AddCouponControlViewModel(IGamesRepository gamesRepository, IStaticData staticData, ICouponRepository couponRepository)
        {
            ExecuteCommand = new DelegateCommand(Execute);
            this.gamesRepository = gamesRepository;
            StaticData = staticData;
            this.couponRepository = couponRepository;
        }

        private async void GetGamePackages(Guid Id)
        {
            ErrorText = string.Empty;
            LoadingVisibility = Visibility.Visible;
            try
            {
                ProductsList.Clear();
                var list = await gamesRepository.GetProducts(Id);
                foreach (var product in list.Products) ProductsList.Add(product);
                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ErrorText = "Could not get game packages";
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private async void Execute()
        {
            ErrorText = string.Empty;
            if (!CouponModel.Validator())
            {
                ErrorText = "Please fill all data";
                return;
            }
            IsButtonEnabled = false;

            try
            {
                ErrorText = await couponRepository.AddCoupon(CouponModel);
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                IsButtonEnabled = true;
                ErrorText = "Something went wrong";
            }
        }

    }
}
