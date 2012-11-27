using System.Collections.Generic;

namespace VSTalk.Engine.Core.Context
{
    public interface IConnectionContainer
    {
        IEnumerable<ClientConnection> AvailableConnections { get; }         
    }
}