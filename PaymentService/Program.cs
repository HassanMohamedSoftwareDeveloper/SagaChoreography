using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Consumers;
using PaymentService.Database;
using Serilog;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, configuration) => configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services));


    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<PaymentDbContext>(config => config.UseSqlServer(builder.Configuration.GetConnectionString("PaymentDbConnection"),
        options => options.MigrationsHistoryTable("PaymentMigrations", "PAYMENTS")));

    builder.Services.AddMassTransit(busConfigure =>
    {
        busConfigure.SetKebabCaseEndpointNameFormatter();

        busConfigure.AddConsumer<InventoryReservedConsumer>();

        busConfigure.UsingRabbitMq((context, configurator) =>
        {
            configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), host =>
            {
                host.Username(builder.Configuration["MessageBroker:Username"]);
                host.Password(builder.Configuration["MessageBroker:Password"]);
            });

            configurator.ConfigureEndpoints(context);
        });

    });
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    await app.PopulateDatabasePreparation();
    app.Run();
    return 0;

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}