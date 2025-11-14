using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Extensions;
using PlanStack.Backend.Database.QueryModels;
using System.Linq.Expressions;

namespace PlanStack.Backend.Database.Repositories
{
    public abstract class BaseRepository<T, Q, TKey>
           where T : BaseDataModel<TKey>, new()
           where Q : BaseQueryModel
    {
        protected readonly DatabaseContext context;

        public BaseRepository(
            DatabaseContext context
        )
        {
            this.context = context;
        }

        #region GetAsync
        public virtual async Task<T> GetAsync(TKey entityId, bool includeRelated = false)
        {
            T result = null;

            if (!includeRelated)
                result = await context.Set<T>().FindAsync(entityId);
            else
            {
                var query = context.Set<T>().AsQueryable();

                // Relations
                query = ApplyRelations(query);

                result = await query.SingleOrDefaultAsync(x => x.Id.Equals(entityId));
            }

            return result;
        }
        #endregion

        #region GetAllAsync
        public virtual async Task<BaseQueryResult<T>> GetAllAsync(Q queryModel, bool includeRelated = false)
        {
            var result = new BaseQueryResult<T>();
            var query = context.Set<T>().AsQueryable();

            // Filtering
            query = ApplyFiltering(query, queryModel);

            // Ordering
            query = query.ApplyOrdering(queryModel, ApplyOrderingMap());

            // Count
            result.Count = await query.CountAsync();

            // Paging
            query = query.ApplyPaging(queryModel);

            // Relations
            if (includeRelated)
                query = ApplyRelations(query);

            result.Entities = await query.ToListAsync();

            return result;
        }
        #endregion

        #region ApplyFiltering
        protected virtual IQueryable<T> ApplyFiltering(IQueryable<T> query, Q queryModel)
        {
            return query;
        }
        #endregion

        #region ApplyOrderingMap
        protected virtual Dictionary<string, Expression<Func<T, object>>> ApplyOrderingMap()
        {
            return new Dictionary<string, Expression<Func<T, object>>>()
            {
                ["id"] = x => x.Id,
            };
        }
        #endregion

        #region ApplyRelations
        protected virtual IQueryable<T> ApplyRelations(IQueryable<T> query)
        {
            return query;
        }
        #endregion

        #region Add
        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }
        #endregion

        #region Update
        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }
        #endregion

        #region Remove
        public void Remove(T entity)
        {
            context.Set<T>().Remove(entity);
        }
        #endregion
    }
}
