namespace PlanStack.Backend.Database
{
    public class UnitOfWork
    {
        private readonly DatabaseContext _databaseContext;

        public UnitOfWork(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        #region SaveChangesAsync
        public async Task SaveChangesAsync()
        {
            await _databaseContext.SaveChangesAsync();
        }
        #endregion
    }
}
