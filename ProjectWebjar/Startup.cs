using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using ProjectWebjar.Data;
using StackExchange.Redis;
using Hangfire;
using Hangfire.SqlServer;
using System;
using ProjectWebjar.Repository;

namespace ProjectWebjar
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

            services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(Configuration.GetConnectionString("CS"), new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            #region Redis Multiplexer

            services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect("localhost:9191,password=123456"));

            #endregion

            //redis
            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = "localhost:6379";
            });
            //
            services.AddScoped<IPicsRepository, PicsRepository>();
            services.AddScoped<ICronJob, CronJob>();

            services.AddControllersWithViews();
            services.AddMvc();
            services.AddDbContext<ProjectWebjarContext>(item => item.UseSqlServer(Configuration.GetConnectionString("CS")));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjectWebjar", Version = "v1" });
            });
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectWebjar v1"));
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

                app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

            //Hangfire and cronjob for picture
            app.UseHangfireDashboard();
            var cronRepo = serviceProvider.GetRequiredService<ICronJob>();
            backgroundJobClient.Enqueue(() => cronRepo.DeletedHeavyPics());
            recurringJobManager.AddOrUpdate(
                "Every Day 00:00",
                () => cronRepo.DeletedHeavyPics(),
                Cron.Minutely
                );
        }
    }
}
