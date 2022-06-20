using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Extensions;
using VisionClient.SignalR;
using VisionClient.Utility;

namespace VisionClient.ViewModels
{
    internal class MessageControlViewModel : BindableBase, IActiveAware
    {
        public event EventHandler? IsActiveChanged;

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                SetProperty(ref _isActive, value, RaiseIsActiveChanged);
                Connection(value);
                if (!value)
                {
                    ClearData();
                    eventAggregator.GetEvent<SendEvent<string>>().Publish("StopFocus");
                }
            }
        }

        protected virtual void RaiseIsActiveChanged() => IsActiveChanged?.Invoke(this, EventArgs.Empty);

        private string messageContent = string.Empty;
        public string MessageContent
        {
            get { return messageContent; }
            set { SetProperty(ref messageContent, value); }
        }

        private UserModel selecteduser = new();
        public UserModel SelectedUser
        {
            get { return selecteduser; }
            set
            {
                SetProperty(ref selecteduser, value);
                if (selecteduser.UserId == Guid.Empty) NavigateToGames();
            }
        }

        private AttachmentModel? selectedattachment;
        public AttachmentModel? SelectedAttachment
        {
            get { return selectedattachment; }
            set
            {
                SetProperty(ref selectedattachment, value);
                if (value is not null) ShowImagePreview(value);
            }
        }

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private Visibility loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set { SetProperty(ref loadingVisibility, value); }
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;
        private readonly IMessageService_Hubs messageService_Hubs;

        private int CurrentPage { get; set; }
        public IStaticData StaticData { get; }
        public DelegateCommand BackwardCommand { get; }
        public DelegateCommand AddAttachmentCommand { get; }
        public DelegateCommand<BitmapImage> RemoveAttachmentCommand { get; }
        public DelegateCommand<MessageModel> EditMessageCommand { get; }
        public DelegateCommand<MessageModel> DeleteMessageCommand { get; }
        public DelegateCommand SendMessageCommand { get; }
        public ObservableCollection<BitmapImage> NewMessageAttachments { get; }

        public MessageControlViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IDialogService dialogService,
            IMessageService_Hubs messageService_Hubs, IStaticData staticData)
        {
            NewMessageAttachments = new ObservableCollection<BitmapImage>();
            EditMessageCommand = new DelegateCommand<MessageModel>(EditMessage);
            DeleteMessageCommand = new DelegateCommand<MessageModel>(DeleteMessage);
            BackwardCommand = new DelegateCommand(NavigateToGames);
            AddAttachmentCommand = new DelegateCommand(AddAttachments);
            RemoveAttachmentCommand = new DelegateCommand<BitmapImage>(RemoveAttachment);
            SendMessageCommand = new DelegateCommand(SendMessage);

            eventAggregator.GetEvent<SendEvent<UserModel>>().Subscribe(x =>
            {
                SelectedUser = x;
            });

            eventAggregator.GetEvent<SendEvent<Guid>>().Subscribe(x =>
            {
                if (SelectedUser.UserId == x) NavigateToGames();
            });

            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            this.messageService_Hubs = messageService_Hubs;
            this.StaticData = staticData;
        }

        private void RemoveAttachment(BitmapImage image) => NewMessageAttachments.Remove(image);

        private void AddAttachments()
        {
            var images = FileDialogHelper.OpenFile(false);
            if ((NewMessageAttachments.Count + images.Count()) > 3) return;
            foreach (var image in images) NewMessageAttachments.Add(image);
        }

        private void ClearData()
        {
            CurrentPage = 1;
            StaticData.ChatId = Guid.Empty;
            SelectedUser = new();
            if (StaticData.Messages.Any()) StaticData.Messages.Clear();
            if (NewMessageAttachments.Any()) NewMessageAttachments.Clear();
            ErrorText = string.Empty;
            MessageContent = string.Empty;
        }

        private void ShowImagePreview(AttachmentModel model)
        {
            dialogService.ShowDialog("ImagePreviewControl", new DialogParameters
            {
                { "attachment", model }
            }, null);
        }

        private void DeleteMessage(MessageModel message)
        {
            try
            {
                ErrorText = string.Empty;
                if (message is null) return;

                var title = "Delete Message";
                var dialogMessage = "Do you want to delete this message?\n(For you only)";
                dialogService.ShowDialog("ConfirmControl", new DialogParameters
                {
                    { "message", dialogMessage },
                    { "title", title }
                }, async r =>
                {
                    if (r.Result == ButtonResult.OK)
                    {
                        await messageService_Hubs.Send("RemoveMessage", new DeleteMessageDto(message.Id, StaticData.ChatId));
                    }
                });
            }
            catch (Exception)
            {
                ErrorText = "Could not delete message";
            }
        }

        private void EditMessage(MessageModel message)
        {
            if (message is null) return;
            dialogService.ShowDialog("EditMessageControl", new DialogParameters
            {
                { "editmessage", message.Clone() }
            }, null);
        }

        private void NavigateToGames()
        {
            eventAggregator.GetEvent<SendEvent<string>>().Publish("StopFocus");
            regionManager.RequestNavigate("LibraryContentRegion", "GamesControl");
        }

        public async Task GetMoreMessages()
        {
            if (StaticData.MaxPages <= CurrentPage || LoadingVisibility == Visibility.Visible) return;
            LoadingVisibility = Visibility.Visible;
            var currentMessages = StaticData.Messages.Count;

            try
            {
                await Task.Delay(1000);
                await messageService_Hubs.Send("GetMoreMessages", new GetMoreMessagesDto(CurrentPage + 1, StaticData.ChatId));
                CurrentPage++;
                while (currentMessages == StaticData.Messages.Count) { await Task.Delay(100); }
                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        public async void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(MessageContent) && NewMessageAttachments.Count == 0) return;
            try
            {
                ErrorText = string.Empty;
                MessageDto message = new();
                message.Content = MessageContent;
                message.ChatId = StaticData.ChatId;

                if (NewMessageAttachments.Any())
                {
                    foreach (var image in NewMessageAttachments) message.AttachmentsList.Add(image.GetBase64());
                }

                await messageService_Hubs.Send("SendMessage", message);
                MessageContent = string.Empty;
                NewMessageAttachments.Clear();
            }
            catch (Exception)
            {
                ErrorText = "Could not send message";
            }
        }

        private async void Connection(bool connect)
        {
            if (SelectedUser.UserId == Guid.Empty) await Task.Delay(50);
            int attempts = 0;
            try
            {
                if (!connect) await messageService_Hubs.Disconnect();
                else
                {
                    LoadingVisibility = Visibility.Visible;
                    await messageService_Hubs.CreateHubConnection(SelectedUser.UserId);
                    while (StaticData.ChatId == Guid.Empty)
                    {
                        attempts++;
                        await Task.Delay(50);
                        if (attempts == 100) throw new Exception();
                    }
                    LoadingVisibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                LoadingVisibility = Visibility.Collapsed;
                ErrorText = "Something went wrong";
            }
        }
    }
}
