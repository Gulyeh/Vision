using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Models;
using VisionClient.Helpers;
using VisionClient.SignalR;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class EditMessageControlViewModel : DialogHelper
    {
        private MessageModel editmessage = new();
        public MessageModel EditMessage
        {
            get { return editmessage; }
            set { SetProperty(ref editmessage, value); }
        }

        public DelegateCommand<AttachmentModel> DeletePhotoCommand { get; }
        public List<Guid> DeletedAttachmentIds { get; }
        private readonly IMessageService_Hubs messageService_Hubs;
        private readonly IStaticData StaticData;

        public EditMessageControlViewModel(IEventAggregator eventAggregator, IMessageService_Hubs messageService_Hubs, IStaticData staticData) : base(eventAggregator)
        {
            DeletePhotoCommand = new DelegateCommand<AttachmentModel>(DeleteAttachment);
            DeletedAttachmentIds = new List<Guid>();
            this.messageService_Hubs = messageService_Hubs;
            this.StaticData = staticData;
        }

        private void DeleteAttachment(AttachmentModel attachment)
        {
            DeletedAttachmentIds.Add(attachment.Id);
            EditMessage.Attachments.Remove(attachment);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            EditMessage = parameters.GetValue<MessageModel>("editmessage");
        }

        protected override async void Execute(object? data)
        {
            try
            {
                var message = new EditMessageDto(EditMessage.Id, EditMessage.Content, DeletedAttachmentIds, StaticData.ChatId);
                await messageService_Hubs.Send("EditMessage", message);
                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }
            catch (Exception)
            {

            }

        }
    }
}
