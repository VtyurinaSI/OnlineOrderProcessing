using FluentValidation;
using OnlineOrder.Application.Contracts;
using OnlineOrder.Application.DTOs;
using OnlineOrder.Domain;
using OnlineOrder.Domain.DTOs;

namespace OnlineOrder.Application
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private static readonly CreateOrderRequestValidator _createValidator = new();

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct = default)
        {
            var validation = await _createValidator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            var order = new Order(request.CustomerName);
            foreach (var it in request.Items)
            {
                order.AddItem(new OrderedItem(it.Name, it.Price, it.Quantity));
            }
            await _repository.InsertAsync(order, ct);
            return order;
        }

        public Task<Order?> GetOrderAsync(Guid id, CancellationToken ct)
            => _repository.GetAsync(id, ct);

        public Task<IEnumerable<OrderStateChange>> GetHistoryAsync(Guid id, CancellationToken ct = default)
            => _repository.GetHistoryAsync(id, ct);

        public Task<bool> PayAsync(Guid id, CancellationToken ct)
            => TriggerAsync(id, o => o.Pay(), ct);

        public Task<bool> DeliverAsync(Guid id, CancellationToken ct)
            => TriggerAsync(id, o => o.Deliver(), ct);

        public Task<bool> CancelAsync(Guid id, CancellationToken ct)
            => TriggerAsync(id, o => o.Cancel(), ct);

        public Task<bool> FailAsync(Guid id, CancellationToken ct)
            => TriggerAsync(id, o => o.Fail(), ct);

        private async Task<bool> TriggerAsync(Guid id, Func<Order, bool> transition, CancellationToken ct)
        {
            var order = await _repository.GetAsync(id, ct);
            if (order is null) return false;
            if (!transition(order)) return false;
            await _repository.UpdateAsync(order, ct);
            return true;
        }
    }
}
