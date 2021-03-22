using OpenIddict.Abstractions;

namespace AuthorizationServer.Customization.Abstractions.Stores
{
    public interface IEpiOpenIdScopeStore<TSCope> : IOpenIddictScopeStore<TSCope> where TSCope : class
    {
    }
}
