using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Core.Extension;
using Microsoft.EntityFrameworkCore;
using Snickler.EFCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Data.Core.Repos
{
    public class Repository<TEntity> : IRepository<TEntity>
  where TEntity : class
    {
        internal DbContext context;
        internal DbSet<TEntity> dbset;
        private readonly IMapper _mapper;
        internal DbQuery<TEntity> qSet;
        public Repository(DbContext context, IMapper mapper)
        {
            this.context = context;
            this.dbset = context.Set<TEntity>();
            //  this.qSet = context.Query<TEntity>();

            this._mapper = mapper;
        }


        public virtual DbCommand MultipleResultSet(string spName, params SqlParameter[] parameters)
        {
            // SprocResults result = null;
            DbCommand command = null;
            command = this.context.LoadStoredProc(spName);
            if (parameters.Length > 0)
            {
                foreach (var item in parameters)
                {
                    command = command.WithSqlParam(item.ParameterName, item);
                }
                // var command =  this.context.LoadStoredProc(spName).WithSqlParam()
            }


            return command;
        }

        public virtual List<TEntity> ExecuteStoredProcedure(string querySQL, string includingProperties = null, params SqlParameter[] parameters)
        {
            this.qSet = this.context.Query<TEntity>();

            if (includingProperties != null)
            {
                foreach (var includeproperty in includingProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    this.qSet.Include(includeproperty);
                }
            }
            context.Database.SetCommandTimeout(180000);
            if (parameters != null)
            {// for ad hoc data type  ------context.Query<TEntity>();
                var query = qSet.FromSql(querySQL, parameters).ToList();
                return query;
            }
            else
            {
                var query = dbset.FromSql(querySQL).ToList();
                return query.ToList();
            }

        }
        public object ExecuteSPObject<T>(string querySQL, params SqlParameter[] parameters)
        {
            context.Database.SetCommandTimeout(180000);

            if (parameters != null)
            {
                object query = dbset.FromSql(querySQL, parameters).FirstOrDefault();
                //object query = context.Database.SqlQuery<T>(querySQL, parameters).FirstOrDefault();
                return query;
            }
            else
            {
                object query = dbset.FromSql(querySQL).FirstOrDefault();
                // object query = context.Database.SqlQuery<T>(querySQL).FirstOrDefault();
                return query;
            }
        }
        public virtual List<TEntity> SQLQuery(string querySQL, bool customObject)
        {
            var query = dbset.FromSql(querySQL).ToList();
            return query;

        }
        public virtual List<TEntity> SQLQuery(string querySQL)
        {

            var query = dbset.FromSql(querySQL).ToList();
            return query;
        }

        public virtual void RunSQLQuery(string querySQL)
        {

            context.Database.SetCommandTimeout(180000);
            context.Database.ExecuteSqlCommand(querySQL);

        }

        public virtual IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null,

            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderyby = null,
            string includingProperties = null, bool noTracking = true
            )
        {

            IQueryable<TEntity> query = dbset;

            if (includingProperties != null)
            {
                foreach (var includeproperty in includingProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeproperty);
                }
            }

            if (filter != null)
            {
                query = query.Where(filter);
                // query = query.Where(tt => !(bo999ol)tt.GetType().GetProperty("IsDeleted").GetValue(tt, null));
            }



            if (orderyby != null)
            {
                if (noTracking)
                {
                    return orderyby(query).AsNoTracking();
                }
                return orderyby(query);
            }

            if (noTracking)
            {
                return query.AsNoTracking();
            }
            return query;
        }






        #region MappingAndTracking
        public virtual IQueryable<U> GetAllAutoMapper<U>(Expression<Func<TEntity, bool>> filter = null,
        string includingProperties = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null, bool noTracking = true
        ) where U : class
        {
            IQueryable<TEntity> query = dbset;
            if (includingProperties != null)
            {
                foreach (var includeproperty in includingProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeproperty);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);

            }





            if (orderby != null)
            {
                if (noTracking)
                {

                    //return orderby(query).AsNoTracking().ProjectTo<U>(_mapper.ConfigurationProvider);
                    return orderby(query).AsNoTracking().ProjectTo<U>(_mapper.ConfigurationProvider);
                }
                return orderby(query).ProjectTo<U>(_mapper.ConfigurationProvider);
            }

            if (noTracking)
            {
                return query.AsNoTracking().ProjectTo<U>(_mapper.ConfigurationProvider);
            }
            return query.ProjectTo<U>(_mapper.ConfigurationProvider);
        }

        #endregion

        public virtual List<TEntity> GetAllGroup(Expression<Func<TEntity, bool>> filter = null,

    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderyby = null,
    string includingProperties = null, bool noTracking = true,
            string[] groupBy = null
    )
        {

            IQueryable<TEntity> query = dbset;





            if (includingProperties != null)
            {
                foreach (var includeproperty in includingProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeproperty);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
                // query = query.Where(tt => !(bool)tt.GetType().GetProperty("IsDeleted").GetValue(tt, null));
            }
            if (groupBy != null)
            {
                var lambda = ExpressionEx.GroupByExpression<TEntity>(groupBy);
                return query.AsEnumerable().GroupBy(lambda.Compile()).SelectMany(cc => cc.ToList()).ToList();
            }

            //return lquery;
            if (orderyby != null)
            {
                if (noTracking)
                {
                    return orderyby(query).AsNoTracking().ToList();
                }
                return orderyby(query).ToList();
            }

            if (noTracking)
            {
                return query.AsNoTracking().ToList();
            }
            return query.ToList();
        }




        #region Crud Methods
        public virtual TEntity GetById(object id)
        {
            return dbset.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbset.Add(entity);
        }
        public virtual void Attach(TEntity entity)
        {
            //svar original = dbset.Find(entity.Id);
            dbset.Attach(entity);
        }
        public virtual void Insert(IEnumerable<TEntity> entityList)
        {
            foreach (var entity in entityList)
            {
                dbset.Add(entity);
            }

        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbset.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbset.Attach(entityToDelete);
            }
            dbset.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbset.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        #endregion



        //public virtual MultiResultSetReader GetMultiResultSet(string query, params SqlParameter[] parameters)
        //{
        //    return this.context.MultiResultSetSqlQuery(query, parameters);
        //}









    }
}
