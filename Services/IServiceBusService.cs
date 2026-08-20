namespace StaffHubApi.Services;

public interface IServiceBusService
{
    Task SendEmployeeCreatedMessageAsync(Guid employeeId, string fullName, string email);
}