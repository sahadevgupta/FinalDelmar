using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormsLoyalty.Repos
{
    public interface IGenericDatabaseRepo<T>
    {
        Task Delete(string id);
        Task Insert(T a);
        Task InsertAll(IEnumerable<T> a);
        Task InsertOrReplace(T a);
        Task<List<T>> GetItemsAsync();
        Task Update(T a);
    }
}