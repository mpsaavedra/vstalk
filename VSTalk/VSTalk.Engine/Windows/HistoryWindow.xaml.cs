using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MahApps.Metro.Controls;
using VSTalk.Engine.Core;
using VSTalk.Engine.Core.Context;
using VSTalk.Engine.Model;
using VSTalk.Engine.Settings;
using VSTalk.Engine.Utils.Extensions;
using VSTalk.Tools;
using VSTalk.Model;

namespace VSTalk.Engine.Windows
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : MetroWindow
    {
        private IModelContext _context;
        public Interlocutor Interlocutor { get; set; }

        public ObservableCollection<HistoryView> HistoryView { get; set; }

        public RichTextBox HistoryMessages { get; set; }

        public HistoryWindow(Interlocutor interlocutor, IModelContext context)
        {
            InitializeComponent();
            _context = context;
            this.DataContext = this;
            Interlocutor = interlocutor;
            var history = new MessageRepository().LoadHistory(interlocutor) ?? new History
            {
                InterlocutorId = interlocutor.Id,
                Messages = new List<Message>()
            };
            HistoryView = new ObservableCollection<HistoryView>();

            HistoryMessages = new RichTextBox(new FlowDocument());
            HistoryMessages.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HistoryMessages.IsReadOnly = true;

            var monthGroups = history.Messages
                .OrderByDescending(note => note.Date)
                .GroupBy(key => key.Date.ToString("MMMM yyyy"));
            foreach (var monthGroup in monthGroups)
            {
                HistoryView.Add(new HistoryView
                {
                    Month = monthGroup.Key,
                    ActiveDates = new ObservableCollection<DayMessageView>(
                        monthGroup.GroupBy(key => key.Date.ToShortDateString())
                        .Select(dayGroup => new DayMessageView
                        {
                            Day = dayGroup.Key,
                            Messages = new ObservableCollection<Message>(dayGroup)
                        }))
                });
            }
        }

        private void TreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedDay = e.NewValue as DayMessageView;
            if (selectedDay == null || e.NewValue == e.OldValue) return;
            HistoryMessages.BeginChange();
            HistoryMessages.Document.Blocks.Clear();
            selectedDay.Messages.Each(msg => HistoryMessages.Document.Blocks.AddRange(msg.CreateParagraph(_context)));
            HistoryMessages.EndChange();
        }
    }

    public class HistoryView
    {
        public String Month { get; set; }

        public ObservableCollection<DayMessageView> ActiveDates { get; set; }
    }

    public class DayMessageView
    {
        public String Day { get; set; }

        public IEnumerable<Message> Messages { get; set; }
    }
}
