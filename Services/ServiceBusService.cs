namespace StaffHubApi.Services;

public class ServiceBusService : IServiceBusService, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly ILogger<ServiceBusService> _logger;

    public ServiceBusService(IConfiguration configuration, ILogger<ServiceBusService> logger)
    {
        _logger = logger;
        var connectionString = configuration["ServiceBus:ConnectionString"];
        var queueName = configuration["ServiceBus__QueueName"] ?? "employee-created";

        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(queueName);
    }

    public async Task SendEmployeeCreatedMessageAsync(Guid employeeId, string fullName, string email)
    {
        try
        {
            var messageBody = JsonSerializer.Serialize(new
            {
                EmployeeId = employeeId,
                FullName = fullName,
                Email = email,
                CreatedAt = DateTime.UtcNow,
                EventType = "EmployeeCreated"
            });

            var message = new ServiceBusMessage(messageBody)
            {
                ContentType = "application/json",
                Subject = "EmployeeCreated",
                MessageId = Guid.NewGuid().ToString()
            };

            await _sender.SendMessageAsync(message);

            _logger.LogInformation(
                "Service Bus message sent for employee {EmployeeId}: {FullName}",
                employeeId, fullName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send Service Bus message for employee {EmployeeId}",
                employeeId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}