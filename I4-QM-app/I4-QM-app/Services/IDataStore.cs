using System.Collections.Generic;
using System.Threading.Tasks;

namespace I4_QM_app.Helpers
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
        Task<IEnumerable<T>> GetItemsFilteredAsync(System.Func<T, bool> predicate);
        Task<bool> DeleteAllItemsAsync();
        Task<bool> DeleteManyItemsAsync();
    }
}
