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
    internal class SearchControlViewModel : BindableBase
    {
        private string username = string.Empty;
        public string Username
        {
            get { return username; }
            set {  SetProperty(ref username, value); }
        }

        public ObservableCollection<SearchModel> FoundUsersList { get; set; }
        public DelegateCommand<SearchModel> SendRequestCommand { get; set; }
        public DelegateCommand SearchUserCommand { get; set; }

        public SearchControlViewModel()
        {
            FoundUsersList = new ObservableCollection<SearchModel>();
            SendRequestCommand = new DelegateCommand<SearchModel>(SendRequest);
            SearchUserCommand = new DelegateCommand(SearchUser);

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

            FoundUsersList.Add(User);
            FoundUsersList.Add(User);
            FoundUsersList.Add(User);
            FoundUsersList.Add(User);
            FoundUsersList.Add(User);
            FoundUsersList.Add(User);
            FoundUsersList.Add(User);
            FoundUsersList.Add(User);
            FoundUsersList.Add(User);
            FoundUsersList.Add(User);
            FoundUsersList.Add(User);
            FoundUsersList.Add(User);
        }

        private void SearchUser()
        {

        }

        private void SendRequest(SearchModel user)
        {

        }
    }
}
