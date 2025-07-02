using Stateless;

namespace OnlineOrder.Domain
{
    internal class OrderStateMachine
    {
        public static StateMachine<OrderStatus, OrderTrigger> Create(
        Func<OrderStatus> getStatus,
        Action<OrderStatus> setStatus,
        Action<OrderTrigger, OrderStatus, OrderStatus>? onTransition = null)
        {
            var sm = new StateMachine<OrderStatus, OrderTrigger>(getStatus, setStatus);

            sm.Configure(OrderStatus.Created)
                .Permit(OrderTrigger.Pay, OrderStatus.Paid)
                .Permit(OrderTrigger.Cancel, OrderStatus.Cancelled)
                .Permit(OrderTrigger.Fail, OrderStatus.Error);

            sm.Configure(OrderStatus.Paid)
                .Permit(OrderTrigger.Deliver, OrderStatus.Delivered)
                .Permit(OrderTrigger.Cancel, OrderStatus.Cancelled)
                .Permit(OrderTrigger.Fail, OrderStatus.Error);

            sm.Configure(OrderStatus.Delivered)
                .Permit(OrderTrigger.Fail, OrderStatus.Error);

            sm.Configure(OrderStatus.Cancelled)
                .Permit(OrderTrigger.Fail, OrderStatus.Error);

            sm.Configure(OrderStatus.Error)
                .Ignore(OrderTrigger.Pay)
                .Ignore(OrderTrigger.Deliver)
                .Ignore(OrderTrigger.Cancel);


            sm.OnTransitioned(t =>
            {
                onTransition?.Invoke(t.Trigger, t.Source, t.Destination);
            });

            return sm;
        }
    }
}
