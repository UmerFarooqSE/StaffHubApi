namespace StaffHubApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class EmployeesController : ControllerBase
{
    private readonly StaffHubDbContext _context;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICacheService _cache;

    public EmployeesController(
        StaffHubDbContext context,
        IEmployeeRepository employeeRepository,
        ICacheService cache)
    {
        _context = context;
        _employeeRepository = employeeRepository;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        const string cacheKey = "employees:all";

        var cached = await _cache.GetAsync<List<Employee>>(cacheKey);
        if (cached is not null)
            return Ok(cached);

        var employees = await _context.Employees
            .Include(e => e.Department)
            .OrderBy(e => e.FullName)
            .ToListAsync();

        await _cache.SetAsync(cacheKey, employees, TimeSpan.FromMinutes(5));
        return Ok(employees);
    }

    [HttpGet("summaries")]
    public async Task<IActionResult> GetSummaries()
    {
        const string cacheKey = "employees:summaries";

        var cached = await _cache.GetAsync<IEnumerable<EmployeeSummary>>(cacheKey);
        if (cached is not null)
            return Ok(cached);

        var summaries = await _employeeRepository.GetEmployeeSummariesAsync();
        await _cache.SetAsync(cacheKey, summaries, TimeSpan.FromMinutes(5));
        return Ok(summaries);
    }

    [HttpGet("{id:guid}/detail")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var cacheKey = $"employees:{id}:detail";

        var cached = await _cache.GetAsync<EmployeeDetail>(cacheKey);
        if (cached is not null)
            return Ok(cached);

        var detail = await _employeeRepository.GetEmployeeDetailAsync(id);

        if (detail is null)
            return NotFound();

        await _cache.SetAsync(cacheKey, detail, TimeSpan.FromMinutes(5));
        return Ok(detail);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var cacheKey = $"employees:{id}";

        var cached = await _cache.GetAsync<Employee>(cacheKey);
        if (cached is not null)
            return Ok(cached);

        var employee = await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee is null)
            return NotFound();

        await _cache.SetAsync(cacheKey, employee, TimeSpan.FromMinutes(5));
        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == request.DepartmentId);
        if (!departmentExists)
            return BadRequest("Department not found.");

        var employee = new Employee
        {
            FullName = request.FullName,
            Email = request.Email,
            JobTitle = request.JobTitle,
            HireDate = request.HireDate,
            DepartmentId = request.DepartmentId,
            IsActive = true
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        await _cache.RemoveByPrefixAsync("employees:");
        await _cache.RemoveByPrefixAsync("departments:");

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
            return NotFound();

        employee.FullName = request.FullName;
        employee.Email = request.Email;
        employee.JobTitle = request.JobTitle;
        employee.IsActive = request.IsActive;
        employee.DepartmentId = request.DepartmentId;

        await _context.SaveChangesAsync();

        await _cache.RemoveByPrefixAsync("employees:");
        await _cache.RemoveByPrefixAsync("departments:");

        return Ok(employee);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
            return NotFound();

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        await _cache.RemoveByPrefixAsync("employees:");
        await _cache.RemoveByPrefixAsync("departments:");

        return NoContent();
    }
}

public record CreateEmployeeRequest(
    string FullName,
    string Email,
    string JobTitle,
    DateTime HireDate,
    Guid DepartmentId);

public record UpdateEmployeeRequest(
    string FullName,
    string Email,
    string JobTitle,
    bool IsActive,
    Guid DepartmentId);