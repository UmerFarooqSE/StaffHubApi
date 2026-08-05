namespace StaffHubApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class DepartmentsController : ControllerBase
{
    private readonly StaffHubDbContext _context;
    private readonly ICacheService _cache;

    public DepartmentsController(StaffHubDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        const string cacheKey = "departments:all";

        var cached = await _cache.GetAsync<List<Department>>(cacheKey);
        if (cached is not null)
            return Ok(cached);

        var departments = await _context.Departments
            .OrderBy(d => d.Name)
            .ToListAsync();

        await _cache.SetAsync(cacheKey, departments, TimeSpan.FromMinutes(10));
        return Ok(departments);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var cacheKey = $"departments:{id}";

        var cached = await _cache.GetAsync<Department>(cacheKey);
        if (cached is not null)
            return Ok(cached);

        var department = await _context.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department is null)
            return NotFound();

        await _cache.SetAsync(cacheKey, department, TimeSpan.FromMinutes(10));
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

        await _cache.RemoveByPrefixAsync("departments:");
        await _cache.RemoveByPrefixAsync("employees:");

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

        await _cache.RemoveByPrefixAsync("departments:");
        await _cache.RemoveByPrefixAsync("employees:");

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

        await _cache.RemoveByPrefixAsync("departments:");
        await _cache.RemoveByPrefixAsync("employees:");

        return NoContent();
    }
}

public record CreateDepartmentRequest(string Name, string? Description);
public record UpdateDepartmentRequest(string Name, string? Description);