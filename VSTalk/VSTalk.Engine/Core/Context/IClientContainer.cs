using System.Collections.Generic;
using System.Collections.ObjectModel;
using VSTalk.Model;

namespace VSTalk.Engine.Core.Context
{
    public interface IClientContainer
    {
        ObservableCollection<Client> AvailableClients { get; } 
    }
}