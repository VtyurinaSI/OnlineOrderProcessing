using OnlineOrder.Application.DTOs;
using OnlineOrder.Domain;
using OnlineOrder.Domain.DTOs;

namespace OnlineOrder.Application.Contracts
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderRequest request, CancellationToken token);
        Task<Order?> GetOrderAsync(Guid id, CancellationToken token);
        Task<IEnumerable<OrderStateChange>> GetHistoryAsync(Guid id, CancellationToken token);

        Task<bool> PayAsync(Guid id, CancellationToken token);
        Task<bool> DeliverAsync(Guid id, CancellationToken token);
        Task<bool> CancelAsync(Guid id, CancellationToken token);
        Task<bool> FailAsync(Guid id, CancellationToken token);
    }
}
