using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System.Windows;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models;
using VisionClient.Helpers;
using VisionClient.Utility;

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
        public DelegateCommand<string> ImageMenuCommand { get; }
        public ImagePreviewControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            ImageMenuCommand = new DelegateCommand<string>(ImageMenu);
        }

        private async void ImageMenu(string data)
        {
            switch (data)
            {
                case "CopyUrl":
                    Clipboard.SetText(Attachment.AttachmentUrl);
                    break;
                case "OpenUrl":
                    OpenBrowserHelper.OpenUrl(Attachment.AttachmentUrl);
                    break;
                case "SaveImage":
                    await FileDialogHelper.SaveFile(Attachment.AttachmentUrl, false);
                    break;
                case "CopyImage":
                    await FileDialogHelper.SaveFile(Attachment.AttachmentUrl, true);
                    break;
                default:
                    break;
            }
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            Attachment = parameters.GetValue<AttachmentModel>("attachment");
        }

        protected override void Execute(object? data)
        {

        }
    }
}
