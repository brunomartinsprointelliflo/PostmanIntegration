using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.Swagger;

namespace PostmanAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Postman API",
                    Description = "A simple example ASP.NET Core Web API for testing Postman"
                });

                c.DocumentFilter<YamlDocumentFilter>();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private class YamlDocumentFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
        {

            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {
                string file = AppDomain.CurrentDomain.BaseDirectory.Replace(@"PostmanAPI\bin\Debug\netcoreapp3.1", "OpenApi") + "swagger.yaml";
                
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                var serializer = new YamlDotNet.Serialization.Serializer();
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, swaggerDoc);
                    var stream = new StreamWriter(file);
                    stream.WriteLine(writer.ToString());
                }
            }
        }
    }

   
}
