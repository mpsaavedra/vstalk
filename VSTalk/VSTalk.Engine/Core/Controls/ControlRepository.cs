using System.Windows.Controls;
using VSTalk.Engine.View;
using VSTalk.Engine.ViewModel;

namespace VSTalk.Engine.Core.Controls
{
    public class ControlRepository : IControlsRepository
    {
        private readonly CommonChatFrame _chatFrame;
        private readonly MainControl _contactList;

        public ControlRepository(VSTalkCore core)
        {
            _chatFrame = new CommonChatFrame
            {
                    DataContext = new ChatFrameViewModel(core)
            };

            _contactList = new MainControl
            {
                    DataContext = new MainViewModel(core)
            };
        }

        public UserControl ContactList
        {
            get { return _contactList; }
        }

        public UserControl ChatFrame
        {
            get { return _chatFrame; }
        }
    }
}