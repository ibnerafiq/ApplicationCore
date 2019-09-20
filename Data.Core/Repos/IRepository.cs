using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Data.Core.Repos
{
    public interface IRepository<T> where T : class
    {
        void Insert(T Entity);
        void Insert(IEnumerable<T> Entity);
        void Update(T Entity);
        void Delete(T Entity);
        void Delete(object id);
        void Attach(T Entity);
        void RunSQLQuery(string querySQL);
        List<T> SQLQuery(string querySQL);
        /// <summary>
        /// This will be use for ad-hoc queryies
        /// </summary>
        /// <param name="querySQL"> your raw query/stored procedure</param>
        /// <param name="includingProperties">nvaigation properties for mapping</param>
        /// <param name="parameters">parameters to pass the query</param>
        /// <returns></returns>
        List<T> ExecuteStoredProcedure(string querySQL, string includingProperties = null, params SqlParameter[] parameters);

        object ExecuteSPObject<U>(string querySQL, params SqlParameter[] parameters);
        DbCommand MultipleResultSet(string spName, params SqlParameter[] parameters);
        List<T> SQLQuery(string querySQL, bool customObject);
        T GetById(object Id);

        // MultiResultSetReader GetMultiResultSet(string query, params SqlParameter[] parameters);


        IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
            string includingProperties = null, bool noTracking = true);
        List<T> GetAllGroup(Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
        string includingProperties = null, bool noTracking = true,
            string[] groupBy = null);

        #region MappingAndTracking
        IQueryable<U> GetAllAutoMapper<U>(Expression<Func<T, bool>> filter = null,
     string includingProperties = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, bool noTracking = true
      ) where U : class;

        #endregion

        #region AutoMappingNoTracking
        //    List<U> GetAllAutoMapperNoTracking<U>(Expression<Func<T, bool>> filter = null,
        // string includingProperties = null, Expression<Func<T, int>> orderBy = null
        //  ) where U : class;
        //    List<U> GetAllAutoMapperNoTracking<U>(Expression<Func<T, bool>> filter = null,
        //  string includingProperties = null, Expression<Func<T, string>> orderBy = null
        //   ) where U : class;

        //    List<U> GetAllAutoMapperNoTracking<U>(Expression<Func<T, bool>> filter = null,
        //string includingProperties = null, Expression<Func<T, long>> orderBy = null
        // ) where U : class;
        #endregion

    }
}
