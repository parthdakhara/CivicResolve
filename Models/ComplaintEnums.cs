namespace CivicResolve.Models
{
    public enum ComplaintPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum ComplaintStatus
    {
        Submitted = 1,
        UnderReview = 2,
        Assigned = 3,
        InProgress = 4,
        Resolved = 5,
        Rejected = 6
    }
}
