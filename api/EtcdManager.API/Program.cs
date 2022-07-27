using System.Globalization;
using EtcdManager.API.Database;
using EtcdManager.API.Filters;
using EtcdManager.API.Repos;
using EtcdManager.API.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
.AddMvcOptions(options =>
            {
                options.Filters.Add(new AuthorizationAttribute());
            })
.AddNewtonsoftJson(x =>
{
    x.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    x.SerializerSettings.Culture = CultureInfo.DefaultThreadCurrentCulture!;
    x.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});

var connectionString = builder.Configuration.GetValue("ConnectionStrings:EtcdManager",
                                                    Path.Combine(builder.Environment.WebRootPath, "database", "etcd-manager.db"));

builder.Services.AddDbContext<EtcdDataContext>(options =>
{
    options.UseSqlite($"DataSource={connectionString};");
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<IKeyValueService, KeyValueService>();
builder.Services.AddScoped<IConnectionRepos, ConnectionRepos>();
builder.Services.AddScoped<IUserRepos, UserRepos>();
builder.Services.AddScoped<IAuthenRepos, AuthenRepos>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder.AllowAnyOrigin();
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// seed data
using (var scope = app.Services.CreateScope())
{
    var liteDb = scope.ServiceProvider.GetRequiredService<EtcdDataContext>();
    liteDb.SeedData();
}

app.Run();
