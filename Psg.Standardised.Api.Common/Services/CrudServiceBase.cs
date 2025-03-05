namespace Psg.Standardised.Api.Common.Services
{
    public class CrudServiceBase
    {
        /// <summary>
        /// Optional code to execute when performing authorisation. Fetches additional input values using the entity.
        /// </summary>
        public virtual Task<(int? personId, int? entityId)> GetPersonFromEntityAsync(int entityId)
            => Task.FromResult<(int?, int?)>((default, entityId)); // this function should be implemented in child class if required.

        /// <summary>
        /// Optional code to execute before Insert query is executed
        /// </summary>
        public virtual Task OnInsertAsync(object model)
            => Task.CompletedTask;

        /// <summary>
        /// Optional code to execute before Update query is executed
        /// </summary>
        public virtual Task OnUpdateAsync(object model)
            => Task.CompletedTask;

        /// <summary>
        /// Optional code to execute before Insert/Update query is executed
        /// </summary>
        public virtual Task OnSaveAsync(object model)
            => Task.CompletedTask;

        /// <summary>
        /// Optional code to execute before Delete query is executed
        /// </summary>
        public virtual Task OnDeleteAsync(long id)
            => Task.CompletedTask;

        /// <summary>
        /// Optional code to execute after Insert query has executed
        /// </summary>
        public virtual Task OnInsertCompletedAsync(object model)
            => Task.CompletedTask;

        /// <summary>
        /// Optional code to execute after Insert/Update query has executed
        /// </summary>
        public virtual Task OnSaveCompletedAsync(object model)
            => Task.CompletedTask;

        /// <summary>
        /// Optional code to execute after Delete query has executed
        /// </summary>
        public virtual Task OnDeleteCompletedAsync(object model)
            => Task.CompletedTask;
    }
}
