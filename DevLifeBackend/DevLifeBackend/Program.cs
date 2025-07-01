// DevLife.Api/Program.cs
using DevLife.Api.Data; // For AppDbContext and MongoDbContext
using DevLife.Api.Services; // For AuthService, UserService, TokenService
using DevLife.Domain.Interfaces; // For IUserRepository
using DevLife.Infrastructure.Repositories; // For UserRepository
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore; // For Entity Framework Core (PostgreSQL)
using Microsoft.IdentityModel.Tokens; // For TokenValidationParameters
using Microsoft.OpenApi.Models; // For Swagger/OpenAPI
using StackExchange.Redis; // For Redis (even if CacheService isn't fully built yet, the connection can be there)
using System.Text; // For Encoding.UTF8


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:80");

// --- 1. სერვისების დამატება Dependency Injection კონტეინერში ---

// 1.1. მონაცემთა ბაზის კონტექსტები და რეპოზიტორები
// PostgreSQL - Entity Framework Core
var postgresConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(postgresConnectionString))
{
    // თუ კავშირის სტრიქონი არ მოიძებნა, აპლიკაცია არ უნდა გაეშვას
    throw new InvalidOperationException("PostgreSQL connection string 'DefaultConnection' not found in configuration.");
}
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

// MongoDB
// MongoDbContext დარეგისტრირებულია როგორც Singleton, რადგან ის მართავს MongoClient-ის კავშირებს
builder.Services.AddSingleton<MongoDbContext>();

// რეპოზიტორების რეგისტრაცია (ამჟამად მხოლოდ UserRepository)
builder.Services.AddScoped<IUserRepository, UserRepository>();
// თუ გაქვთ სხვა რეპოზიტორები, დაამატეთ აქ:
// builder.Services.AddScoped<IDatingProfileRepository, DatingProfileRepository>();
// builder.Services.AddScoped<ICodeSnippetRepository, CodeSnippetRepository>();


// 1.2. აპლიკაციის სერვისები
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<HoroscopeService>();

// თუ გაქვთ სხვა სერვისები, დაამატეთ აქ:
// builder.Services.AddScoped<CodeRoastService>();
// builder.Services.AddScoped<DevDatingService>();
// builder.Services.AddScoped<ICacheService, RedisCacheService>(); // თუ RedisCacheService გაქვთ


// 1.3. Redis-ის კონფიგურაცია (ქეშირებისთვის)
var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
if (string.IsNullOrEmpty(redisConnectionString))
{
    // შეგიძლიათ აქ გადაწყვიტოთ, აუცილებელია თუ არა Redis.
    // თუ აუცილებელია, throw InvalidOperationException. თუ არა, უბრალოდ არ დაამატოთ.
    // ამ მაგალითში, თუ არ არის, შეცდომას არ ვაგდებთ, მაგრამ Log-ში ვაფიქსირებთ.
    builder.Logging.AddConsole().AddDebug(); // Ensure logging is configured for this warning
    builder.Logging.Services.BuildServiceProvider().GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>()
           .LogWarning("Redis connection string 'RedisConnection' not found. Redis caching will not be available.");
}
else
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect(redisConnectionString));
    // თუ გაქვთ RedisCacheService, დაამატეთ აქ:
    // builder.Services.AddScoped<ICacheService, RedisCacheService>();
}


// 1.4. JWT ავთენტიფიკაციის კონფიგურაცია
var jwtSettings = builder.Configuration.GetSection("Jwt");
// აუცილებელია, რომ Key, Issuer და Audience არსებობდეს appsettings.json-ში
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key (Jwt:Key) not found in configuration."));
var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer (Jwt:Issuer) not found in configuration.");
var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience (Jwt:Audience) not found in configuration.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // ტოკენის ვადის გასვლის დაყოვნების მოსაშორებლად
    };
});

// 1.5. ავტორიზაცია
builder.Services.AddAuthorization();

// 1.6. CORS პოლიტიკის კონფიგურაცია (Frontend-თან კომუნიკაციისთვის)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder => policyBuilder.WithOrigins(
                                "http://localhost:3000", // თქვენი React/frontend-ის დეველოპერული სერვერი
                                "http://localhost:5173"  // სხვა გავრცელებული frontend-ის დეველოპერული პორტი (Vite-ის სტანდარტული)
                                                         // "http://yourproductionfrontend.com" // თქვენი Production-ის frontend URL-ები
                            )
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()); // საჭიროა SignalR-ისთვის და ავთენტიფიცირებული მოთხოვნებისთვის
});

// 1.7. კონტროლერები
builder.Services.AddControllers();

// 1.8. Swagger/OpenAPI კონფიგურაცია (API დოკუმენტაციისთვის)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DevLife API", Version = "v1" });

    // JWT ავთენტიფიკაციის მხარდაჭერა Swagger UI-ში
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            Array.Empty<string>() // Scope-ის მოთხოვნები
        }
    });
});

// 1.9. SignalR-ის დამატება (თუ გამოიყენებთ რეალურ დროში კომუნიკაციისთვის)
builder.Services.AddSignalR();


// --- 2. აპლიკაციის აწყობა ---
var app = builder.Build();

// --- 3. HTTP მოთხოვნების პაიპლაინის კონფიგურაცია ---

// 3.1. დეველოპერული გარემოს კონფიგურაცია
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // დეტალური შეცდომის გვერდები დეველოპმენტში
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevLife API V1");
        // c.RoutePrefix = string.Empty; // თუ გსურთ Swagger UI იყოს root-ზე
    });
}
else
{
    // 3.2. Production გარემოს კონფიგურაცია
    app.UseExceptionHandler("/Error"); // ზოგადი შეცდომის გვერდი Production-ისთვის
    app.UseHsts(); // HTTPS-ის უზრუნველყოფა კლიენტებისთვის (HTTP Strict Transport Security)
}

app.UseHttpsRedirection(); // HTTP მოთხოვნების გადამისამართება HTTPS-ზე
app.UseRouting(); // Endpoint Routing-ის ჩართვა (უნდა იყოს UseCors, UseAuthentication, UseAuthorization-მდე)

app.UseCors("AllowSpecificOrigin"); // CORS პოლიტიკის გამოყენება

app.UseAuthentication(); // უნდა იყოს UseAuthorization-მდე, ამატებს ავთენტიფიკაციის შესაძლებლობებს
app.UseAuthorization();  // უნდა იყოს UseAuthentication-ის შემდეგ, ამოწმებს მომხმარებლის უფლებებს

// 3.3. WebSocket კონფიგურაცია (SignalR-ისთვის, თუ WebSockets-ს იყენებთ)
// ეს საჭიროა, თუ SignalR იყენებს WebSockets-ს, რაც სტანდარტულია.
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
    ReceiveBufferSize = 4 * 1024
};
app.UseWebSockets(webSocketOptions);


// 3.4. Endpoint-ების Mapping
app.MapControllers(); // აკავშირებს თქვენი API კონტროლერების მარშრუტებს

// SignalR Hub-ების Mapping
// თუ NotificationHub გაქვთ შექმნილი DevLife.Api/Hubs/NotificationHub.cs-ში
// app.MapHub<NotificationHub>("/notifications"); // მოხსენით კომენტარი, როდესაც შექმნით Hub-ს


// --- 4. აპლიკაციის გაშვება ---
app.Run();