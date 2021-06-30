using System.Collections.Generic;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace API
{
    #region StartUpProduction
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("StoreConnection"));
            });
            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("IdentityConnection"));
            });

            services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(config =>
            {
                var configuration = ConfigurationOptions.Parse(_configuration
                    .GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(configuration);
            });
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddIdentityServices(_configuration);
            services.AddApplicationServices();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Skinet API", Version = "v1" });
                var securitySchema = new OpenApiSecurityScheme()
                {
                    Description = "JWT Auth Bearer Schema",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Type = SecuritySchemeType.Http,
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme,
                    }
                };
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securitySchema);
                var securityRequirement = new OpenApiSecurityRequirement()
                {
                    {securitySchema, new List<string>() {JwtBearerDefaults.AuthenticationScheme} }
                };
                options.AddSecurityRequirement(securityRequirement);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://shop.com");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            app.UseMiddleware<ExceptionMiddleware>();
            // if a request comes into Aapi but we dont have an endpoint that match 
            //that particular request then we are going to hit this bit of middleware and
            // its going to redirect to ErrorController
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles(); // wwwroot
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "SKiNet APi v1");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }



    #endregion

    #region StartupDevelopment

    public class StartupDevelopment
    {
        private readonly IConfiguration _configuration;
        public StartupDevelopment(IConfiguration configuration)
        {
            _configuration =configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("StoreConnection"));
            });
            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("IdentityConnection"));
            });

            services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(config =>
            {
                var configuration = ConfigurationOptions.Parse(_configuration
                    .GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(configuration);
            });
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddIdentityServices(_configuration);
            services.AddApplicationServices();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Skinet API", Version = "v1" });
                var securitySchema = new OpenApiSecurityScheme()
                {
                    Description = "JWT Auth Bearer Schema",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Type = SecuritySchemeType.Http,
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme,
                    }
                };
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securitySchema);
                var securityRequirement = new OpenApiSecurityRequirement()
                {
                    {securitySchema, new List<string>() {JwtBearerDefaults.AuthenticationScheme} }
                };
                options.AddSecurityRequirement(securityRequirement);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            app.UseMiddleware<ExceptionMiddleware>();
            // if a request comes into Aapi but we dont have an endpoint that match 
            //that particular request then we are going to hit this bit of middleware and
            // its going to redirect to ErrorController
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles(); // wwwroot
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "SKiNet APi v1");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    #endregion


}
