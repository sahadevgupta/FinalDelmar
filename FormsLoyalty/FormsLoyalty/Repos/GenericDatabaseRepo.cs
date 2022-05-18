using FormsLoyalty.Helpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FormsLoyalty.Repos
{
    public class GenericDatabaseRepo<T> : IGenericDatabaseRepo<T> where T : new()
    {
        SQLiteAsyncConnection _sQLite;
        public GenericDatabaseRepo()
        {
            _sQLite = new SQLiteAsyncConnection(DbHelper.DatabasePath, DbHelper.Flags);
        }
        public async Task Insert(T a)
        {
           await _sQLite.InsertAsync(a);
        }

        public async Task InsertOrReplace(T a)
        {
           await _sQLite.InsertOrReplaceAsync(a);
        }
        public async Task InsertAll(IEnumerable<T> a)
        {
           await _sQLite.InsertAllAsync(a);
        }
        public async Task Update(T a)
        {
           await _sQLite.UpdateAsync(a);
        }

        public async Task UpdateAll(IEnumerable<T> a)
        {
            await _sQLite.UpdateAllAsync(a);
        }

        public async Task<List<T>> GetItemsAsync()
        {
            var data = await _sQLite.Table<T>().ToListAsync();
            return data;
        }

        public async Task Delete(string id)
        {
           await _sQLite.DeleteAsync<T>(id);
        }

    }
}
