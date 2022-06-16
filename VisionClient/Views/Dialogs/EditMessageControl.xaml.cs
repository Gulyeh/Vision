using System.Windows.Controls;
using System.Windows.Input;

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

            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) textBox.AppendText("\n");
            }
        }
    }
}
