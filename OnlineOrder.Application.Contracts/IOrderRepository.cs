using OnlineOrder.Domain;
using OnlineOrder.Domain.DTOs;

namespace OnlineOrder.Application.Contracts
{
    public interface IOrderRepository
    {
        Task<Order?> GetAsync(Guid id, CancellationToken token);
        Task InsertAsync(Order order, CancellationToken token);
        Task UpdateAsync(Order order, CancellationToken token);
        Task<IEnumerable<OrderStateChange>> GetHistoryAsync(Guid orderId, CancellationToken token);
    }
}
