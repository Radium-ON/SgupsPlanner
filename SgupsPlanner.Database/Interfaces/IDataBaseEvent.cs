using System.Threading.Tasks;

namespace SgupsPlanner.Database.Interfaces
{
    public interface IDataBaseEvent<T, TV> : IDatabase<T, TV>
    {
        Task<TV> CreateAsync(T newObject);
    }
}
