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
using VisionClient.Core.Events;
using VisionClient.Core.Models;

namespace VisionClient.ViewModels
{
    internal class MessageControlViewModel : BindableBase
    {
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
            set { SetProperty(ref selecteduser, value); }
        }

        private AttachmentModel? selectedattachment;
        public AttachmentModel? SelectedAttachment
        {
            get { return selectedattachment; }
            set 
            { 

                SetProperty(ref selectedattachment, value);
                if(value is not null) ShowImagePreview(value);
            }
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;

        public DelegateCommand BackwardCommand { get; set; }
        public DelegateCommand<MessageModel> EditMessageCommand { get; set; }
        public DelegateCommand<MessageModel> DeleteMessageCommand { get; set; }
        public ObservableCollection<MessageModel> MessagesList { get; set; }
        public ObservableCollection<AttachmentModel> NewMessageAttachments { get; set; }

        public MessageControlViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IDialogService dialogService)
        {
            MessagesList = new ObservableCollection<MessageModel>();
            NewMessageAttachments = new ObservableCollection<AttachmentModel>();
            EditMessageCommand = new DelegateCommand<MessageModel>(EditMessage);
            DeleteMessageCommand = new DelegateCommand<MessageModel>(DeleteMessage);
            BackwardCommand = new DelegateCommand(navigateToGames);

            eventAggregator.GetEvent<SendEvent<UserModel>>().Subscribe(x => {
                SelectedUser = x;
                MessageContent = string.Empty;
            });

            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.dialogService = dialogService;


            //TEST
            var message = new MessageModel()
            {
                Content = "ASDJASKDAJDKASJDSAKDJASKDJSAKDADKADAK",
                DateSent = DateTime.Now,
                Id = 1,
                IsEdited = true,
                User = new UserModel()
                {
                    PhotoUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460__340.png",
                    Id = new Guid()
                },
                Attachments = new ObservableCollection<AttachmentModel>()
                {
                    new AttachmentModel()
                    {
                        Id = 1,
                        PhotoUrl = "https://media.istockphoto.com/photos/extra-wide-evening-panorama-of-business-miami-skyline-picture-id1058108750?b=1&k=20&m=1058108750&s=170667a&w=0&h=YzDzPZd4CXZe1-G_cjL2WIylKsvK2JjgS9MGdQ2uy-Q="
                    },
                    new AttachmentModel()
                    {
                        Id = 2,
                        PhotoUrl = "https://a.allegroimg.com/original/11fa09/0c2ab1a3471fb19d8a5693dc804a/BUTY-PILKARSKIE-SPORTOWE-LANKI-KORKI"
                    }
                }
            };
            var message1 = new MessageModel()
            {
                Content = "asvdaavsdavKDJSAKDADKADAK",
                DateSent = DateTime.Now,
                Id = 1,
                IsEdited = false,
                User = new UserModel()
                {
                    PhotoUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460__340.png",
                    Id = SelectedUser.Id
                },
                Attachments = new ObservableCollection<AttachmentModel>()
                {
                    new AttachmentModel()
                    {
                        Id = 1,
                        PhotoUrl = "https://a.allegroimg.com/original/11fa09/0c2ab1a3471fb19d8a5693dc804a/BUTY-PILKARSKIE-SPORTOWE-LANKI-KORKI"
                    },
                    new AttachmentModel()
                    {
                        Id = 2,
                        PhotoUrl = "https://a.allegroimg.com/original/11fa09/0c2ab1a3471fb19d8a5693dc804a/BUTY-PILKARSKIE-SPORTOWE-LANKI-KORKI"
                    },
                    new AttachmentModel()
                    {
                        Id = 3,
                        PhotoUrl = "https://a.allegroimg.com/original/11fa09/0c2ab1a3471fb19d8a5693dc804a/BUTY-PILKARSKIE-SPORTOWE-LANKI-KORKI"
                    }
                }
            };
            MessagesList.Add(message);
            MessagesList.Add(message1);
            MessagesList.Add(message);
            MessagesList.Add(message1);
            MessagesList.Add(message);
            MessagesList.Add(message1);
            MessagesList.Add(message);
            MessagesList.Add(message1);
            MessagesList.Add(message);
            MessagesList.Add(message1);
            MessagesList.Add(message);
            MessagesList.Add(message1);
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
            if (message is null) return;

            var title = "Delete Message";
            var dialogMessage = "Do you want to delete this message?\n(For you only)";
            dialogService.ShowDialog("ConfirmControl", new DialogParameters
            {
                { "message", dialogMessage },
                { "title", title }
            }, null);
        }

        private void EditMessage(MessageModel message)
        {
            if (message is null) return;
            dialogService.ShowDialog("EditMessageControl", new DialogParameters
            {
                { "editmessage", message.Clone() }
            }, null);
        }


        private void navigateToGames()
        {
            eventAggregator.GetEvent<SendEvent<string>>().Publish("StopFocus");
            regionManager.RequestNavigate("LibraryContentRegion", "GamesControl");
        }
    }
}
