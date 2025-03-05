using Microsoft.EntityFrameworkCore;

namespace Psg.Common.Registrations.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task<List<T>> ToListNoLockAsync<T>(this DbContext context,
                                                               IQueryable<T> query)
        {

            var strategy = context.Database.CreateExecutionStrategy();

            var results = await strategy.ExecuteAsync(async () =>
            {
                using var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                try
                {
                    var results = await query.ToListAsync();

                    transaction.Commit();

                    return results;

                }
                catch (Exception)
                {
                    // Rollback the transaction if there is an exception
                    transaction.Rollback();
                    throw;
                }
            });

            return results;
        }

        public static async Task<T?> FirstOrDefaultNoLockAsync<T>(this DbContext context,
                                                               IQueryable<T> query)
        {

            var strategy = context.Database.CreateExecutionStrategy();

            var results = await strategy.ExecuteAsync(async () =>
            {
                using var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                try
                {
                    var results = await query.FirstOrDefaultAsync();

                    transaction.Commit();

                    return results;

                }
                catch (Exception)
                {
                    // Rollback the transaction if there is an exception
                    transaction.Rollback();
                    throw;
                }
            });

            return results;
        }
    }
}
