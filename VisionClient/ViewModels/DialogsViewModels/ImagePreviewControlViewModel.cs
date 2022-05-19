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
    internal class ImagePreviewControlViewModel : DialogHelper
    {
        private AttachmentModel attachment = new();
        public AttachmentModel Attachment
        {
            get { return attachment; }
            set { SetProperty(ref attachment, value); }
        }

        public ImagePreviewControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            Attachment = parameters.GetValue<AttachmentModel>("attachment");
        }

        public override void Execute(object? data)
        {
        
        }
    }
}
