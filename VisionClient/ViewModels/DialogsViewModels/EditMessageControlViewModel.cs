using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Models;
using VisionClient.Helpers;

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

        public DelegateCommand<AttachmentModel> DeletePhotoCommand { get; set; }
        public EditMessageControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DeletePhotoCommand = new DelegateCommand<AttachmentModel>(DeleteAttachment);
        }

        private void DeleteAttachment(AttachmentModel attachment)
        {
            EditMessage.Attachments.Remove(attachment);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            EditMessage = parameters.GetValue<MessageModel>("editmessage");
        }

        public override void Execute(object? data)
        {
          
        }
    }
}
