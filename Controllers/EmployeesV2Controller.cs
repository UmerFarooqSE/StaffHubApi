namespace StaffHubApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/employees")]
[ApiVersion("2.0")]
public class EmployeesV2Controller : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICacheService _cache;

    public EmployeesV2Controller(
        IEmployeeRepository employeeRepository,
        ICacheService cache)
    {
        _employeeRepository = employeeRepository;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        const string cacheKey = "v2:employees:summaries";

        var cached = await _cache.GetAsync<IEnumerable<EmployeeSummary>>(cacheKey);
        if (cached is not null)
            return Ok(new
            {
                version = "2.0",
                count = cached.Count(),
                data = cached
            });

        var summaries = await _employeeRepository.GetEmployeeSummariesAsync();
        await _cache.SetAsync(cacheKey, summaries, TimeSpan.FromMinutes(5));

        return Ok(new
        {
            version = "2.0",
            count = summaries.Count(),
            data = summaries
        });
    }
}