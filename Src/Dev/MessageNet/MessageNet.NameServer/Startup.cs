using Autofac;
using Autofac.Extensions.DependencyInjection;
using Khooversoft.MessageNet.Interface;
using Khooversoft.MessageNet.Management;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Autofac;
using Khooversoft.Toolbox.Configuration;
using Khooversoft.Toolbox.Standard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MessageHub.NameServer
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _hostEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope? AutofacContainer { get; private set; }

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

            builder.Register(x => new WorkContextBuilder().Set(new ServiceProviderProxy(x => AutofacContainer.Resolve(x), x => AutofacContainer.ResolveOptional(x))).Build())
                .As<IWorkContext>()
                .InstancePerLifetimeScope();

            builder.RegisterContainerModule(new RouteManagerContainerRegistrationModule());

            builder.RegisterType<BlobStore>().As<IRegisterStore>().InstancePerLifetimeScope();

            builder.Register(x => new ActorConfigurationBuilder().Set(x.Resolve<IWorkContext>()).Build()).As<ActorConfiguration>().InstancePerLifetimeScope();

            Option option = Configuration.BuildOption<Option>();
            option.Verify(nameof(option)).IsNotNull().Value.Verify();

            builder.Register(x => new BlobStoreConnection(option.BlobRepository.ContainerName, option.BlobRepository.Connection));
            builder.Register(x => new ServiceBusConnection(option.ServiceBusConnection));
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
