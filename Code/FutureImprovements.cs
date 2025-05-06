
// // Add compliance services
// builder.Services.AddCompliance(options =>
// {
//     options.SetDefaultDataClassification(DataClassificationTypes.General);
// });

// builder.Logging.EnableRedaction(options =>
// {     
// });

// builder.Services.AddRedaction(x =>
// {
//     //x.SetRedactor<ErasingRedactor>(new DataClassificationSet(DataTaxonomy.SensitiveData));
//     x.SetRedactor<StarRedactor>(new DataClassificationSet(GDPRType.Sensitive));
//
// #pragma warning disable EXTEXP0002
//     x.SetHmacRedactor(options =>
//     {
//         options.Key = Convert.ToBase64String("SecretKeyDontHardcodeInsteadStoreAndLoadSecurely"u8);
//         options.KeyId = 69;
//     }, new DataClassificationSet(DataTaxonomy.PiiData));
// });


//
// builder.Services.AddOpenApi();
//
// // Register Swagger services
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.CustomOperationIds(apiDesc =>
//     {
//         // Generate a unique ID using controller name, action name, and HTTP method
//         return
//             $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.ActionDescriptor.RouteValues["action"]}_{apiDesc.HttpMethod}";
//
//         // options.CustomOperationIds(apiDesc =>
//         // {
//         //     var controller = apiDesc.ActionDescriptor.RouteValues.TryGetValue("controller", out var controllerName)
//         //         ? controllerName
//         //         : "Default";
//         //     var action = apiDesc.ActionDescriptor.RouteValues.TryGetValue("action", out var actionName)
//         //         ? actionName
//         //         : apiDesc.RelativePath ?? "Unknown";
//         //     return $"{controller}_{action}_{apiDesc.HttpMethod}";
//         // });
//
//         options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//         {
//             Title = "AppBlueprint API",
//             Version = "v1"
//         });
//
//         // options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
//         // {
//         //     Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
//         //     In = ParameterLocation.Header,
//         //     Name = "Authorization",
//         //     Type = SecuritySchemeType.ApiKey
//         // });
//         // Bearer
//         options.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme()
//         {
//             Name = "Authorization",
//             Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
//             In = ParameterLocation.Header,
//             Scheme = "bearer",
//             BearerFormat = "JWT",
//             Type = SecuritySchemeType.Http,
//         });
//
//         options.AddSecurityRequirement(new OpenApiSecurityRequirement
//         {
//             {
//                 new OpenApiSecurityScheme
//                 {
//                     Reference = new OpenApiReference
//                     {
//                         Type = ReferenceType.SecurityScheme,
//                         Id = "BearerAuth"
//                     }
//                 },
//                 new string[] { }
//             }
//         });
//
//         // Register the custom operation filter
//         options.OperationFilter<AddHeaderOperationFilter>();
//         options.OperationFilter<SecurityRequirementsOperationFilter>();
//         return null;
//     });
// });




// this might fail to work on coolify because traefik will handle the ssl termination thus the app will not be able to see the ssl certificate and should run on http
// this config makes swagger docs ui fail to render!
// builder.WebHost.ConfigureKestrel((context, options) =>
//    {
//        options.ListenAnyIP(8081, listenOptions =>
//        {
//            // enable http1, http2 and http3
//            listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
//            listenOptions.UseHttps();
//        });
//    });

/*
 * pulumi - infrastructure as code  (not deployment manager - will run as part of creation step in the developer cli using pulumi automation api)
 * Cloudflare setup for application
 *
 * blob file structure
 * generate sbom using syft in github actions pipeline and then feed to dependency track system to visualize the sbom<
 * remote managed feature flags
 * ansible - configuration management for linux vm
 * redis som queue message broker
 * postgresql row level security https://www.google.com/url?sa=t&source=web&rct=j&opi=89978449&url=https://www.youtube.com/watch%3Fv%3DvZT1Qx2xUCo&ved=2ahUKEwjbtP7IqPmKAxW_R_EDHetcNmgQwqsBegQIEBAF&usg=AOvVaw1X5HuVzLTxJqXHGSpUE_kk
 */


// Register Swagger services
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.CustomOperationIds(apiDesc =>
//     {
//         // Generate a unique ID using controller name, action name, and HTTP method
//         return
//             $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.ActionDescriptor.RouteValues["action"]}_{apiDesc.HttpMethod}";
//
//         // options.CustomOperationIds(apiDesc =>
//         // {
//         //     var controller = apiDesc.ActionDescriptor.RouteValues.TryGetValue("controller", out var controllerName)
//         //         ? controllerName
//         //         : "Default";
//         //     var action = apiDesc.ActionDescriptor.RouteValues.TryGetValue("action", out var actionName)
//         //         ? actionName
//         //         : apiDesc.RelativePath ?? "Unknown";
//         //     return $"{controller}_{action}_{apiDesc.HttpMethod}";
//         // });
//
//         options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//         {
//             Title = "AppBlueprint API",
//             Version = "v1"
//         });
//
//         // options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
//         // {
//         //     Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
//         //     In = ParameterLocation.Header,
//         //     Name = "Authorization",
//         //     Type = SecuritySchemeType.ApiKey
//         // });
//         // Bearer
//         options.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme()
//         {
//             Name = "Authorization",
//             Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
//             In = ParameterLocation.Header,
//             Scheme = "bearer",
//             BearerFormat = "JWT",
//             Type = SecuritySchemeType.Http,
//         });
//
//         options.AddSecurityRequirement(new OpenApiSecurityRequirement
//         {
//             {
//                 new OpenApiSecurityScheme
//                 {
//                     Reference = new OpenApiReference
//                     {
//                         Type = ReferenceType.SecurityScheme,
//                         Id = "BearerAuth"
//                     }
//                 },
//                 new string[] { }
//             }
//         });
//
//         // Register the custom operation filter
//         options.OperationFilter<AddHeaderOperationFilter>();
//         options.OperationFilter<SecurityRequirementsOperationFilter>();
//         return null;
//     });
// });


// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new OpenApiInfo { Title = "AppBlueprint API", Version = "v1" });
//     options.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
//     {
//         Name = "Authorization",
//         Description = "JWT Authorization header using the Bearer scheme",
//         In = ParameterLocation.Header,
//         Scheme = "bearer",
//         BearerFormat = "JWT",
//         Type = SecuritySchemeType.Http,
//     });
//         
//     options.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "BearerAuth"
//                 }
//             },
//             new string[] { }
//         }
//     });
// });


// app.UseSwaggerUI(c =>
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Appblueprint API V1");
//     c.RoutePrefix = string.Empty; // Swagger at root (optional)
// });    


// Add only the specific controller as a service
// builder.Services.AddTransient<AppBlueprint.Presentation.ApiModule.Controllers.Baseline.AccountController>();

// builder.Services.AddDbContext<ApplicationDBContext>(options =>
//         options.UseNpgsql(
//             connectionString,
//             npgsqlOptions =>
//             {
//                 npgsqlOptions.CommandTimeout(60); // Query timeout
//                 npgsqlOptions.EnableRetryOnFailure(
//                     maxRetryCount: 5, // Retry failed attempts
//                     maxRetryDelay: TimeSpan.FromSeconds(10),
//                     errorCodesToAdd: null);
//             }),
//     // .EnableDetailedErrors()
//     // .LogTo(Console.WriteLine),
//     ServiceLifetime.Scoped); // Use Scoped lifetime



// nswag til at genere sdk til at kalde api via frontend

// builder.Services.ConfigureHttpJsonOptions(options =>
// {
//     options.SerializerOptions.MaxDepth =
//         256; // Increase depth for Scalar otherwise it will throw an exception when generating the open api schema
// });
//
// builder.Services.AddOpenApi();
//
// builder.Logging.AddConsole(); // Output logs to the console
//
// string jwtSecretSigningKey =
//     Environment.GetEnvironmentVariable("APPBLUEPRINT_SUPABASE_JWT_SIGNING_KEY", EnvironmentVariableTarget.Process);
//
// if (builder.Environment.IsDevelopment())
// {
//     jwtSecretSigningKey =
//         Environment.GetEnvironmentVariable("APPBLUEPRINT_SUPABASE_JWT_SIGNING_KEY", EnvironmentVariableTarget.User);
// }
//
// Console.WriteLine("jwtSecretKey : " + jwtSecretSigningKey);
//
//
// string supabaseConnectionString = string.Empty;
//
// if (builder.Environment.IsProduction())
// {
//     supabaseConnectionString =
//         Environment.GetEnvironmentVariable("APPBLUEPRINT_SUPABASE_CONNECTIONSTRING", EnvironmentVariableTarget.Process);
// }
//
// if (builder.Environment.IsDevelopment())
// {
//     supabaseConnectionString =
//         Environment.GetEnvironmentVariable("APPBLUEPRINT_SUPABASE_CONNECTIONSTRING", EnvironmentVariableTarget.User);
// }
//
// Console.WriteLine("connectionstring : " + supabaseConnectionString);
//
// // Add service defaults & Aspire components.
// builder.AddServiceDefaults();
//
// builder.Services.AddProblemDetails();
//
// builder.Services.AddAntiforgery();
//
// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policy =>
//     {
//         policy.AllowAnyOrigin()
//             .AllowAnyMethod()
//             .AllowAnyHeader();
//     });
// });
//
// builder.Services.AddDbContext<ApplicationDBContext>(options =>
//         options.UseNpgsql(
//             supabaseConnectionString,
//             npgsqlOptions =>
//             {
//                 npgsqlOptions.CommandTimeout(60); // Query timeout
//                 npgsqlOptions.EnableRetryOnFailure(
//                     maxRetryCount: 5, // Retry failed attempts
//                     maxRetryDelay: TimeSpan.FromSeconds(10),
//                     errorCodesToAdd: null);
//             }),
//     // .EnableDetailedErrors()
//     // .LogTo(Console.WriteLine),
//     ServiceLifetime.Scoped); // Use Scoped lifetime
//
//
//
//
// // Register Swagger services
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.CustomOperationIds(apiDesc =>
//     {
//         // Generate a unique ID using controller name, action name, and HTTP method
//         return
//             $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.ActionDescriptor.RouteValues["action"]}_{apiDesc.HttpMethod}";
//
//         // options.CustomOperationIds(apiDesc =>
//         // {
//         //     var controller = apiDesc.ActionDescriptor.RouteValues.TryGetValue("controller", out var controllerName)
//         //         ? controllerName
//         //         : "Default";
//         //     var action = apiDesc.ActionDescriptor.RouteValues.TryGetValue("action", out var actionName)
//         //         ? actionName
//         //         : apiDesc.RelativePath ?? "Unknown";
//         //     return $"{controller}_{action}_{apiDesc.HttpMethod}";
//         // });
//
//         options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//         {
//             Title = "AppBlueprint API",
//             Version = "v1"
//         });
//
//         // options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
//         // {
//         //     Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
//         //     In = ParameterLocation.Header,
//         //     Name = "Authorization",
//         //     Type = SecuritySchemeType.ApiKey
//         // });
//         // Bearer
//         options.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme()
//         {
//             Name = "Authorization",
//             Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
//             In = ParameterLocation.Header,
//             Scheme = "bearer",
//             BearerFormat = "JWT",
//             Type = SecuritySchemeType.Http,
//         });
//
//         options.AddSecurityRequirement(new OpenApiSecurityRequirement
//         {
//             {
//                 new OpenApiSecurityScheme
//                 {
//                     Reference = new OpenApiReference
//                     {
//                         Type = ReferenceType.SecurityScheme,
//                         Id = "BearerAuth"
//                     }
//                 },
//                 new string[] { }
//             }
//         });
//
//         // Register the custom operation filter
//         options.OperationFilter<AddHeaderOperationFilter>();
//         options.OperationFilter<SecurityRequirementsOperationFilter>();
//         return null;
//     });
// });
//
// builder.Services.AddAuthentication(options =>
//     {
//         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//     })
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = "https://nnihwwacvgqfbkxzjnvx.supabase.co/auth/v1",
//             ValidAudience = "authenticated",
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretSigningKey))
//         };
//         options.Events = new JwtBearerEvents
//         {
//             OnMessageReceived = context =>
//             {
//                 string authHeader = context.Request.Headers["Authorization"].ToString();
//
//                 if (!string.IsNullOrEmpty(authHeader) &&
//                     !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
//                 {
//                     context.Request.Headers["Authorization"] = $"Bearer {authHeader}";
//                 }
//
//                 return Task.CompletedTask;
//             }
//         };
//     });
//
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("AdminOnly", policy =>
//         policy.RequireClaim("role", "admin"));

// IAuthorizationRequirement requirement = new ResourceAccessRequirement("Read");
//
//     options.AddPolicy("CustomerUser", policy =>
//         policy.RequireClaim("role", "customerUser"));
//
//     options.AddPolicy("CustomerAdmin", policy =>
//         policy.RequireClaim("role", "customerAdmin"));
//
//     options.AddPolicy("AdminOrManagerWithEditPermission", policy =>
//         policy.RequireRole("Admin", "Manager") // Check roles
//             .RequireClaim("Permission", "EditProject")); // Check additional conditions
//
//     // options.AddPolicy("Users:Read", policy =>
//     //     policy.Requirements.Add(new ResourceAccessRequirement("Read")));
// });
//
// builder.Services.AddControllers(options =>
// {
//     // options.Conventions.Add(new CustomRouteConvention());
// });
//
// WebApplication app = builder.Build();
//
// app.Use(async (context, next) =>
// {
//     Console.WriteLine("Request started: " + context.Request.Path);
//     await next();
//     Console.WriteLine("Request ended");
// });
//
// app.UseStaticFiles(); // Enables serving static files
//
// app.UseSwagger();
// app.UseSwaggerUI(c =>
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Appblueprint API V1");
//     c.RoutePrefix = string.Empty; // Swagger at root (optional)
// });

// app.MapOpenApi();

// app.Use(async (context, next) =>
// {
//     Console.WriteLine("Request started: " + context.Request.Path);
//     await next();
//     Console.WriteLine("Request ended");
// });
//
//
// // Configure the HTTP request pipeline.
// app.UseExceptionHandler();
//

//
// app.MapScalarApiReference(options =>
// {
//     // Fluent API
//     options
//         .WithTitle("Appblueprint API")
//         // .WithPreferredScheme("OAuth2") // Security scheme name from the OpenAPI document
//         .WithTheme(Scalar.AspNetCore.ScalarTheme.Mars)
//         .WithSidebar(true);
//
//     // Object initializer            
//     options.EnabledClients = new[]
//     {
//         ScalarClient.RestSharp,
//         ScalarClient.HttpClient,
//         ScalarClient.Curl
//     };
//
//     options.WithPreferredScheme("bearerAuth");
//     options.WithHttpBearerAuthentication(bearer => { bearer.Token = "Bearer {token}"; });
// });
//
// app.UseAuthentication();
// app.UseAuthorization();
//
// app.MapControllers();
//
// // app.MapHealthChecks("/health", new HealthCheckOptions
// // {
// //     ResponseWriter = HealthCheckResponseWriter.WriteResponse
// // });
//
// // app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
// //     .WithName("HealthCheck")
// //     .Producespublic class SqlHealthCheck: IHealthCheck
//
// // public SqlHealthCheck(ApplicationDBContext context)
// // {
// //     CheckHealthAsync();
// // }
// //
// // public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
// //     CancellationToken cancellationToken = new CancellationToken())
// // {
// //     try
// //     {
// //         await _context.Database.ExecuteSqlRawAsync("SELECT 1");
// //         return HealthCheckResult.Healthy();
// //     }
// //     catch (Exception e)
// //     {
// //         return HealthCheckResult.Unhealthy(e.Message);
// //     }
// // }



// DISCARDED



