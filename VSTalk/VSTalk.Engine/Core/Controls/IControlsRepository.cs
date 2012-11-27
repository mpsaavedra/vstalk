using System.Windows.Controls;

namespace VSTalk.Engine.Core.Controls
{
    public interface IControlsRepository
    {
        UserControl ContactList { get; }
        UserControl ChatFrame { get; }
    }
}