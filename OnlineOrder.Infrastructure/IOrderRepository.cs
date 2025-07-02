using OnlineOrder.Domain;

namespace OnlineOrder.Infrastructure
{
    internal interface IOrderRepository
    {
        Task<Order?> GetAsync(Guid id);
        Task InsertAsync(Order order);
        Task UpdateAsync(Order order);
        Task<IEnumerable<OrderStateChange>> GetHistoryAsync(Guid orderId);
    }
}
