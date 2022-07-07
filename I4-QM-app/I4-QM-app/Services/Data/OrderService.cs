using I4_QM_app.Models;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    /// <summary>
    /// Implementation of IDataService for Orders with LiteDB.
    /// </summary>
    public class OrderService : IDataService<Order>
    {
        private readonly ILiteCollection<Order> orderCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class.
        /// </summary>
        public OrderService()
        {
            orderCollection = App.DB.GetCollection<Order>("orders");
        }

        /// <summary>
        /// Add order item to data store.
        /// </summary>
        /// <param name="order">Order.</param>
        /// <returns>Task.</returns>
        public async Task<bool> AddItemAsync(Order order)
        {
            orderCollection.Insert(order);
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Update order item from data store.
        /// </summary>
        /// <param name="order">Order.</param>
        /// <returns>Task.</returns>
        public async Task<bool> UpdateItemAsync(Order order)
        {
            orderCollection.Update(order);
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Delete order item from data store with id.
        /// </summary>
        /// <param name="id">Unique id.</param>
        /// <returns>Task.</returns>
        public async Task<bool> DeleteItemAsync(string id)
        {
            orderCollection.Delete(id);
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Delete many order items from data store with predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Task.</returns>
        public async Task<bool> DeleteManyItemsAsync(System.Func<Order, bool> predicate)
        {
            // TODO Func predicate -> BsonExpression ???
            var list = orderCollection.FindAll().Where(predicate).ToList();
            list.ForEach(i => orderCollection.Delete(i.Id));
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Delete all order items from data store.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task<bool> DeleteAllItemsAsync()
        {
            orderCollection.DeleteAll();
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Get order item from data store with id.
        /// </summary>
        /// <param name="id">Unique id.</param>
        /// <returns>Task.</returns>
        public async Task<Order> GetItemAsync(string id)
        {
            var order = orderCollection.FindAll().FirstOrDefault(a => a.Id == id);
            return await Task.FromResult(order);
        }

        /// <summary>
        /// Get order items from data store.
        /// </summary>
        /// <returns>Task.</returns
        public async Task<IEnumerable<Order>> GetItemsAsync()
        {
            return await Task.FromResult(orderCollection.FindAll());
        }

        /// <summary>
        /// Get order items from data store with predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Task.</returns>
        public async Task<IEnumerable<Order>> GetItemsFilteredAsync(System.Func<Order, bool> predicate)
        {
            return await Task.FromResult(orderCollection.FindAll().Where(predicate).ToList());
        }
    }
}
