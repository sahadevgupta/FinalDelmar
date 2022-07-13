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
        SQLiteConnection _sQLite;
        public GenericDatabaseRepo()
        {
            _sQLite = new SQLiteConnection(DbHelper.DatabasePath, DbHelper.Flags);
        }
        public void Insert(T a)
        {
           _sQLite.Insert(a);
        }

        public void InsertOrReplace(T a)
        {
            _sQLite.InsertOrReplace(a);
        }
        public void InsertAll(IEnumerable<T> a)
        {
            _sQLite.InsertAll(a);
        }
        public void Update(T a)
        {
            _sQLite.Update(a);
        }

        public void UpdateAll(IEnumerable<T> a)
        {
             _sQLite.UpdateAll(a);
        }

        public List<T> GetItemsAsync()
        {
            var data =  _sQLite.Table<T>().ToList();
            return data;
        }

        public void Delete(string id)
        {
            _sQLite.Delete<T>(id);
        }

    }
}
