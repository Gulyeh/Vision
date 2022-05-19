﻿using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Views;
using VisionClient.Views.Login;
using VisionClient.Views.SettingsControls;

namespace VisionClient.Extensions
{
    internal static class RegisterForNavigationExtensions
    { 
        public static void RegisterViews(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<LibraryControl>("LibraryControl");
            containerRegistry.RegisterForNavigation<FriendsControl>("FriendsControl");
            containerRegistry.RegisterForNavigation<GameListControl>("GameListControl");
            containerRegistry.RegisterForNavigation<GameDetailsControl>("GameDetailsControl");
            containerRegistry.RegisterForNavigation<HomeControl>("HomeControl");
            containerRegistry.RegisterForNavigation<HomeDetailsControl>("HomeDetailsControl");
            containerRegistry.RegisterForNavigation<NewsControl>("NewsControl");
            containerRegistry.RegisterForNavigation<SettingsControl>("SettingsControl");
            containerRegistry.RegisterForNavigation<MessageControl>("MessageControl");
            containerRegistry.RegisterForNavigation<GamesControl>("GamesControl");
            containerRegistry.RegisterForNavigation<ProfileControl>("ProfileControl");
            containerRegistry.RegisterForNavigation<SecurityControl>("SecurityControl");
            containerRegistry.RegisterForNavigation<AuthenticationControl>("AuthenticationControl");
            containerRegistry.RegisterForNavigation<AdminPanelControl>("AdminPanelControl");
            containerRegistry.RegisterForNavigation<SearchUserControl>("SearchUserControl");
            containerRegistry.RegisterForNavigation<SearchControl>("SearchControl");
            containerRegistry.RegisterForNavigation<PendingControl>("PendingControl");
            containerRegistry.RegisterForNavigation<RequestsControl>("RequestsControl");
            containerRegistry.RegisterForNavigation<PurchaseControl>("PurchaseControl");
        }
    }
}
