namespace StaffHubApi.Dtos;

public class EmployeeSummary
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class EmployeeDetail
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int TotalColleagues { get; set; }
}