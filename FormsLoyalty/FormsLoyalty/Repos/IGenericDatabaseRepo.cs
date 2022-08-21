using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormsLoyalty.Repos
{
    public interface IGenericDatabaseRepo<T>
    {
        void Delete(string id);
        void Insert(T a);
        void InsertAll(IEnumerable<T> a);
        void InsertOrReplace(T a);
        List<T> GetItemsAsync();
        void Update(T a);
        void UpdateAll(IEnumerable<T> a);
    }
}