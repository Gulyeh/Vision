using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisionClient.Core;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.SignalR;

namespace VisionClient.ViewModels
{
    internal class TransactionsHistoryControlViewModel : BindableBase
    {
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

        public DelegateCommand ExecuteCommand { get; }
        public DelegateCommand<string> OpenWebCommand { get; }
        public ObservableCollection<OrderModel> OrdersList { get; set; } = new();
        private readonly IOrderRepository orderRepository;

        public TransactionsHistoryControlViewModel(IOrderRepository orderRepository)
        {
            ExecuteCommand = new DelegateCommand(GetOrders);
            OpenWebCommand = new DelegateCommand<string>(OpenPaymentUrl);
            this.orderRepository = orderRepository;
        }

        private void OpenPaymentUrl(string url) => OpenBrowserHelper.OpenUrl(url);
             
        private async void GetOrders()
        {
            ErrorText = string.Empty;
            LoadingVisibility = Visibility.Visible;
            try
            {
                OrdersList.Clear();
                var orderList = await orderRepository.GetUserOrders();
                if (orderList.Any()) OrdersList.AddRange(orderList);

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
