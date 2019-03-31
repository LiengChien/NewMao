using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Unicode;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;
using NewMao.Service.AutoMapper;
using NewMao.WebApi.Middleware;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NewMao.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration
            , IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
            AutoMapperConfig.Configure();
        }
        
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        public Autofac.IContainer ApplicationContainer { get; private set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors().AddMvc();

            services.AddSingleton(_ => Configuration);

            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(UnicodeRanges.All);
            });

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var builder = new ContainerBuilder();
            var selfServices = Assembly.Load("NewMao.Service");
            builder.RegisterAssemblyTypes(selfServices).AsImplementedInterfaces();
            var selfRepos_Repo = Assembly.Load("NewMao.Repository");
            builder.RegisterAssemblyTypes(selfRepos_Repo).AsImplementedInterfaces();

            var selfApi = Assembly.Load("NewMao.WebApi");
            builder.RegisterAssemblyTypes(selfApi).AsImplementedInterfaces();
            builder.Populate(services);

            this.ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(this.ApplicationContainer);
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //Exception Middleware
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseCors(builder =>
            builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            );

            //app.UseHttpsRedirection();
            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)
        //{
        //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        //}
    }
}
