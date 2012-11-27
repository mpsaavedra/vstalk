using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace VSTalk.Tools
{
    public static class ObservableCollectionExtension
    {
        public static void HandleCollectionChanges<T>(this ObservableCollection<T> collection, Action<T> added, Action<T> removed)
        {
            collection.CollectionChanged += (sender, args) =>
            {
                if (args.OldItems != null)
                {
                    foreach (var oldItem in args.OldItems)
                    {
                        removed((T) oldItem);
                    }
                }

                if (args.NewItems != null)
                {
                    foreach (var newItem in args.NewItems)
                    {
                        added((T) newItem);
                    }
                }
            };
        }

        public static void HandleCollectionChanges<T>(this ObservableCollection<T> collection, Action<T> initial, Action<T> added, Action<T> removed)
        {
            foreach (var item in collection)
            {
                initial(item);
            }
            HandleCollectionChanges(collection, added, removed);
        }

        public static void HandleInnerChanges<T1,T2>(this ObservableCollection<T1> collection, Expression<Func<T1, T2>> innerSelector, Action<T2> handler)
            where T1 : INotifyPropertyChanged
        {
            Action<T1> collectionHandler = obj =>
            {
                obj.PropertyChanged += (sender, args) =>
                {
                    if (GetPropertyName(innerSelector) == args.PropertyName)
                    {
                        handler(innerSelector.Compile()(obj));
                    }
                };
            };
            HandleCollectionChanges(collection, collectionHandler, collectionHandler, delegate { });
        }

        private static string GetPropertyName<T1, T2>(Expression<Func<T1, T2>> property)
            where T1 : INotifyPropertyChanged
        {
            var lambda = property as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.Name;
            }
            throw new InvalidOperationException("Cannot extract property name");
        }
    }
}