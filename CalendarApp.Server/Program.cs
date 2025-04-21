using System.Threading.Tasks;

namespace CalendarApp.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

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

            app.UseAuthorization();

            // Initialize the database
            await using var dbContext = new AppDbContext();
            await dbContext.Database.EnsureCreatedAsync();

            // User endpoints

            // Create a new user
            app.MapPost("/users", async (UserDto inputtedUser) =>
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

            app.MapGet("/users", () =>
            {
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
