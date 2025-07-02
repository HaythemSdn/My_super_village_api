using Business.Implementations;
using Business.Interfaces;
using DataAccess.Implementations;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

public class Program
{
    public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration
                .AddUserSecrets<Program>(true)
                .Build();
            builder.Services.AddDbContext<MyDbContext>(opt =>
                opt.UseNpgsql(
                    builder.Configuration.GetConnectionString("MyVillageDb")
                )
            );
            // Add services to the container.
            builder.Services.AddTransient<IUsersDataAccess, UsersDataAccess>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IUserRessourceDataAccess, UserRessourceDataAccess>();
            builder.Services.AddTransient<IUserRessourceService, UserRessourceService>();
            builder.Services.AddTransient<IUserBuildingDataAccess, UserBuildingDataAccess>();
            builder.Services.AddTransient<IUserBuildingService, UserBuildingService>();
            builder.Services.AddTransient<IUserConstructionDataAccess, UserConstructionDataAccess>();
            builder.Services.AddTransient<IUserConstructionService, UserConstructionService>();
            
            //Add Controllers to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddCors(options => {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy => {
                        policy
                            .WithOrigins("http://localhost:5173")
                            //.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthorization();

            app.MapControllers();
            app.UseSwaggerUI(options =>
            {
                options.ConfigObject.Urls =
                [
                    new UrlDescriptor
                    {
                        Name = "My super village api",
                        Url = "/openapi/v1.json"
                    }
                ];
            });
            using (var scope = app.Services.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

                // Here is the migration executed
                dbContext.Database.Migrate();
            }
            app.Run();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            throw;
        }
    }
}