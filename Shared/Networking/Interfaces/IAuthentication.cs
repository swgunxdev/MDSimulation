//
// File Namespace: IAuthentication.cs
// ----------------------------------------------------------------------

namespace Networking.Interfaces
{
    public interface ICredentials<IdType, CredentialType>
    {
        IdType UserId { get; }
        CredentialType Key { get; }
    }

    public interface IAuthenticatorSimple
    {
        // Authenticate a user based upon user credentials
        bool Authenticate(string userId, string password);
    }

    public interface IAuthenticator<IdType, CredentialType> : IAuthenticatorSimple
    {
        // Authenticate a user based upon user credentials
        bool Authenticate(ICredentials<IdType, CredentialType> creds);
    }
}

