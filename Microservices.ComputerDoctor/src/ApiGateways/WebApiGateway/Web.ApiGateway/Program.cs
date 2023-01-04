using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/*builder.Host.ConfigureAppConfiguration((host, config) =>
{
    config.SetBasePath(host.HostingEnvironment.ContentRootPath)
        .AddJsonFile("Configurations/ocelot.json")
        .AddEnvironmentVariables();
});*/
IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("Configurations/ocelot.json")
                            .Build();

builder.Services.AddOcelot(configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseOcelot().Wait();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
