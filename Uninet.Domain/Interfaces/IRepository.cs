using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Entities;

namespace Uninet.Domain.Interfaces
{
    public interface IRepository<TDbContext>
    {
        //Task AddRange<T>(IEnumerable<T> entities) where T : class;
        
        Task<List<T>> GetAllAsync<T>() where T : class;

        Task<T> GetByIdAsync<T>(int id) where T : class;

        T GetById<T>(int id) where T : class;
        Task<T> CreateAsyncReturnEntity<T>(T entity) where T : class;
        Task DeleteAsync<T>(T entity) where T : class;
       Task CreateAsync<T>(T entity) where T : class;

        int Create<T>(T entity) where T : class;

        int GetLastInsertedId<T>(T entity) where T : class;

        int Update<T>(T entity) where T : class;

        Task UpdateAsync<T>(T entity) where T : class;

        Task DeleteAsync<T>(Expression<Func<T, bool>> filterExpression) where T : class;

        int Delete<T>(T entity) where T : class;

        IQueryable<T> Get<T>() where T : class;

        IQueryable<TRes> ExecuteGetSP<TRes>(string spName, object parameters = null) where TRes : class;

        Task<List<TRes>> ExecuteGetSPAsync<TRes>(string spName, object parameters = null) where TRes : class;

        Task<int> ExecuteSqlCommandAsync(string spName, object parameters = null);

        Task<T> FindAsync<T>(Expression<Func<T, bool>> expression) where T : class;

        // Task<T> FindAsync2<T>(Expression<Func<T, bool>> expression) where T : class;

        T Find<T>(Expression<Func<T, bool>> expression) where T : class;

        Task<T> GetFirstObjectAsync<T>(Expression<Func<T, bool>> filterExpression) where T : class;
        T GetFirstObject<T>(Expression<Func<T, bool>> filterExpression) where T : class;
        Task<List<T>> GetListOfObjectsAsync<T>(Expression<Func<T, bool>> filterExpression) where T : class;
        List<T> GetListOfObjects<T>(Expression<Func<T, bool>> filterExpression) where T : class;
        List<T> GetListOfObjectsPaging<T>(Expression<Func<T, bool>> filterExpression, int pageNumber, int pageSize) where T : class;
        Hashtable GetHashtableOfExternalFields(int Lang);
         List<ExternalSystemDynamicFields> GetListOfFiledObjects(int externalSystemId);
    }
}
