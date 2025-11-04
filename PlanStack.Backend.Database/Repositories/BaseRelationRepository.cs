using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Extensions;
using PlanStack.Backend.Database.QueryModels;

namespace PlanStack.Backend.Database.Repositories
{
    public abstract class BaseRelationRepository<T, Q>
           where T : BaseRelationDataModel, new()
           where Q : BaseQueryModel
    {
        protected readonly DatabaseContext context;

        public BaseRelationRepository(
            DatabaseContext context
        )
        {
            this.context = context;
        }

        #region GetAsync
        public virtual async Task<T> GetAsync(int entityId, bool includeRelated = false)
        {
            T result = null;

            if (!includeRelated)
                result = await context.Set<T>().FindAsync(entityId);
            else
            {
                var query = context.Set<T>().AsQueryable();

                result = await query.SingleOrDefaultAsync(x => x.Id == entityId);
            }

            return result;
        }
        #endregion

        #region GetAllAsync
        public virtual async Task<BaseQueryResult<T>> GetAllAsync(Q queryModel, bool includeRelated = false)
        {
            var result = new BaseQueryResult<T>();
            var query = context.Set<T>().AsQueryable();

            // Count
            result.Count = await query.CountAsync();

            // Paging
            query = query.ApplyPaging(queryModel);

            result.Entities = await query.ToListAsync();

            return result;
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
