namespace EmployeeManagement.Models
{
    public class DepartmentModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public override string ToString() => Name;
    }
}