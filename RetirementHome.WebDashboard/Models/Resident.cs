namespace RetirementHome.WebDashboard.Models
{
    public class Resident
    {
        public Guid Id { get; } = Guid.NewGuid();
        public required string FirstName { get; init; } 
        public required string LastName { get; init; }
    }
}