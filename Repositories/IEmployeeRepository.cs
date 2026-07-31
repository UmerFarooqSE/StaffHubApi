namespace StaffHubApi.Repositories;

public interface IEmployeeRepository
{
    Task<IEnumerable<EmployeeSummary>> GetEmployeeSummariesAsync();
    Task<EmployeeDetail?> GetEmployeeDetailAsync(Guid id);
}