namespace StaffHubApi.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly string _connectionString;

    public EmployeeRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public async Task<IEnumerable<EmployeeSummary>> GetEmployeeSummariesAsync()
    {
        const string sql = """
            SELECT 
                e.id,
                e.full_name,
                e.job_title,
                d.name AS department_name,
                e.is_active
            FROM employees e
            INNER JOIN departments d ON d.id = e.department_id
            ORDER BY e.full_name
            """;

        using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<EmployeeSummary>(sql);
    }

    public async Task<EmployeeDetail?> GetEmployeeDetailAsync(Guid id)
    {
        const string sql = """
            SELECT 
                e.id,
                e.full_name,
                e.email,
                e.job_title,
                e.hire_date,
                e.is_active,
                d.name AS department_name,
                COUNT(colleagues.id) AS total_colleagues
            FROM employees e
            INNER JOIN departments d ON d.id = e.department_id
            LEFT JOIN employees colleagues ON colleagues.department_id = e.department_id 
                AND colleagues.id != e.id
            WHERE e.id = @Id
            GROUP BY e.id, e.full_name, e.email, e.job_title, 
                     e.hire_date, e.is_active, d.name
            """;

        using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<EmployeeDetail>(sql, new { Id = id });
    }
}