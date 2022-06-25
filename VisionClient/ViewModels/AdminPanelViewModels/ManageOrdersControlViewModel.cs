using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class ManageOrdersControlViewModel : BindableBase
    {
        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private bool isButtonEnabled = true;
        public bool IsButtonEnabled
        {
            get { return isButtonEnabled; }
            set { SetProperty(ref isButtonEnabled, value); }
        }

        private Visibility loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set { SetProperty(ref loadingVisibility, value); }
        }

        private readonly IOrderRepository orderRepository;

        public DelegateCommand<Guid?> MakePaidCommand { get; }
        public DelegateCommand<string> GetOrdersCommand { get; }
        public ObservableCollection<OrderModel> OrderList { get; set; }
        public ManageOrdersControlViewModel(IOrderRepository orderRepository)
        {
            MakePaidCommand = new DelegateCommand<Guid?>(ChangeToPaid);
            GetOrdersCommand = new DelegateCommand<string>(GetOrders);
            OrderList = new();
            this.orderRepository = orderRepository;
        }

        private async void ChangeToPaid(Guid? orderId)
        {
            ErrorText = string.Empty;
            if (orderId is null || orderId == Guid.Empty) return;
            IsButtonEnabled = false;
            try
            {
                (bool success, ErrorText) = await orderRepository.ChangeOrderToPaid((Guid)orderId);
                if (success) OrderList.First(x => x.Id == orderId).Paid = true;
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                IsButtonEnabled = true;
                ErrorText = "Something went wrong";
            }
        }

        private async void GetOrders(string orderId)
        {
            ErrorText = string.Empty;
            if (string.IsNullOrWhiteSpace(orderId)) return;
            LoadingVisibility = Visibility.Visible;

            try
            {
                OrderList.Clear();
                var list = await orderRepository.GetOrders(orderId);
                foreach (var item in list) OrderList.Add(item);
                ErrorText = $"Found {list.Count} record(s)";
                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                LoadingVisibility = Visibility.Collapsed;
                ErrorText = "Something went wrong";
            }
        }
    }
}
