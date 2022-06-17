using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;
using VisionClient.Helpers;
using VisionClient.Utility;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class ChangePhotoControlViewModel : DialogHelper
    {
        private object imageSource = new();
        public object ImageSource
        {
            get { return imageSource; }
            set { SetProperty(ref imageSource, value); }
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

        public DelegateCommand SelectFileCommand { get; }
        private readonly IUsersRepository usersRepository;

        public ChangePhotoControlViewModel(IEventAggregator eventAggregator, IUsersRepository usersRepository) : base(eventAggregator)
        {
            SelectFileCommand = new DelegateCommand(SelectFile);
            this.usersRepository = usersRepository;
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            ImageSource = parameters.GetValue<string>("ImageSource");
        }

        private void SelectFile()
        {
            ImageSource = FileDialogHelper.OpenFile(false).First();
        }

        protected override async void Execute(object? data)
        {
            try
            {
                LoadingVisibility = Visibility.Visible;
                ErrorText = string.Empty;

                if (imageSource is not BitmapImage image)
                {
                    ErrorText = "Image format not supported";
                    return;
                }

                await usersRepository.ChangePhoto(image.GetBase64());
                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }
            catch (Exception e)
            {
                LoadingVisibility = Visibility.Collapsed;
                ErrorText = "Something went wrong";
            }
        }
    }
}
