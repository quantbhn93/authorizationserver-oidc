using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Cryptography.X509Certificates;

namespace AuthorizationServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                        {
                            options.LoginPath = "/account/login";
                        });

            services.AddDbContext<DbContext>(options =>
            {
                // configure context to use in memory store
                options.UseInMemoryDatabase(nameof(DbContext));

                // register the entity sets needed by OpenIddict (creating tables for storing OpenIddictAppilications, OpenIddictAuthorization, OpeniddictScopes, OpenIddictTokens)
                //options.UseOpenIddict();
            });

            services.AddOpenIddict()

                // register openiddict core components
                .AddCore(options =>
                {
                    // configure Openiddict to use the EF Core stores/modules
                    //options.UseEntityFrameworkCore().UseDbContext<DbContext>(); //.ReplaceDefaultEntities() https://kevinchalet.com/2018/06/20/openiddict-rc3-is-out/
                    options.UseEpiManagers()
                            .UseEpiInmemoryStore();
                })

                // register  the Openiddict server component
                .AddServer(options =>
                {
                    options.AllowClientCredentialsFlow();
                    options.AllowAuthorizationCodeFlow()
                                // PKCE doesn't replace client secrets. It's meant to bind the authorization code with an authorization request, 
                                // so that the server can check whether the component sending the token request (typically, a mobile app) is the one that started the flow.
                                .RequireProofKeyForCodeExchange() // this makes sure all clients are required to use PKCE (Proof Key for Code Exchange).
                            .AllowRefreshTokenFlow();

                    options.SetTokenEndpointUris("/connect/token");
                    options.SetAuthorizationEndpointUris("/connect/authorize");
                    // It's wise to keep your tokens small (access token, id token etc..). Therefore, the OpenID Connect protocol offers the possibility to expose
                    // an userinfo endpoint from which clients can retrieve extra information about the end-user which is not stored in the identity token.
                    options.SetUserinfoEndpointUris("/connect/userinfo");
                    //options.SetLogoutEndpointUris("");

                    // encryption and signing of tokens (the token is not only signed but also encrypted)
                    options.AddEphemeralEncryptionKey(); // register key for encrypting token.only use during developement
                    options.AddEphemeralSigningKey(); // register key for signing token. only use during developement

                    //X509Certificate2 _jwtSigningCert = new X509Certificate2("", "");
                    //options.AddSigningCertificate(_jwtSigningCert);

                    options.DisableAccessTokenEncryption(); // can disable encryption for token (token is encyprted by default) https://stackoverflow.com/questions/64725068/openiddict-decryption-of-key-failure
                    //options.DisableSlidingRefreshTokenExpiration();

                    // register scopes (permissions) (this will be available through discovery endpoint)
                    //options.RegisterScopes("api");

                    //options.SetAccessTokenLifetime(TimeSpan.FromDays(2));

                    // OpenIddict requires Https even on local evinronment/during development 

                    // register the ASP.NET Core host and configure the ASP.NET Core specific-options (things will available in our custom token endpoints)
                    options.UseAspNetCore().EnableTokenEndpointPassthrough();
                    options.UseAspNetCore().EnableAuthorizationEndpointPassthrough().EnableUserinfoEndpointPassthrough();
                    //options.UseAspNetCore().EnableLogoutEndpointPassthrough();
                });
            //.AddValidation(options =>
            // {
            //     options.UseLocalServer();

            //     options.UseAspNetCore();
            // });

            services.AddHostedService<TestData>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization(); // /connect/userinfo action is decorated with [Authorize] attribute, so we need to use authorization here

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
            });
        }
    }
}
