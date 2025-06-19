using Business.Implementations;
using Business.Interfaces;
using DataAccess.Implementations;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<MyDbContext>(opt =>
                opt.UseNpgsql(
                    builder.Configuration.GetConnectionString("MyVillageDb")
                )
            );
            // Add services to the container.
            builder.Services.AddTransient<IUsersDataAccess, UsersDataAccess>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

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