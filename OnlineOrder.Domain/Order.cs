using OnlineOrder.Domain.DTOs;
using Stateless;

namespace OnlineOrder.Domain
{
    public class Order
    {
        public Order(string _customerName)
        {
            if (string.IsNullOrWhiteSpace(_customerName))
                throw new ArgumentException("CustomerName required", nameof(_customerName));
            CustomerName = _customerName;
            _fsm = OrderStateMachine.Create(
                () => Status,
                s => { Status = s; UpdatedAt = DateTime.UtcNow; },
                (trigger, from, to) => { _history.Add(new(trigger, from, to, DateTime.UtcNow)); Touch(); }
                );
        }
        public Order()
        {

            CustomerName = null!;
            _fsm = OrderStateMachine.Create(
                () => Status,
                s => { Status = s; UpdatedAt = DateTime.UtcNow; },
                (trigger, from, to) => { _history.Add(new(trigger, from, to, DateTime.UtcNow)); Touch(); }
                );

        }
        private readonly StateMachine<OrderStatus, OrderTrigger> _fsm;

        private List<OrderStateChange> _history = [];

        private List<OrderedItem> _items = [];

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public string CustomerName { get; private set; }

        public IReadOnlyCollection<OrderStateChange> History
        {
            get => _history.AsReadOnly();
            set => _history = value.ToList();
        }

        public Guid Id { get; private set; } = Guid.NewGuid();

        public IReadOnlyCollection<OrderedItem> Items => _items.AsReadOnly();

        public OrderStatus Status { get; private set; } = OrderStatus.Created;

        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        public void AddItem(OrderedItem item)
        {
            var existing = _items.FirstOrDefault(i => i.Name == item.Name && i.Price == item.Price);
            if (existing != null)
                existing.IncreaseQuantity(item.Quantity);
            else
                _items.Add(item);
            Amount += item.Price * item.Quantity;
            Touch();
        }

        public bool CanCancel() => _fsm.CanFire(OrderTrigger.Cancel);

        public bool Cancel()
        {
            if (!CanCancel()) return false;
            _fsm.Fire(OrderTrigger.Cancel);
            return true;
        }

        public bool CanDeliver() => _fsm.CanFire(OrderTrigger.Deliver);

        public bool CanPay() => _fsm.CanFire(OrderTrigger.Pay);

        public bool Deliver()
        {
            if (!CanDeliver()) return false;
            _fsm.Fire(OrderTrigger.Deliver);
            return true;
        }

        public bool Fail()
        {
            if (!_fsm.CanFire(OrderTrigger.Fail)) return false;
            _fsm.Fire(OrderTrigger.Fail);
            return true;
        }

        public bool Pay()
        {
            if (!CanPay()) return false;
            _fsm.Fire(OrderTrigger.Pay);
            return true;
        }

        public bool RemoveItem(string itemName)
        {
            var toRemove = _items.Where(i => i.Name == itemName).ToList();
            if (toRemove.Count == 0) return false;

            foreach (var it in toRemove)
            {
                if (!it.DecreaseQuantity())
                    _items.Remove(it);

                Amount -= it.Price;
            }
            Touch();
            return true;
        }

        public override string ToString()
            => $"Заказ {Id} для {CustomerName} - {Status} - Сумма: {Amount:C}, количество товаров: {_items.Sum(it => it.Quantity)}";

        public void LoadHistory(IEnumerable<OrderStateChange> history)
        {
            _history.Clear();
            _history.AddRange(history);
        }
        private void Touch() => UpdatedAt = DateTime.UtcNow;
    }
}
