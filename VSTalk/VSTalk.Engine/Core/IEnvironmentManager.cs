using System.Collections.Generic;

namespace VSTalk.Engine.Core
{
    public interface IEnvironmentManager
    {
        IEnumerable<string> GetCallStack();

        string GetActiveDocument();

        string GetDebugOutput();
    }
}