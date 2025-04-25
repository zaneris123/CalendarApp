using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://localhost:50068") // Corrected to HTTPS
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });

            // Ensure DbContext is registered with a scoped lifetime
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowSpecificOrigin"); 

            app.UseAuthorization();

            // User endpoints

            // Create a new user
            app.MapPost("/createusers", async (UserDto inputtedUser, AppDbContext dbContext) =>
            {
                // Check if the user already exists
                bool userExists = dbContext.Users.Any(u => u.Name == inputtedUser.Name);
                if (userExists)
                {
                    return Results.Conflict("User already exists.");
                }
                
                // Validate the inputted user data
                var newUser = new User // Use the User type from CalendarApp.Server.Models
                {
                    Name = inputtedUser.Name,
                    CreatedAt = DateTime.UtcNow
                };

                // Add the new user to the database
                dbContext.Users.Add(newUser);

                // Save changes to the database
                await dbContext.SaveChangesAsync();
                return Results.Created($"/users/{newUser.Id}", newUser);
            })
            .WithName("CreateUser")
            .WithOpenApi();

            // Get all users
            app.MapGet("/users", (AppDbContext dbContext) =>
            {
                // Retrieve all users from the database
                var users = dbContext.Users.ToList();
                return Results.Ok(users);
            })
            .WithName("GetUsers")
            .WithOpenApi();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
