using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VisionClient.Views.Dialogs
{
    /// <summary>
    /// Logika interakcji dla klasy EditMessageControl.xaml
    /// </summary>
    public partial class EditMessageControl : UserControl
    {
        public EditMessageControl()
        {
            InitializeComponent();
        }

        private void Message_NewLine(object sender, KeyEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox is null) return;

            if(e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) textBox.AppendText("\n");
            }
        }
    }
}
