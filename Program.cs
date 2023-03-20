using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using RM.Api.Data;
using RM.Api.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

builder.Services.AddSingleton(builder.Configuration);
// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Load IdentityOptions from appsettings.json
//builder.Services.Configure<IdentityOptions>(builder.Configuration.GetSection("Identity"));
builder.Services.AddSingleton<IAppDbContextFactory>(sp =>
{
    var dbContextOptionsFactory = new Func<DbContextOptions<AppDbContext>>(() =>
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
        return optionsBuilder.Options;
    });

    return new AppDbContextFactory(dbContextOptionsFactory);
});
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "TokenScheme";
    options.DefaultChallengeScheme = "TokenScheme";
})
.AddScheme<AuthenticationSchemeOptions, TokenAuthenticationHandler>("TokenScheme", null);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(logging =>
{
    if (builder.Environment.IsDevelopment())
    {
        logging.AddConsole();
        logging.AddDebug();
    }
});

var app = builder.Build();

// Apply migrations
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
dbContext.Database.Migrate();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(pb =>
{
    pb.AllowAnyOrigin()
      .AllowAnyHeader()
      .AllowAnyMethod();
});

app.MapControllers();

app.Run();
