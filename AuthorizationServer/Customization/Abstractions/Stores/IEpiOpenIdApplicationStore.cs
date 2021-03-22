using OpenIddict.Abstractions;

namespace AuthorizationServer.Customization.Abstractions.Stores
{
    public interface IEpiOpenIdApplicationStore<TApplication> : IOpenIddictApplicationStore<TApplication> where TApplication : class
    {
    }
}
