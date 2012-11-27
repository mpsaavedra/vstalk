using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using VSTalk.Model;

namespace VSTalk.Engine.ViewModel.ContactList
{
    public class InterlocutorCollectionConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = (ObservableCollection<Interlocutor>) value[0];
            var client = (Client) value[1];
            var nullWrapper = new ObservableCollection<object>();

            items.CollectionChanged += (sender, args) => HandleCollectionChanges(client, args,
                                                                                 actualCollection: items,
                                                                                 nullCollection: nullWrapper);
            if (items.Count == 0)
            {
                nullWrapper.Add(new NullInterlocutor(client));
            }
            else
            {
                foreach (var interlocutor in items)
                {
                    nullWrapper.Add(interlocutor);
                }
            }
            return nullWrapper;
        }

        private void HandleCollectionChanges(Client client, NotifyCollectionChangedEventArgs args, ObservableCollection<Interlocutor> actualCollection, ObservableCollection<object> nullCollection)
        {
            if (actualCollection.Count == 0)
            {
                nullCollection.Clear();
                nullCollection.Add(new NullInterlocutor(client));
                return;
            }
            var nullInterlocutor = nullCollection.OfType<NullInterlocutor>().FirstOrDefault();
            if (nullInterlocutor != null)
            {
                nullCollection.Remove(nullInterlocutor);
            }

            if (args.OldItems != null)
            {
                foreach (var oldItem in args.OldItems)
                {
                    nullCollection.Remove(oldItem);
                }
            }

            if (args.NewItems != null)
            {
                foreach (var newItem in args.NewItems)
                {
                    nullCollection.Insert(actualCollection.IndexOf((Interlocutor) newItem), newItem);
                }
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}