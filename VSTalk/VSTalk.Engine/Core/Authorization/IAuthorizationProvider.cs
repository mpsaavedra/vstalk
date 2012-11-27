using VSTalk.Engine.Model;

namespace VSTalk.Engine.Core.Authorization
{
    public interface IAuthorizationProvider
    {
        void HandleAuthorizationsRequest(VsJid id);
        void PushAuthorizationsRequest(VsJid id);
    }
}