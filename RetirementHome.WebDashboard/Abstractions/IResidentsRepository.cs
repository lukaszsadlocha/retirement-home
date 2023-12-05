using RetirementHome.WebDashboard.Models;

namespace RetirementHome.WebDashboard.Abstractions
{
    public interface IResidentsRepository
    {
        Resident AddNewResident(string firstName, string lastName);
        public IEnumerable<Resident> GetResidents();
    }

    public class ResidentsRepository : IResidentsRepository
    {
        private List<Resident> _residents;

        public ResidentsRepository()
        {
            _residents = new List<Resident>()
            {
                new Resident() { FirstName = "Adam", LastName = "Atomic" },
                new Resident() { FirstName = "Barbara", LastName = "Bored"},
                new Resident() { FirstName = "Colin", LastName="Crazy"}
            };
        }

        public Resident AddNewResident(string firstName, string lastName)
        {
            var resident = new Resident { FirstName= firstName, LastName= lastName };
            _residents.Add(resident);
            return resident;
        }

        public IEnumerable<Resident> GetResidents()
        {
            return _residents;
        }
    }
}