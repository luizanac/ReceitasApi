using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Aplub.Api.Authentication;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReceitasApi.Authentication;
using ReceitasApi.AutoMapper;
using ReceitasApi.Constants;
using ReceitasApi.Database;
using ReceitasApi.Entities;
using ReceitasApi.Validators;

namespace ReceitasApi {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            #region Injections

            services.AddSingleton<ITokenFactory, TokenFactory> ();

            #endregion

            // Configure CORS
            services.AddCors (options => {
                options.AddPolicy ("DevCORS", policy => {
                    policy.AllowAnyHeader ()
                        .AllowAnyMethod ()
                        .AllowAnyOrigin ()
                        .WithExposedHeaders ("X-Total-Count", "Link", "X-Continuation-Token", "Authorization");
                });

                options.AddPolicy ("ProdCORS", policy => {
                    policy.AllowAnyHeader ()
                        .AllowAnyMethod ()
                        .AllowAnyOrigin ()
                        .WithExposedHeaders ("X-Total-Count", "Link", "X-Continuation-Token", "Authorization");
                });
            });

            //Configure ef core
            services.AddDbContext<ApplicationDbContext> (options => {
                options.UseLazyLoadingProxies ();
                options.UseMySql (MySqlUtil.GetConnectionString (), cfg => {
                    cfg.EnableRetryOnFailure ();
                });
            });

            // Configure identity
            services.AddIdentity<User, IdentityRole<Guid>> (options => {

                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                    options.User.RequireUniqueEmail = true;
                }).AddEntityFrameworkStores<ApplicationDbContext> ()
                .AddDefaultTokenProviders ();

            //Configure Authentication
            services.AddAuthentication (options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer (config => {
                config.RequireHttpsMetadata = true;
                config.SaveToken = true;

                config.TokenValidationParameters = new TokenValidationParameters () {
                    ValidIssuer = Configuration["JwtTokenConfig:Issuer"],
                    ValidateIssuer = false,
                    ValidateAudience = bool.Parse (Configuration["JwtTokenConfig:ValidateAudience"]),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (Environment.GetEnvironmentVariable (EnvVars.JwtKey)))
                };
            });

            //Configure responseCompression
            services.AddResponseCompression ().Configure<BrotliCompressionProviderOptions> (options => {
                options.Level = CompressionLevel.Optimal;
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen (c => {
                c.SwaggerDoc ("v1", new OpenApiInfo { Title = "Receitas Api", Version = "v1" });
                c.AddSecurityDefinition ("Bearer", new OpenApiSecurityScheme {
                    In = ParameterLocation.Header,
                        Description = "Please insert JWT with Bearer into field",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement (new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                c.EnableAnnotations ();
            });

            //Configure AutoMapper
            services.AddAutoMapper (
                typeof (DomainToDtoMappingProfile),
                typeof (DtoToDomainMappingProfile));

            services.AddControllers ()
                .ConfigureApiBehaviorOptions (options => {
                    options.SuppressMapClientErrors = false;
                })
                .AddJsonOptions (options => {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                })
                .SetCompatibilityVersion (Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest)
                .AddFluentValidation (config => {
                    config.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    config.RegisterValidatorsFromAssemblyContaining<Startup> ();
                    ValidatorOptions.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;
                });;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseCors ("DevCORS");
                app.UseDeveloperExceptionPage ();
            } else if (env.IsProduction ()) {
                app.UseCors ("ProdCORS");
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger ();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI (c => {
                c.SwaggerEndpoint ("/swagger/v1/swagger.json", "Receitas API V1");
                c.RoutePrefix = "";
            });

            //app.UseHttpsRedirection ();

            app.UseRouting ();

            app.UseAuthorization ();
            app.UseAuthentication ();
            app.UseResponseCompression ();

            app.UseEndpoints (endpoints => {
                endpoints.MapControllers ();
            });
        }
    }
}