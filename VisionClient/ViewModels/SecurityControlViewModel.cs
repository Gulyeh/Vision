using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.ViewModels
{
    internal class SecurityControlViewModel : BindableBase
    {
        private string newpassword = string.Empty;
        public string NewPassword
        {
            get { return newpassword; }
            set {  SetProperty(ref newpassword, value); }
        }

        private string repeatpassword = string.Empty;
        public string RepeatPassword
        {
            get { return repeatpassword; }
            set { SetProperty(ref repeatpassword, value); }
        }

        private string oldpassword = string.Empty;
        public string OldPassword
        {
            get { return oldpassword; }
            set { SetProperty(ref oldpassword, value); }
        }

        public DelegateCommand SaveNewPasswordCommand { get; set; }

        public SecurityControlViewModel()
        {
            SaveNewPasswordCommand = new DelegateCommand(SaveNewPassword);
        }

        private void SaveNewPassword()
        {
            if (!NewPassword.Equals(RepeatPassword)) return;
        }
    }
}
