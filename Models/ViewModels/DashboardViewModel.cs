namespace CivicResolve.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalComplaints { get; set; }
        public int PendingComplaints { get; set; }
        public int InProgressComplaints { get; set; }
        public int ResolvedComplaints { get; set; }
        public int RejectedComplaints { get; set; }
        public int OverdueComplaints { get; set; }
        public int CriticalComplaints { get; set; }
        public double ResolutionRate { get; set; }
        public double AverageSatisfactionRating { get; set; }

        public List<DepartmentStatItem> DepartmentStats { get; set; } = new List<DepartmentStatItem>();
        public List<Complaint> RecentComplaints { get; set; } = new List<Complaint>();
        public List<Complaint> OverdueList { get; set; } = new List<Complaint>();
    }

    public class DepartmentStatItem
    {
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Resolved { get; set; }
        public int Pending { get; set; }
        public double PercentageResolved => Total > 0 ? Math.Round((double)Resolved / Total * 100, 1) : 0;
    }
}
