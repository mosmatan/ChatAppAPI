using ChatAPI.Data;
using ChatAPI.Repositories;
using ChatAPI.Services;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        addDatebaseService(builder);

        addCors(builder);

        addScopes(builder);

        builder.Services.AddControllers();

        //for swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        //show swagger only in development environment
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty;
            });

        }

        app.UseCors("MyCors");

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
            dbContext.Database.Migrate();
        }

        app.Run();
    }

    private static void addDatebaseService(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ChatDbContext>(options => options.UseSqlServer(
            "Data Source=localhost,1433;" +
            "Initial Catalog=Chat;" +
            "User id=asMosmatan;" +
            "Password=1234!Secret;" +
            "Encrypt=false;"));
    }

    private static void addCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("MyCors", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });
    }

    private static void addScopes(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IContactRequestRepository, ContactRequestRepository>();
        builder.Services.AddScoped<IConverstionRepository, ConverstionRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<ConversationService>();
    }


}
