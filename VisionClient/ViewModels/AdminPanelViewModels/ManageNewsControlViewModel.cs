using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class ManageNewsControlViewModel : BindableBase
    {
        private Guid selectedGameId;
        public Guid SelectedGameId
        {
            get { return selectedGameId; }
            set 
            {
                SetProperty(ref selectedGameId, value);
                if (value != Guid.Empty) GetNewsFromPage();
            }
        }

        private int pageNumber;
        public int PageNumber
        {
            get { return pageNumber; }
            set { SetProperty(ref pageNumber, value); }
        }

        private int maxPages;
        public int MaxPages
        {
            get { return maxPages; }
            set { SetProperty(ref maxPages, value); }
        }

        private Visibility loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set { SetProperty(ref loadingVisibility, value); }
        }

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private readonly IGamesRepository gamesRepository;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;

        public DelegateCommand PageChangingCommand { get; }
        public DelegateCommand<NewsModel> EditNewsCommand { get; }
        public DelegateCommand<Guid?> DeleteNewsCommand { get; }
        public ObservableCollection<NewsModel> NewsList { get; set; }
        public IStaticData StaticData { get; }

        public ManageNewsControlViewModel(IStaticData staticData, IGamesRepository gamesRepository, IEventAggregator eventAggregator, IRegionManager regionManager,
            IDialogService dialogService)
        {
            NewsList = new();
            PageChangingCommand = new DelegateCommand(GetNewsFromPage);
            EditNewsCommand = new DelegateCommand<NewsModel>(EditNews);
            DeleteNewsCommand = new DelegateCommand<Guid?>(DeleteNews);
            StaticData = staticData;
            this.gamesRepository = gamesRepository;
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                GetNewsFromPage();
            }, ThreadOption.PublisherThread, false, x => x.Equals("NewsUpdated"));
        }
 
        private void EditNews(NewsModel model)
        {
            regionManager.RequestNavigate("AdminPanelRegion", "EditNewsControl");
            eventAggregator.GetEvent<SendEvent<NewsModel>>().Publish(model);
        }

        private async void DeleteNews(Guid? newsId)
        {
            if (newsId == Guid.Empty || newsId is null || SelectedGameId == Guid.Empty) return;
            
            bool result = false;
            dialogService.ShowDialog("ConfirmControl", new DialogParameters
            {
                { "title", "Confirm Delete" },
                { "message", "Are you sure, you want delete this news?" }
            }, x =>
            {
                result = x.Result switch
                {
                    ButtonResult.OK => true,
                    ButtonResult.Cancel => false,
                    _ => false,
                };
            });

            if (!result) return;

            ErrorText = string.Empty;
            LoadingVisibility = Visibility.Visible;
            try
            {
                (bool success, ErrorText) = await gamesRepository.DeleteNews(SelectedGameId, (Guid)newsId);
                if (success) NewsList.Remove(NewsList.First(x => x.Id == newsId));
                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                LoadingVisibility = Visibility.Collapsed;
            }          
        }

        private async void GetNewsFromPage()
        {
            ErrorText = string.Empty;
            LoadingVisibility = Visibility.Visible;
            try
            {
                var pagedNews = await gamesRepository.GetPagedNews(SelectedGameId, PageNumber);
                if(pagedNews.NewsList.Any())
                {
                    NewsList.Clear();
                    foreach(var item in pagedNews.NewsList) NewsList.Add(item);
                    MaxPages = pagedNews.MaxPages;
                }
                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                LoadingVisibility = Visibility.Collapsed;
            }
        }
    }
}
