using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisionClient.Core.Helpers.Aggregators
{
    public class LoginWindowLoading
    {
        public LoginWindowLoading(bool visibility)
        {
            LoadingVisibility = visibility;
        }

        public bool LoadingVisibility { get; private set; } = false;
    }
}
