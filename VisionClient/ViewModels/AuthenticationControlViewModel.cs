using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.ViewModels
{
    internal class AuthenticationControlViewModel : BindableBase
    {
        private string securityCode = string.Empty;
        public string SecurityCode
        {
            get { return securityCode; }
            set 
            {  
                if(value.All(char.IsDigit)) SetProperty(ref securityCode, value); 
            }
        }

        public DelegateCommand<string> GetAppCommand { get; set; }
        public DelegateCommand EnableAuthCommand { get; set; }

        public AuthenticationControlViewModel()
        {
            GetAppCommand = new DelegateCommand<string>(GetApp);
            EnableAuthCommand = new DelegateCommand(EnableAuth);
        }

        private void GetApp(string name)
        {
            switch (name)
            {
                case "Google":
                    break;
                case "Apple":
                    break;
                default:
                    break;
            }
        }

        private void EnableAuth()
        {

        }
    }
}
