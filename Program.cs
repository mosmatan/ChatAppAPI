using ChatAPI.Data;
using ChatAPI.Repositories;
using ChatAPI.Services;
using Microsoft.EntityFrameworkCore;
using ChatAPI.SampleDataScripts;

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


        Task.Run(RunSampleDataScript);

        app.Run();

        
    }

    private static void addDatebaseService(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ChatDbContext>(options => options.UseInMemoryDatabase("InMemoryDB"));
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

    private static async void RunSampleDataScript()
    {
        await Task.Delay(10000);
        SampleDataScript dataScript = new SampleDataScript();
        await dataScript.InitializeSampleData();
    }

}
