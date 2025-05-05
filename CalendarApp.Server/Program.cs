using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://localhost:50068") // Corrected to HTTPS
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });

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

            // User endpoints

            // Create a new user
            app.MapPost("/users", async (UserDto inputtedUser, AppDbContext dbContext) =>
            {
                // Validate the inputted user data
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(inputtedUser);
                if (!Validator.TryValidateObject(inputtedUser, validationContext, validationResults, true))
                {
                    return Results.BadRequest(validationResults);
                }

                // Check if the user already exists
                bool userExists = dbContext.Users.Any(u => u.Name == inputtedUser.Name);
                if (userExists)
                {
                    return Results.Conflict("User already exists.");
                }
                
                var newUser = new User
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

            // Create a new event
            app.MapPost("/events", async (EventDto inputtedEvent, AppDbContext dbContext) =>
            {
                // Validate the inputted event data
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(inputtedEvent);
                if (!Validator.TryValidateObject(inputtedEvent, validationContext, validationResults, true))
                {
                    return Results.BadRequest(validationResults);
                }

                // Check if the user exists
                bool userExists = dbContext.Users.Any(u => u.Id == inputtedEvent.UserId);
                if (!userExists)
                {
                    return Results.NotFound("User not found.");
                }

                // Check if dates are on the same day
                if (inputtedEvent.StartDate.Date != inputtedEvent.EndDate.Date)
                {
                    return Results.BadRequest("Start and end dates must be on the same day.");
                }

                // check if the end date is after the start date
                if (inputtedEvent.EndDate <= inputtedEvent.StartDate)
                {
                    return Results.BadRequest("End date must be after start date.");
                }

                // Check if event overlaps with existing events for the user
                bool eventOverlap = dbContext.Events.Any(e =>
                    e.UserId == inputtedEvent.UserId &&
                    ((inputtedEvent.StartDate >= e.StartDate && inputtedEvent.StartDate <= e.EndDate) ||
                     (inputtedEvent.EndDate >= e.StartDate && inputtedEvent.EndDate <= e.EndDate)));
                if (eventOverlap)
                {
                    return Results.Conflict("Event overlaps with an existing event.");
                }

                var newEvent = new Event
                {
                    Title = inputtedEvent.Title,
                    Description = inputtedEvent.Description,
                    StartDate = inputtedEvent.StartDate,
                    EndDate = inputtedEvent.EndDate,
                    UserId = inputtedEvent.UserId
                };

                // Add the new event to the database
                dbContext.Events.Add(newEvent);

                // Save changes to the database
                await dbContext.SaveChangesAsync();
                return Results.Created($"/events/{newEvent.Id}", newEvent);
            })
            .WithName("CreateEvent")
            .WithOpenApi();

            // Get all events for a user
            app.MapGet("/events/{userId}", (Guid userId, AppDbContext dbContext) =>
            {
                // Check if the user exists
                bool userExists = dbContext.Users.Any(u => u.Id == userId);
                if (!userExists)
                {
                    return Results.NotFound("User not found.");
                }

                var events = dbContext.Events.Where(e => e.UserId == userId).ToList();
                return Results.Ok(events);
            })
            .WithName("GetEvents")
            .WithOpenApi();

            //delete an event
            app.MapDelete("/events/{eventId}", async (Guid eventId, AppDbContext dbContext) =>
            {
                var eventToDelete = await dbContext.Events.FindAsync(eventId);
                if (eventToDelete == null)
                {
                    return Results.NotFound("Event not found.");
                }

                dbContext.Events.Remove(eventToDelete);
                await dbContext.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithName("DeleteEvent")
            .WithOpenApi();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
