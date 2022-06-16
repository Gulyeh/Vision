using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Models;
using VisionClient.Utility;

namespace VisionClient.SignalR
{
    internal delegate void CouponTextHandler(object sender, CouponTextEventArgs e);
    internal delegate void CouponProductHandler(object sender, CouponProductEventArgs e);

    internal class CouponTextEventArgs
    {
        public CouponTextEventArgs(string text) { Text = text; }
        public string Text { get; }
    }

    internal class CouponProductEventArgs
    {
        public CouponProductEventArgs(ProductAccessDto data) { Data = data; }
        public ProductAccessDto Data { get; }
    }


    internal interface IUsersService_Hubs
    {
        Task Send(UserServiceHubs hub, string methodName, object data);
        Task CreateFriendsConnection();
        Task CreateUsersConnection();
        Task Disconnect();
        event CouponTextHandler? CouponTextEvent;
        event CouponProductHandler? CouponProductEvent;
    }

    internal sealed class UsersService_Hubs : IUsersService_Hubs
    {
        private HubConnection? userHubConnection;
        private HubConnection? friendsHubConnection;

        private readonly IToastNotification toastNotification;
        private readonly IStaticData StaticData;

        public event CouponTextHandler? CouponTextEvent;
        public event CouponProductHandler? CouponProductEvent;

        public UsersService_Hubs(IToastNotification toastNotification, IStaticData staticData)
        {
            this.toastNotification = toastNotification;
            StaticData = staticData;
        }

        public async Task Disconnect()
        {
            if (userHubConnection is null || friendsHubConnection is null) return;
            await userHubConnection.DisposeAsync();
            await friendsHubConnection.DisposeAsync();

            userHubConnection = null;
            friendsHubConnection = null;
        }
        public async Task CreateUsersConnection()
        {
            if (userHubConnection is not null) return;

            userHubConnection = new HubConnectionBuilder()
                .WithUrl(ConnectionData.UsersHub, opts =>
                {
                    opts.AccessTokenProvider = () => Task.FromResult(StaticData.UserData?.Access_Token);
                })
                .WithAutomaticReconnect()
                .Build();

            ListenUsersConnections();
            ListenSelfUserConnections();
            await userHubConnection.StartAsync();
        }
        public async Task CreateFriendsConnection()
        {
            if (friendsHubConnection is not null) return;

            friendsHubConnection = new HubConnectionBuilder()
                .WithUrl(ConnectionData.FriendsHub, opts =>
                {
                    opts.AccessTokenProvider = () => Task.FromResult(StaticData.UserData?.Access_Token);
                })
                .WithAutomaticReconnect()
                .Build();

            ListenFriendsConnections();
            ListenSelfFriendsConnections();
            await friendsHubConnection.StartAsync();
        }

        private void ListenFriendsConnections()
        {
            if (friendsHubConnection is null) throw new Exception("friendsHubConnection is null");

            friendsHubConnection.On<BaseUserModel>("NewFriendRequest", (userData) =>
            {
                if (userData is null) return;
                StaticData.FriendRequestsList.Add(userData);
                toastNotification.Show("Friend Request", $"{userData.Username} sent you friend request!");
            });

            friendsHubConnection.On<Guid>("FriendDeleted", (data) =>
            {
                if (data == Guid.Empty) return;

                var user = StaticData.FriendsList.FirstOrDefault(x => x.UserId == data);
                if (user is not null) StaticData.FriendsList.Remove(user);
            });

            friendsHubConnection.On<Guid>("RequestDeclined", (data) =>
            {
                if (data == Guid.Empty) return;

                var user = StaticData.FriendRequestsList.FirstOrDefault(x => x.UserId == data);
                if (user is not null)
                {
                    StaticData.FriendRequestsList.Remove(user);
                    return;
                }

                user = StaticData.PendingFriendsList.FirstOrDefault(x => x.UserId == data);
                if (user is not null) StaticData.PendingFriendsList.Remove(user);
            });

            friendsHubConnection.On<UserModel>("NewFriend", (userData) =>
            {
                if (userData is null) return;
                var user = StaticData.PendingFriendsList.FirstOrDefault(x => x.UserId == userData.UserId);
                if (user is null) return;

                StaticData.PendingFriendsList.Remove(user);
                StaticData.FriendsList.Add(userData);
            });
        }
        private void ListenSelfFriendsConnections()
        {
            if (friendsHubConnection is null) throw new Exception("friendsHubConnection is null");

            friendsHubConnection.On<ObservableCollection<BaseUserModel>, ObservableCollection<BaseUserModel>, ObservableCollection<UserModel>>("GetFriendsData", (pendingFriends, friendRequests, friendsList) =>
            {
                StaticData.FriendsList.Clear();
                StaticData.FriendsList.AddRange(friendsList);
                StaticData.FriendRequestsList.AddRange(friendRequests);
                StaticData.PendingFriendsList.AddRange(pendingFriends);
            });

            friendsHubConnection.On<BaseUserModel>("FriendRequestsPending", (userData) =>
            {
                if (userData is null) return;
                StaticData.PendingFriendsList.Add(userData);

                var user = StaticData.FoundUsersList.FirstOrDefault(x => x.User.UserId == userData.UserId);
                if (user is null) return;

                user.IsRequestable = false;
            });

            friendsHubConnection.On<UserModel>("AcceptedFriendRequest", (userData) =>
            {
                if (userData is null) return;
                var user = StaticData.FriendRequestsList.FirstOrDefault(x => x.UserId == userData.UserId);
                if (user is null) return;

                StaticData.FriendRequestsList.Remove(user);
                StaticData.FriendsList.Add(userData);
            });

            friendsHubConnection.On<Guid, bool>("ToggleBlockFriend", (userId, isBlocked) =>
            {
                var user = StaticData.FriendsList.FirstOrDefault(x => x.UserId == userId);
                if (user is null) return;

                user.IsBlocked = isBlocked;
                var blockedText = isBlocked ? "Blocked" : "Unblocked";

                toastNotification.Show($"User {blockedText}", $"You have {blockedText.ToLower()} {user.Username}");
            });
        }
        private void ListenUsersConnections()
        {
            if (userHubConnection is null) throw new Exception("userHubConnection is null");
            userHubConnection.On<UserDataDto>("GetUserData", (userData) =>
            {
                if (userData is null) return;
                StaticData.UserData.Status = userData.Status;
                StaticData.UserData.Username = userData.Username;
                StaticData.UserData.CurrencyValue = userData.CurrencyValue;
                StaticData.UserData.UserId = userData.UserId;
                StaticData.UserData.PhotoUrl = userData.PhotoUrl;
                StaticData.UserData.Description = userData.Description;
            });

            userHubConnection.On<UserModel>("UserIsOnline", (data) =>
            {
                if (data is null) return;

                var user = StaticData.FriendsList.FirstOrDefault(x => x.UserId == data.UserId);
                if (user is not null)
                {
                    user.Username = data.Username;
                    user.PhotoUrl = data.PhotoUrl;
                    user.Description = data.Description;
                    user.UserId = data.UserId;
                    user.Status = data.Status;
                }
                toastNotification.Show("", $"{data.Username} is online");
            });

            userHubConnection.On<Guid>("UserIsOffline", (data) =>
            {
                if (data == Guid.Empty) return;

                var user = StaticData.FriendsList.FirstOrDefault(x => x.UserId == data);
                if (user is not null) user.Status = Status.Offline;
            });

            userHubConnection.On<ChangedUserDataDto>("UserChangedData", (data) =>
            {
                if (data is null) return;

                var user = StaticData.FriendsList.FirstOrDefault(x => x.UserId == data.UserId);
                if (user is not null)
                {
                    user.Username = data.Username;
                    user.Description = data.Description;
                    return;
                }
            });

            userHubConnection.On<UserChangedStatusDto>("UserChangedStatus", (data) =>
            {
                if (data is null) return;

                UserModel? user = StaticData.FriendsList.FirstOrDefault(x => x.UserId == data.UserId);
                if (user is null) return;
                var previousStatus = user.Status;
                user.Status = data.Status;

                if (previousStatus == Status.Invisible && (data.Status == Status.Online || data.Status == Status.Away)) toastNotification.Show("", $"{user.Username} is online");
            });

            userHubConnection.On<UserChangedPhotoDto>("UserChangedPhoto", (data) =>
            {
                if (data is null) return;

                var user = StaticData.FriendsList.FirstOrDefault(x => x.UserId == data.UserId);
                if (user is not null) user.PhotoUrl = data.PhotoUrl;
            });

            userHubConnection.On<Guid>("ChatNotification", (senderId) =>
            {
                if (senderId == Guid.Empty) return;
                var user = StaticData.FriendsList.FirstOrDefault(x => x.UserId == senderId);
                if (user is not null && !user.HasUnreadMessages)
                {
                    toastNotification.Show("New message", $"{user.Username} has sent you a new message!");
                    user.HasUnreadMessages = true;
                }
            });
        }
        private void ListenSelfUserConnections()
        {
            if (userHubConnection is null) throw new Exception("userHubConnection is null");
            userHubConnection.On<ChangedUserDataDto>("ChangedData", (userData) =>
            {
                if (userData is null) return;
                StaticData.UserData.Username = userData.Username;
                StaticData.UserData.Description = userData.Description;
            });

            userHubConnection.On<UserChangedStatusDto>("ChangedStatus", (userData) =>
            {
                if (userData is null) return;
                StaticData.UserData.Status = userData.Status;
            });

            userHubConnection.On<CurrencyPurchasedDto>("CurrencyCodeUsed", (data) =>
            {
                if (!data.IsSuccess || data is null)
                {
                    CouponTextEvent?.Invoke(this, new CouponTextEventArgs("Something went wrong while using coupon"));
                    return;
                }

                StaticData.UserData.CurrencyValue += data.Amount;
                CouponTextEvent?.Invoke(this, new CouponTextEventArgs($"{data.Amount} Visions has been added to your account"));
            });

            userHubConnection.On<ProductAccessDto>("ProductCodeUsed", (data) =>
            {
                if (data is null || !data.IsSuccess)
                {
                    CouponTextEvent?.Invoke(this, new CouponTextEventArgs("Something went wrong while using coupon"));
                    return;
                }

                CouponTextEvent?.Invoke(this, new CouponTextEventArgs("Coupon has been redeemed successfully"));
                CouponProductEvent?.Invoke(this, new CouponProductEventArgs(data));
            });

            userHubConnection.On<string>("CodeFailed", (data) =>
            {
                if (string.IsNullOrWhiteSpace(data)) return;
                CouponTextEvent?.Invoke(this, new CouponTextEventArgs(data));
            });
        }

        public async Task Send(UserServiceHubs hub, string methodName, object data)
        {
            var hubSelected = hub switch
            {
                UserServiceHubs.Users => userHubConnection,
                UserServiceHubs.Friends => friendsHubConnection,
                _ => null,
            };

            if (hubSelected is not null) await hubSelected.SendAsync(methodName, data);
        }

    }
}
