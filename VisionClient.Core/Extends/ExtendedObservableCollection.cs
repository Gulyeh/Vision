using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using VisionClient.Core.Models;

namespace VisionClient.Core.Extends
{
    public class ExtendedObservableCollection<T> : ObservableCollection<T> where T : BaseUserModel
    {
        public ExtendedObservableCollection()
        {
            CollectionChanged += ContentCollectionChanged;
        }

        public void ContentCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems is null) return;
                foreach (T item in e.OldItems)
                {
                    item.PropertyChanged -= ItemPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems is null) return;
                foreach (T item in e.NewItems)
                {
                    item.PropertyChanged += ItemPropertyChanged;
                }
            }
        }

        public void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is null) return;

            if ("Status".Equals(e.PropertyName) || "Username".Equals(e.PropertyName))
            {
                NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
                OnCollectionChanged(args);
            }
        }
    }
}
