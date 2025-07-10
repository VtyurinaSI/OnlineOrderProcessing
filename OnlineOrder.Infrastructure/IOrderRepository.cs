using OnlineOrder.Domain;
using OnlineOrder.Domain.DTOs;

namespace OnlineOrder.Infrastructure
{
    public interface IOrderRepository
    {
        Task<Order?> GetAsync(Guid id);
        Task InsertAsync(Order order);
        Task UpdateAsync(Order order);
        Task<IEnumerable<OrderStateChange>> GetHistoryAsync(Guid orderId);
    }
}
