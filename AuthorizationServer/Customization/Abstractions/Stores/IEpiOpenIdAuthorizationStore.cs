using OpenIddict.Abstractions;

namespace AuthorizationServer.Customization.Abstractions.Stores
{
    public interface IEpiOpenIdAuthorizationStore<TAuthorization> : IOpenIddictAuthorizationStore<TAuthorization> where TAuthorization : class
    {
    }
}
