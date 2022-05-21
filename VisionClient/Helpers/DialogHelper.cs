using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisionClient.Core.Events;

namespace VisionClient.Helpers
{
    internal abstract class DialogHelper : BindableBase, IDialogAware
    {
        private string? _message;
        public string? Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string? _title;
        public string? Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string? content;
        public string? Content
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }

        public event Action<IDialogResult>? RequestClose;
        public DelegateCommand<string> CloseDialogCommand { get; set; }
        public DelegateCommand<object?> ExecuteCommand { get; set; }
        private readonly IEventAggregator eventAggregator;

        public DialogHelper(IEventAggregator eventAggregator)
        {
            CloseDialogCommand = new DelegateCommand<string>(CloseDialog);
            ExecuteCommand = new DelegateCommand<object?>(Execute);
            this.eventAggregator = eventAggregator;
        }

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
                result = ButtonResult.OK;
            else if (parameter?.ToLower() == "false")
                result = ButtonResult.Cancel;

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
            eventAggregator.GetEvent<SendEvent<Visibility>>().Publish(Visibility.Hidden);
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            eventAggregator.GetEvent<SendEvent<Visibility>>().Publish(Visibility.Visible);
            Message = parameters.GetValue<string>("message");
            Title = parameters.GetValue<string>("title");
            Content = parameters.GetValue<string>("content");
        }

        public abstract void Execute(object? data);
    }
}
