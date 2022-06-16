using System.Collections.ObjectModel;
using VisionClient.Core.Helpers;

namespace VisionClient.Core.Models
{
    public class MessageModel : NotifyPropertyChanged, ICloneable
    {
        public MessageModel()
        {
            User = new UserModel();
            Attachments = new ObservableCollection<AttachmentModel>();
        }

        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        private string? content;
        public string? Content
        {
            get => content;
            set
            {
                content = value;
                OnPropertyChanged();
            }
        }
        private DateTime messageSent;
        public DateTime MessageSent
        {
            get => messageSent;
            set => messageSent = value.ToLocalTime();
        }
        private DateTime? dateRead;
        public DateTime? DateRead
        {
            get => dateRead;
            set
            {
                dateRead = value;
                OnPropertyChanged();
            }
        }
        private bool isEdited;
        public bool IsEdited
        {
            get => isEdited;
            set
            {
                isEdited = value;
                OnPropertyChanged();
            }
        }
        public UserModel User { get; set; }

        private ObservableCollection<AttachmentModel> attachments = new();
        public ObservableCollection<AttachmentModel> Attachments
        {
            get => attachments;
            set
            {
                attachments = value;
                OnPropertyChanged();
            }
        }

        public object Clone()
        {
            MessageModel clone = (MessageModel)this.MemberwiseClone();
            clone.Attachments = new ObservableCollection<AttachmentModel>();
            foreach (var attachment in Attachments)
            {
                clone.Attachments.Add((AttachmentModel)attachment.Clone());
            }

            return clone;
        }
    }
}
