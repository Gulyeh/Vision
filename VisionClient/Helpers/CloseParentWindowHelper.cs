using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VisionClient.Helpers
{
    internal class CloseParentWindowHelper
    {
        public static void Close(UserControl? control)
        {
            Window parentWindow = Window.GetWindow(control as UserControl);
            if(parentWindow.Owner is not null) parentWindow.Owner.Close();
            else parentWindow.Close();
        }
    }
}
