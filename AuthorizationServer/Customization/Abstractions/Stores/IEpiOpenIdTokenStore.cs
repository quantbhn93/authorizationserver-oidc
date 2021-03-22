using OpenIddict.Abstractions;

namespace AuthorizationServer.Customization.Abstractions.Stores
{
    public interface IEpiOpenIdTokenStore<TToken> : IOpenIddictTokenStore<TToken> where TToken : class
    {
    }
}
