using OnlineOrder.Domain;
using OnlineOrder.Domain.DTOs;
using OnlineOrder.Infrastructure;

namespace OnlineOrder.Application.UseCases
{
    public class OrderProcesses
    {
        public OrderProcesses(IOrderRepository repo) => this.repo = repo;

        IOrderRepository repo;
        public Order CreateOrder(string _customerName, params OrderedItem[] items)
        {
            Order order = new(_customerName);
            foreach (var item in items)
                order.AddItem(item);
            return order;
        }
        public async Task<Order?> GetOrderById(Guid id)
        {
            return await repo.GetAsync(id);
        }
        public bool Pay(Order order) => order.Pay();
        public bool Deliver(Order order) => order.Deliver();
        public bool Fail(Order order) => order.Fail();
        public bool Cancel(Order order) => order.Cancel();

        public async Task<IEnumerable<OrderStateChange>> GetOrderHistory(Guid id)
        {
            return await repo.GetHistoryAsync(id);
        }
    }
}
