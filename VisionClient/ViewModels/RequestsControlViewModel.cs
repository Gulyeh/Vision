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
    internal class RequestsControlViewModel : BindableBase
    {
        public ObservableCollection<SearchModel> RequestsList { get; set; }
        public DelegateCommand<SearchModel> AcceptRequestCommand { get; set; }
        public DelegateCommand<SearchModel> DeclineRequestCommand { get; set; }

        public RequestsControlViewModel()
        {
            RequestsList = new ObservableCollection<SearchModel>();
            AcceptRequestCommand = new DelegateCommand<SearchModel>(AcceptRequest);
            DeclineRequestCommand = new DelegateCommand<SearchModel>(DeclineRequest);

            var User = new SearchModel()
            {
                IsFriend = false,
                User = new UserModel()
                {
                    PhotoUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460__340.png",
                    UserName = "TestSUperUser",
                    Id = new Guid()
                }
            };

            RequestsList.Add(User);
            RequestsList.Add(User);
            RequestsList.Add(User);
            RequestsList.Add(User);
            RequestsList.Add(User);
            RequestsList.Add(User);
            RequestsList.Add(User);
            RequestsList.Add(User);
            RequestsList.Add(User);
            RequestsList.Add(User);
        }

        private void AcceptRequest(SearchModel user)
        {

        }

        private void DeclineRequest(SearchModel user)
        {

        }
    }
}
