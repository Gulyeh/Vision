using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VisionClient.Core.Dtos;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;
using VisionClient.Utility;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class EditNewsControlViewModel : BindableBase
    {
        private object coverImage = new();
        public object CoverImage
        {
            get { return coverImage; }
            set { SetProperty(ref coverImage, value); }
        }

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

        private EditNewsDto newsData = new();
        public EditNewsDto NewsData
        {
            get { return newsData; }
            set { SetProperty(ref newsData, value); }
        }

        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IGamesRepository gamesRepository;

        public DelegateCommand BackwardCommand { get; }
        public DelegateCommand OpenImageCommand { get; }
        public DelegateCommand ExecuteCommand { get; }
        public EditNewsControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IGamesRepository gamesRepository)
        {
            BackwardCommand = new DelegateCommand(NavigateToNews);
            OpenImageCommand = new DelegateCommand(OpenImage);
            ExecuteCommand = new DelegateCommand(Execute);

            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.gamesRepository = gamesRepository;
            eventAggregator.GetEvent<SendEvent<NewsModel>>().Subscribe(x =>
            {
                var newNews = new EditNewsDto()
                {
                    Title = x.Title,
                    CreatedDate = x.CreatedDate,
                    Content = x.Content,
                    Id = x.Id,
                    GameId = x.GameId
                };
                NewsData = newNews;
                CoverImage = x.PhotoUrl;
            });
        }

        private bool Validation() => !string.IsNullOrWhiteSpace(NewsData.Title) && !string.IsNullOrWhiteSpace(NewsData.Content);
        
        private async void Execute()
        {
            ErrorText = string.Empty;
            if(!Validation())
            {
                ErrorText = "Fill all data";
                return;
            }
            IsButtonEnabled = false;

            try
            { 
                if (CoverImage.GetType() == typeof(BitmapImage))
                {
                    BitmapImage bitmapImage = (BitmapImage)CoverImage;
                    NewsData.Photo = bitmapImage.GetBase64();
                }

                (bool success, ErrorText) = await gamesRepository.EditNews(NewsData);
                IsButtonEnabled = true;
                if (success)
                {
                    eventAggregator.GetEvent<SendEvent<string>>().Publish("NewsUpdated");
                    NavigateToNews();
                }
            }
            catch(Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }

        private void OpenImage()
        {
            var image = FileDialogHelper.OpenFile(false);
            if (image.Any()) CoverImage = image.First();
        }

        private void NavigateToNews() => regionManager.RequestNavigate("AdminPanelRegion", "ManageNewsControl");
    }
}
