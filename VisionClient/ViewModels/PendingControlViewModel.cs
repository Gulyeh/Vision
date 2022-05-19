using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Models;

namespace VisionClient.ViewModels
{
    internal class PendingControlViewModel : BindableBase
    {
        public ObservableCollection<SearchModel> PendingUsersList { get; set; }
        public DelegateCommand<SearchModel> CancelPendingCommand { get; set; }
        public PendingControlViewModel()
        {
            PendingUsersList = new ObservableCollection<SearchModel>();
            CancelPendingCommand = new DelegateCommand<SearchModel>(CancelPending);
            var User = new SearchModel()
            {
                IsFriend = false,
                User = new UserModel()
                {
                    PhotoUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460__340.png",
                    UserName = "TestSUperUser",
                    Id = 12312313
                }
            };

            PendingUsersList.Add(User);
            PendingUsersList.Add(User);
            PendingUsersList.Add(User);
            PendingUsersList.Add(User);
            PendingUsersList.Add(User);
            PendingUsersList.Add(User);
            PendingUsersList.Add(User);
            PendingUsersList.Add(User);
            PendingUsersList.Add(User);
            PendingUsersList.Add(User);
        }

        private void CancelPending(SearchModel user)
        {

        }
    }
}
