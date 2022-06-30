using Microsoft.AspNetCore.SignalR.Client;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Events;
using VisionClient.Core.Helpers;

namespace VisionClient.SignalR
{
    internal interface IOrderService_Hubs
    {
        HubConnection? OrderHubConnection { get; set; }
        Task CreateHubConnection(Guid productId, Guid paymentMethodId, OrderType orderType, Guid? gameId = null, string? couponCode = null);
        Task Disconnect();
    }
    internal class OrderService_Hubs : IOrderService_Hubs
    {
        private readonly IStaticData staticData;
        private readonly IEventAggregator eventAggregator;

        public HubConnection? OrderHubConnection { get; set; }

        public OrderService_Hubs(IStaticData staticData, IEventAggregator eventAggregator)
        {
            this.staticData = staticData;
            this.eventAggregator = eventAggregator;
        }

        public async Task CreateHubConnection(Guid productId, Guid paymentMethodId, OrderType orderType, Guid? gameId = null, string? couponCode = null)
        {
            if (OrderHubConnection is not null) return;

            OrderHubConnection = new HubConnectionBuilder()
               .WithUrl(ConnectionData.OrderHub, opts =>
               {
                   opts.AccessTokenProvider = () => Task.FromResult(staticData.UserData?.Access_Token);
                   opts.Headers.Add("orderType", orderType.ToString());
                   opts.Headers.Add("productId", productId.ToString());
                   opts.Headers.Add("paymentMethodId", paymentMethodId.ToString());
                   if (couponCode is not null) opts.Headers.Add("couponCode", couponCode);

                   var GameId = gameId?.ToString();
                   if (GameId is not null) opts.Headers.Add("gameId", GameId);
               })
               .Build();

            ListenConnections();
            await OrderHubConnection.StartAsync();
        }

        private void ListenConnections()
        {
            if (OrderHubConnection is null) return;

            OrderHubConnection.Closed += (e) =>
            {
                SendAggregator("ConnectionClosed");
                return Task.CompletedTask;
            };

            OrderHubConnection.On("PaymentFailed", () =>
            {
                SendAggregator("PaymentFailed");
            });

            OrderHubConnection.On<string>("PaymentUrl", (url) =>
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    SendAggregator("PaymentFailed");
                    return;
                }

                OpenBrowserHelper.OpenUrl(url);

                SendAggregator("PaymentUrl");
            });

            OrderHubConnection.On<CurrencyPurchasedDto>("CurrencyPaymentDone", (data) =>
            {
                if (!data.IsSuccess)
                {
                    SendAggregator("PaymentFailed");
                    return;
                }

                staticData.UserData.CurrencyValue += data.Amount;
                SendAggregator("PaymentDone");
            });

            OrderHubConnection.On<bool>("ProductPaymentDone", (data) =>
            {
                if (!data)
                {
                    SendAggregator("PaymentFailed");
                    return;
                }

                SendAggregator("ProductPurchased");
                SendAggregator("PaymentDone");
            });
        }

        public async Task Disconnect()
        {
            if (OrderHubConnection is null) return;
            await OrderHubConnection.DisposeAsync();
            OrderHubConnection = null;
        }

        private void SendAggregator<T>(T payload)
        {
            Application.Current.MainWindow.Dispatcher.Invoke(new Action(() =>
            {
                eventAggregator.GetEvent<SendEvent<T>>().Publish(payload);
            }));
        }
    }
}
