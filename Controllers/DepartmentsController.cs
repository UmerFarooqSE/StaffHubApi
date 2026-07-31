namespace StaffHubApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly StaffHubDbContext _context;

    public DepartmentsController(StaffHubDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _context.Departments
            .OrderBy(d => d.Name)
            .ToListAsync();
        return Ok(departments);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var department = await _context.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department is null)
            return NotFound();

        return Ok(department);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        var department = new Department
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentRequest request)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department is null)
            return NotFound();

        department.Name = request.Name;
        department.Description = request.Description;

        await _context.SaveChangesAsync();
        return Ok(department);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department is null)
            return NotFound();

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public record CreateDepartmentRequest(string Name, string? Description);
public record UpdateDepartmentRequest(string Name, string? Description);