using System.Collections.Generic;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddOrderAsync(T item);
        Task<bool> UpdateOrderAsync(T item);
        Task<bool> DeleteOrderAsync(string id);
        Task<T> GetOrderAsync(string id);
        Task<IEnumerable<T>> GetOrdersAsync(bool forceRefresh = false);
    }
}
