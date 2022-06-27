using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Helpers
{
    internal delegate void TextEventHandler(object sender, TextEventArgs e);
    internal class TextEventArgs
    {
        public TextEventArgs(string text) { Text = text; }
        public string Text { get; }
    }

    internal interface ITextEventHelper
    {
        event TextEventHandler NotifyOpened;
        void Notify(string panelName);
    }

    internal class TextEventHelper : ITextEventHelper
    {
        public event TextEventHandler? NotifyOpened;

        public void Notify(string panelName) => NotifyOpened?.Invoke(this, new TextEventArgs(panelName));       
    }
}
