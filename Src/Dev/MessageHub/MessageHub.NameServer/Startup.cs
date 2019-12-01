using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Khooversoft.MessageHub.Management;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Standard;
using MessageHub.Management.Test.RouteManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Toolbox.Autofac;

namespace MessageHub.NameServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac, like:
            //builder.RegisterModule(new MyApplicationModule());

            builder.Register(x => new WorkContextBuilder().Set(new ServiceProviderProxy(x => AutofacContainer.Resolve(x), x => AutofacContainer.ResolveOptional(x))))
                .As<IWorkContext>()
                .InstancePerLifetimeScope();

            builder.RegisterContainerModule(new RouteManagerContainerRegistrationModule());

            builder.RegisterType<BlobRepositoryFake>().As<IBlobRepository>().InstancePerLifetimeScope();
            builder.RegisterType<QueueManagementFake>().As<IQueueManagement>().InstancePerLifetimeScope();
            builder.RegisterType<BlobStore>().As<IRegisterStore>().InstancePerLifetimeScope();

            builder.Register(x => new ActorConfigurationBuilder().Set(x.Resolve<IWorkContext>()).Build()).As<ActorConfiguration>().InstancePerLifetimeScope();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

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
    }
}
