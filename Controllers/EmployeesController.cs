namespace StaffHubApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly StaffHubDbContext _context;
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeesController(StaffHubDbContext context, IEmployeeRepository employeeRepository)
    {
        _context = context;
        _employeeRepository = employeeRepository;
    }

    // EF Core: simple list
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .OrderBy(e => e.FullName)
            .ToListAsync();
        return Ok(employees);
    }

    // Dapper: complex summary with department name joined
    [HttpGet("summaries")]
    public async Task<IActionResult> GetSummaries()
    {
        var summaries = await _employeeRepository.GetEmployeeSummariesAsync();
        return Ok(summaries);
    }

    // Dapper: detailed view with colleague count
    [HttpGet("{id:guid}/detail")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var detail = await _employeeRepository.GetEmployeeDetailAsync(id);

        if (detail is null)
            return NotFound();

        return Ok(detail);
    }

    // EF Core: get by ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee is null)
            return NotFound();

        return Ok(employee);
    }

    // EF Core: create
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

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    // EF Core: update
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
        return Ok(employee);
    }

    // EF Core: delete
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
            return NotFound();

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
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