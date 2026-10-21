namespace CivicResolve.Models.ViewModels
{
    public class ComplaintFilterViewModel
    {
        public string? SearchTerm { get; set; }
        public int? DepartmentId { get; set; }
        public ComplaintStatus? Status { get; set; }
        public ComplaintPriority? Priority { get; set; }
        public int? WardNumber { get; set; }
        public bool? OnlyOverdue { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public List<Complaint> Complaints { get; set; } = new List<Complaint>();
        public List<Department> Departments { get; set; } = new List<Department>();
    }
}
