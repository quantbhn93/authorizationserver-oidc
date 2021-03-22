using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace AuthorizationServer.Controllers
{
    public class AuthorizationController : Controller
    {
        /// <summary>
        ///     - Authorization Endpoint
        ///     - TThis endpoint authorises access a protected resource. This resource could be the resource owners identity or an API.
        ///     - Endpoints for retrieving authorization code, which will be used for exchanging acccess token at `~/connect/token` endpoint
        /// </summary>
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Retrieve the user principal stored in the authentication cookie. (user logged in using CookieAuthenticationDefaults.AuthenticationScheme in Acccount Controller)
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // If the user principal can't be extracted, redirect the user to the login page.
            if (!result.Succeeded)
            {
                return Challenge(
                    authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                            Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                    });
            }

            // To store extra profile information in the identity token, you can add claims in the Authorize method 
            // in the AuthorizationController when using the authorization code flow 
            // or add claims in the token endpoint if you are using the client credentials flow. Make sure to set the destination to IdentityToken.

            // Create a new claims principal
            var claims = new List<Claim>
            {
                // 'subject' claim which is required
                new Claim(OpenIddictConstants.Claims.Subject, result.Principal.Identity.Name),
                new Claim("quantran2 claim", "quantran2 value").SetDestinations(OpenIddictConstants.Destinations.AccessToken),
                new Claim(OpenIddictConstants.Claims.Email, "quan.tran@episerver.com").SetDestinations(OpenIddictConstants.Destinations.IdentityToken)
            };

            var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Set requested scopes (this is not done automatically)
            claimsPrincipal.SetScopes(request.GetScopes());

            // Signing in with the OpenIddict authentiction scheme trigger OpenIddict to issue a code (which can be exchanged for an access token)
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Create a new authentication ticket holding the user identity.
            //var ticket = new AuthenticationTicket(claimsPrincipal,
            //    new AuthenticationProperties(),
            //    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            //// Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            //return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        }

        /// <summary>
        ///     - Tokens endpoint.
        ///     - This endpoint allows the requester to directly retrieve tokens. If the authorization endpoint is human interaction, this endpoint is machine to machine interaction.
        ///     - Identity token will be returned as an additional token (along with access token) if client requesting `scope` including `openid` scope
        ///     - If a client wants
        /// </summary> to use refresh tokens, it needs to request the `offline_access` scope. Just like we requested the openid scope for identity tokens
        ///     - WHen using refresh token (grant type = `refresh_token`) to get new access token, don't need to specify scope because the refresh token holds all needed inforamtion
        ///             and all access token, refresh token, id token(if previsouly specify)  wil be included in the response.
        [HttpPost("~/connect/token")]
        [Produces("application/json")]
        public async Task<IActionResult> ExchangeAsync()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
            ClaimsPrincipal claimsPrincipal;

            if (request.IsClientCredentialsGrantType())
            {
                // Note: the client credentials are automatically validated by OpenIddict:
                // if client_id or client_secret are invalid, this action won't be invoked.
                var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // Subject (sub) is a required field, we use the client id as the subject identifier here.
                // this claim will be included in access token automatically
                identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

                // Add some claim, don't forget to add destination otherwise it won't be added to the access token.
                identity.AddClaim("quantran-claim", "quantran-value", OpenIddictConstants.Destinations.AccessToken);

                claimsPrincipal = new ClaimsPrincipal(identity);

                // OpenIddict has already checked if the requested scopes are allowed (in general and for the current client).
                // The reason why we have to add the scopes manually here is that we are able to filter the scopes granted here if we want to.
                claimsPrincipal.SetScopes(request.GetScopes());
            }
            else if (request.IsAuthorizationCodeGrantType())
            {
                // Retrieve the claims principal stored in the authorization code
                claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            }
            else if (request.IsRefreshTokenGrantType())
            {
                // Basically, we only need to retrieve the ClaimsPrincipal from the refresh token, and sign in again, which will return a new access token.
                // Retrieve the claims principal stored in the refresh token.
                claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

                // set life time 
                claimsPrincipal.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
                claimsPrincipal.SetAuthorizationCodeLifetime(TimeSpan.FromMinutes(2));
                claimsPrincipal.SetIdentityTokenLifetime(TimeSpan.FromMinutes(30));
                claimsPrincipal.SetRefreshTokenLifetime(TimeSpan.FromDays(2));
            }
            else
            {
                throw new InvalidOperationException("The specified grant type is not supported.");
            }

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        /// <summary>
        ///     - User Info Endpoint
        ///     - A separate endpoint for getting user information (because not all user information included in Id token)
        ///             and we should keep tokens as small size as possible 
        ///     - We can decide for ourselves which user info we want to share with clients.
        /// </summary>
        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo")]
        public async Task<IActionResult> Userinfo()
        {
            var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

            return Ok(new
            {
                Name = claimsPrincipal.GetClaim(OpenIddictConstants.Claims.Subject),
                Occupation = ".NET Developer",
                Age = 28,
                FullName = "Tran Ba Quan",
                Company = "Optimizely !"
            });
        }

    }
}
