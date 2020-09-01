using System;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CoreAPi;
using AutoMapper;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http.Features;
using TokenDecrypt;
namespace Mywebsite
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
            services.AddEntityFrameworkSqlServer().AddDbContext<AppDBContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            }, ServiceLifetime.Scoped);
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(Options =>
                {
                    var ServerSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
                    Options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = Configuration["Jwt:Issuer"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        //RoleClaimType ="http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });
            //加入MVC架構
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(
                options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; //輸出時自動忽略Null欄位
                    options.SerializerSettings.Formatting = Formatting.Indented; //輸出時按照JSON格式自動縮排
                }
            );
            //解碼token
            services.AddSingleton<AuthorizeTokenDecrypt>();
            services.AddHttpClient();
            // 加入Swagger API 文件
            services.AddSwaggerGen(c =>
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "Api.xml");
                c.IncludeXmlComments(filePath);
                c.SwaggerDoc(
                    name: "v1",
                    info: new OpenApiInfo
                    {
                        Title = "My API",
                        Version = "v1",
                        Description = "ABC",
                        TermsOfService = new Uri("https://example.com/terms"),
                        Contact = new OpenApiContact
                        {
                            Name = "Ryouji",
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Use under LICX",
                            Url = new Uri("https://example.com/license")
                        }
                    }
                );
            });
            services.AddCors(Options =>
            {
                Options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            // 加入 AutoMapper
            services.AddAutoMapper();
            // 檔案上傳
            services.AddSingleton<IFileProvider>
            (
                new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
            );
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
            // Step1: 設定用哪種方式驗證HTTP Request 是否合法
            
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, AppDBContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //建立資料庫連線
            dbContext.Database.EnsureCreated();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "My API v1"
                );
            });
            app.UseCors("CorsPolicy");
            app.UseMvc();
            app.UseMvcWithDefaultRoute();
        }
    }
}


