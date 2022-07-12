using System.Collections.Generic;
using System.Threading.Tasks;

namespace I4_QM_app.Services.Data
{
    /// <summary>
    /// Interface to represent a generic data service.
    /// </summary>
    /// <typeparam name="T">Generic object.</typeparam>
    public interface IDataService<T>
    {
        /// <summary>
        /// Add item to data store.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <returns>Task.</returns>
        Task<bool> AddItemAsync(T item);

        /// <summary>
        /// Update item from data store.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <returns>Task.</returns>
        Task<bool> UpdateItemAsync(T item);

        /// <summary>
        /// Delete item from data store with id.
        /// </summary>
        /// <param name="id">Unique id.</param>
        /// <returns>Task.</returns>
        Task<bool> DeleteItemAsync(string id);

        /// <summary>
        /// Get item from data store with id.
        /// </summary>
        /// <param name="id">Unique id.</param>
        /// <returns>Task.</returns>
        Task<T> GetItemAsync(string id);

        /// <summary>
        /// Get items from data store.
        /// </summary>
        /// <returns>Task.</returns>
        Task<IEnumerable<T>> GetItemsAsync();

        /// <summary>
        /// Get items from data store with predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Task.</returns>
        Task<IEnumerable<T>> GetItemsFilteredAsync(System.Func<T, bool> predicate);

        /// <summary>
        /// Delete all items from data store.
        /// </summary>
        /// <returns>Task.</returns>
        Task<bool> DeleteAllItemsAsync();

        /// <summary>
        /// Delete many items from data store with predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Task.</returns>
        Task<bool> DeleteManyItemsAsync(System.Func<T, bool> predicate);
    }
}
