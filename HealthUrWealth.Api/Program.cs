using HealthUrWealth.Infrastructure.Addresses;
using HealthUrWealth.Infrastructure.Authentication.Jwt;
using HealthUrWealth.Infrastructure.Authentication.Repositories;
using HealthUrWealth.Infrastructure.BlueDart;
using HealthUrWealth.Infrastructure.Cart;
using HealthUrWealth.Infrastructure.Common;
using HealthUrWealth.Infrastructure.Common.Caching;
using HealthUrWealth.Infrastructure.Discovery.Read;
using HealthUrWealth.Infrastructure.Notifications;
using HealthUrWealth.Infrastructure.Notifications.Email;
using HealthUrWealth.Infrastructure.Notifications.Sms;
using HealthUrWealth.Infrastructure.Notifications.Templates;
using HealthUrWealth.Infrastructure.Orders;
using HealthUrWealth.Infrastructure.Payments;
using HealthUrWealth.Infrastructure.Payments.PayU;
using HealthUrWealth.Infrastructure.Persistence.Repositories;
using HealthUrWealth.Infrastructure.Users;
using HealthUrWelath.Application.Addresses.Interfaces;
using HealthUrWelath.Application.Authentication.Interfaces;
using HealthUrWelath.Application.BlueDart.Dtos;
using HealthUrWelath.Application.BlueDart.Interfaces;
using HealthUrWelath.Application.Cart.Interfaces;
using HealthUrWelath.Application.Checkout.Interfaces;
using HealthUrWelath.Application.Common.Caching;
using HealthUrWelath.Application.Common.CDN;
using HealthUrWelath.Application.Common.Interfaces;
using HealthUrWelath.Application.Home.Interfaces;
using HealthUrWelath.Application.Impersonate.Interfaces;
using HealthUrWelath.Application.Notifications.Interfaces;
using HealthUrWelath.Application.Orders.Interfaces;
using HealthUrWelath.Application.Payments.Interfaces;
using HealthUrWelath.Application.Users.Interfaces;
using HealthUrWelath.Infrastructure.Notifications.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Host.UseSerilog((ctx, lc) =>
//{
//    lc.MinimumLevel.Error() // 🔥 log only exceptions
//      .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
//      //.MinimumLevel.Override("System", LogEventLevel.Warning)
//      .Enrich.FromLogContext()
//      .WriteTo.File(
//          path: "logs/log-.txt",
//          rollingInterval: RollingInterval.Day,
//          retainedFileCountLimit: 30, // 🔥 keep only 30 days
//          shared: true
//      );
//});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(HealthUrWelath.Application.AssemblyReference).Assembly));

//builder.Services.AddValidatorsFromAssembly(
//    typeof(HealthUrWelath.Application.AssemblyReference).Assembly);

//builder.Services.AddTransient(
//    typeof(IPipelineBehavior<,>),
//    typeof(ValidationBehavior<,>));


//builder.Services.AddFluentValidationAutoValidation();


//DB 
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.SuppressModelStateInvalidFilter = true;
//});

//Repo DI
builder.Services.AddScoped<IDiscoveryReadRepository, DiscoveryReadRepository>();
builder.Services.AddScoped<INavigationReadRepository, NavigationReadRepository>();

builder.Services.AddScoped<IProductReadRepository, ProductReadRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<IOtpRepository, OtpRepository>(); 

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenFactory, JwtTokenFactory>();
builder.Services.AddScoped<IUserContext, UserContext>();

builder.Services.AddScoped<ICurrencyContext, CurrencyContext>();

builder.Services.AddScoped<IImpersonationAuditRepository, ImpersonationAuditRepository>();

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<ITemplateRenderer, TemplateRenderer>(); 

builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<IAddressRepository, AddressRepository>();

builder.Services.AddScoped<ICheckoutRepository, CheckoutRepository>();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IPayUService, PayUService>();

builder.Services.AddHttpClient<IBunnyCdnStorageService, HealthUrWealth.Infrastructure.Common.CDN.BunnyCdnStorageService>();

builder.Services.AddHttpClient<ISmsService, SmsService>();

builder.Services.AddHttpClient("BrevoClient", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["BrevoEmail:BaseUrl"];

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("accept", "application/json");
});

builder.Services.Configure<BluedartSettings>(
    builder.Configuration.GetSection("Bluedart"));

builder.Services.AddHttpClient<IBluedartService, BluedartService>();

builder.Services.AddScoped<IEmailService, EmailService>();


//Cache 
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IAppCache, MemoryCacheService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "HealthUrWealth API",
        Version = "v1"
    });
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException("JWT Key is missing in configuration");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            // 🔑 IMPORTANT
            NameClaimType = JwtRegisteredClaimNames.Sub,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


var allowedOrigins =
    builder.Configuration
           .GetSection("Cors:AllowedOrigins")
           .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("RestrictedCors", policy =>
    {
        policy
            .WithOrigins(allowedOrigins!)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});


var app = builder.Build();

//app.Use(async (context, next) =>
//{
//    var logPath = Path.Combine(
//        Directory.GetCurrentDirectory(),
//        "payu_hits.txt"
//    );

//    File.AppendAllText(
//        logPath,
//        $"{DateTime.Now} {context.Request.Method} {context.Request.Path} {context.Connection.RemoteIpAddress}{Environment.NewLine}"
//    );
//    await next();
//});


if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthUrWealth API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("RestrictedCors");

app.UseAuthentication(); 

app.UseAuthorization();

//app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
