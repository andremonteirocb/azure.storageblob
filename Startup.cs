using Azure.Storage.Blobs;
using Fundamentos.Azure.StorageBlob.Interfaces;
using Fundamentos.Azure.StorageBlob.Services;
using Fundamentos.Azure.StorageBlob.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Fundamentos.Azure.StorageBlob
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fundamentos.Azure.StorageBlob", Version = "v1" });
            });

            var blobSettings = Configuration.GetSection("BlobSettings").Get<BlobSettings>();
            services.AddSingleton(new BlobContainerClient(blobSettings.ConnectionString, blobSettings.ContainerName));
            services.AddScoped<IBlobService, BlobService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fundamentos.Azure.StorageBlob v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
