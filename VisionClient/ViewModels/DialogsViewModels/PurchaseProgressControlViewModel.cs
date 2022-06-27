using Prism.Events;
using Prism.Services.Dialogs;
using System.Threading.Tasks;
using System.Windows;
using VisionClient.Core.Events;
using VisionClient.Helpers;
using VisionClient.SignalR;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class PurchaseProgressControlViewModel : DialogHelper
    {
        private string progressText = "Generating payment data...";
        public string ProgressText
        {
            get { return progressText; }
            set { SetProperty(ref progressText, value); }
        }

        private Visibility buttonVisibility = Visibility.Collapsed;
        public Visibility ButtonVisibility
        {
            get => buttonVisibility;
            set => SetProperty(ref buttonVisibility, value);
        }

        private readonly IOrderService_Hubs orderService_Hubs;

        public PurchaseProgressControlViewModel(IEventAggregator eventAggregator, IOrderService_Hubs orderService_Hubs) : base(eventAggregator)
        {
            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                ProgressText = "Something failed.\nIf you payed for a product and did not get it, please contact support.";
                ButtonVisibility = Visibility.Visible;
            }, ThreadOption.PublisherThread, false, x => x.Equals("PaymentFailed"));

            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                ProgressText = "Waiting for payment...";
            }, ThreadOption.PublisherThread, false, x => x.Equals("PaymentUrl"));

            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(async x =>
            {
                ProgressText = "Payment completed!\nProduct has been added to your account";
                await Task.Delay(3000);
                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }, ThreadOption.PublisherThread, false, x => x.Equals("PaymentDone"));

            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                ProgressText = "Connection to order server has been closed.\nYour session is still in progress but will require to log in again after purchase.";
                ButtonVisibility = Visibility.Visible;
            }, ThreadOption.PublisherThread, false, x => x.Equals("ConnectionClosed"));
            this.orderService_Hubs = orderService_Hubs;
        }

        public override async void OnDialogClosed()
        {
            base.OnDialogClosed();
            await orderService_Hubs.Disconnect();
        }

        protected override void Execute(object? data)
        {

        }
    }
}
