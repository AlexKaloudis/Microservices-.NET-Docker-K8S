using PlatformService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace PlatformService.Data
{
    public static class PrepDb
    {
            public static void PrepPopulation(IApplicationBuilder app,bool isProd)
            {
                using(var serviceScope = app.ApplicationServices.CreateScope())
                {
                    SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(),isProd);
                }

            }

            public static void SeedData(AppDbContext context,bool isProd)
            {
                if(isProd){
                    Console.WriteLine("--> attempting to apply migrations...");
                    try
                    {
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        
                        Console.WriteLine($"--> Could not apply migrations{ex.Message}");
                    } 
                    
                }
                if(!context.Platforms.Any())
                {
                    Console.WriteLine("--> Seeding data...");
                    context.Platforms.AddRange(
                        new Platform() {
                            Name = "Dot Net",
                            Publisher = "Microsoft",
                            Cost = "Free"
                        },
                        new Platform() {
                            Name = "SQL Server Express",
                            Publisher = "Microsoft",
                            Cost = "Free"
                        },
                        new Platform() {
                            Name = "Kubernetes",
                            Publisher = "Cloud Native Computing Foundation",
                            Cost = "Free"
                        }
                    );

                    context.SaveChanges();

                }else
                {
                    Console.WriteLine("--> We already have data");
                }
            }
    }
}