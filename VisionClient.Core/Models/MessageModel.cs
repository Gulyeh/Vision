using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Models
{
    public class MessageModel : ICloneable
    {
        public MessageModel()
        {
            Content = string.Empty;
            User = new UserModel();
            Attachments = new ObservableCollection<AttachmentModel>();
        }

        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime DateSent { get; set; }
        public bool IsEdited { get; set; }
        public UserModel User { get; set; }
        public ObservableCollection<AttachmentModel> Attachments { get; set; }

        public object Clone()
        {
            MessageModel clone = (MessageModel)this.MemberwiseClone();
            clone.Attachments = new ObservableCollection<AttachmentModel>();
            foreach(var attachment in Attachments)
            {
                clone.Attachments.Add((AttachmentModel)attachment.Clone());
            }

            return clone;
        }
    }
}
