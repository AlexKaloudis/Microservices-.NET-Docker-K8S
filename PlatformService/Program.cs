using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;
using Grpc.AspNetCore.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPlatformRepo,PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient,HttpCommandDataClient>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<IMessageBusClient,MessageBusClient>();
builder.Services.AddGrpc();
builder.WebHost.UseKestrel();
Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");
if(builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using sql server db");
    builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConnection")));
    Console.WriteLine("info "+builder.Configuration.GetConnectionString("PlatformsConnection"));
}else{
    Console.WriteLine("--> Using in memory Db");
    builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseInMemoryDatabase("InMem"));
}
var app = builder.Build();

// Configure the HTTP request pipeline.


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
// app.UseHttpsRedirection();
app.UseGrpcWeb();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<GrpcPlatformService>().EnableGrpcWeb();

                endpoints.MapGet("/protos/platforms.proto", async context =>
                {
                    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
                });
            });
app.MapControllers();

PrepDb.PrepPopulation(app,app.Environment.IsProduction());

app.Run();
