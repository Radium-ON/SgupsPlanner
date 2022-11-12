using System.Collections.Generic;
using System.Threading.Tasks;
using SgupsPlanner.Database.Entities;

namespace SgupsPlanner.Database.Interfaces
{
    public interface IDataBaseFile<T, TV> : IDatabase<T, TV>
    {
        Task<List<T>> GetFilesByEventAsync(int idEvent, bool enableSubQuery = false);
        Task<TV> CreateAsync(int idEvent, T newObject);
    }
}
