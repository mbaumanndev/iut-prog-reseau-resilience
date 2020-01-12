using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace IutAmiens.ProgReseau.SampleApi
{
    public class Startup
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

        public void ConfigureServices(IServiceCollection p_Services)
        {
            p_Services.AddControllers();

            p_Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder p_App, IWebHostEnvironment p_Env)
        {
            if (p_Env.IsDevelopment())
            {
                p_App.UseDeveloperExceptionPage();
            }

            p_App.UseHttpsRedirection();

            p_App.UseRouting();

            p_App.UseAuthorization();

            p_App.UseSwagger();
            p_App.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            p_App.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}