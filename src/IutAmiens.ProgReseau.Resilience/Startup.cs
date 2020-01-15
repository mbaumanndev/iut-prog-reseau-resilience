using IutAmiens.ProgReseau.Resilience.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Registry;
using Refit;
using System;
using System.Net.Http;

namespace IutAmiens.ProgReseau.Resilience
{
    public sealed class Startup
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public Startup(IConfiguration p_Configuration)
        {
            Configuration = p_Configuration;
        }

        public IConfiguration Configuration { get; }

        private IServiceProvider Services { get; set; }

        public void ConfigureServices(IServiceCollection p_Services)
        {
            p_Services.AddMemoryCache();
            p_Services.AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>();

            IPolicyRegistry<string> v_Registry = p_Services.AddPolicyRegistry();

            p_Services.AddControllersWithViews();

            p_Services.AddHttpClient("weather", context =>
            {
                context.BaseAddress = new Uri("http://webapi");
            })
                .AddTypedClient(RestService.For<IWeatherService>)
                .AddPolicyHandlerFromRegistry(PolicySelector)
                
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5)
                }))
                .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                ))
            ;
        }

        private IAsyncPolicy<HttpResponseMessage> PolicySelector(
            IReadOnlyPolicyRegistry<string> p_Registry,
            HttpRequestMessage p_Request) {
                return p_Registry.Get<IAsyncPolicy<HttpResponseMessage>>("CachingPolicy");
            }

        public void Configure(
            IApplicationBuilder p_App,
            IWebHostEnvironment p_Env,
            IAsyncCacheProvider p_CacheProvider,
            IPolicyRegistry<string> p_Registry)
        {
            AsyncCachePolicy<HttpResponseMessage> v_Policy =
                Policy.CacheAsync<HttpResponseMessage>(
                    p_CacheProvider,
                    TimeSpan.FromSeconds(30)
                );
            p_Registry.Add("CachingPolicy", v_Policy);

            Services = p_App.ApplicationServices;

            if (p_Env.IsDevelopment())
            {
                p_App.UseDeveloperExceptionPage();
            }
            else
            {
                p_App.UseExceptionHandler("/Home/Error");
                p_App.UseHsts();
            }
            p_App.UseHttpsRedirection();
            p_App.UseStaticFiles();

            p_App.UseRouting();

            p_App.UseAuthorization();

            p_App.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}